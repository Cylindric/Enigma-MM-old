﻿using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using LibNbt.Tags;

namespace EnigmaMM.Engine
{
    /// <summary>
    /// The main Server Manager class.
    /// Keeps track of the server listener, and manages the Minecraft process.
    /// </summary>
    public class EMMServer
    {
        private const int COMMAND_TIMEOUT_MS = 5000;

        private Process mServerProcess;
        private Status mServerStatus;
        private string mStatusMessage;
        private bool mServerSaving;
        private CommandParser mParser;
        private Scheduler.SchedulerManager mScheduler;
        private Config mSettings;
        private PowerManager mPowerManager;
        private UserManager mUserManager;
        private string mServerRoot;

        // Thread lock objects
        private readonly object mAutoSaveLock = new object();

        // Java and Minecraft Server settings
        private System.IO.StreamWriter mCommandInjector;
        private MCServerProperties mMinecraftSettings;
        private ArrayList mSavedUsers;
        private ArrayList mOnlineUsers;
        private int mAutoSaveBlocks;
        private bool mAutoSaveEnabled;
        private SettingsFile mMinecraftWhitelist;

        #region Interface EMMServer Events

        /// <summary>
        /// Raised whenever the Minecraft server stops.
        /// </summary>
        public event EventHandler<ServerMessageEventArgs> ServerStopped;

        /// <summary>
        /// Raised whenever the Minecraft server starts.
        /// </summary>
        public event EventHandler<ServerMessageEventArgs> ServerStarted;

        /// <summary>
        /// Raised whenever the Minecraft server sends a message.
        /// </summary>
        public event EventHandler<ServerMessageEventArgs> ServerMessage;

        /// <summary>
        /// Raised whenever the Minecraft server throws an error.
        /// </summary>
        public event EventHandler<ServerMessageEventArgs> ServerError;

        /// <summary>
        /// Raised whenever the Minecraft server status changes.
        /// </summary>
        public event EventHandler<ServerMessageEventArgs> StatusChanged;

        #endregion
        
        #region Interface IServer Properties

        public Config Settings
        {
            get { return mSettings; }
        }

        public MCServerProperties MinecraftSettings
        {
            get { return mMinecraftSettings; }
        }

        public Status CurrentStatus
        {
            get { return mServerStatus; }
        }

        public string LastStatusMessage
        {
            get { return mStatusMessage; }
        }

        public ArrayList Users
        {
            get { return mOnlineUsers; }
        }
        
        #endregion
        
        /// <summary>
        /// Starts the Minecraft server process.
        /// </summary>
        /// <remarks>
        /// Note that the server is started asynchronously, so CurrentStatus
        /// should be queried to determine when (if!) the server successfully started.
        /// </remarks>
        public void StartServer()
        {
            mPowerManager.StartServer();
        }

        /// <summary>
        /// Shuts down the running Server.
        /// </summary>
        /// <remarks><see cref="PowerManager.StopServer"/></remarks>
        public void StopServer(bool graceful)
        {
            mPowerManager.StopServer(graceful, -1, false);
        }

        /// <summary>
        /// Shuts down the running Server.
        /// </summary>
        /// <remarks><see cref="PowerManager.StopServer"/></remarks>
        public void StopServer(bool graceful, int timeout, bool force)
        {
            mPowerManager.StopServer(graceful, timeout, force);
        }

        /// <summary>
        /// Performs a restart of the server.
        /// </summary>
        /// <remarks><see cref="PowerManager.RestartServer"/></remarks>
        public void RestartServer(bool graceful)
        {
            mPowerManager.RestartServer(graceful);
        }

        /// <summary>
        /// Aborts a pending stop operation.
        /// </summary>
        public void AbortPendingOperations()
        {
            if ((mServerStatus == Status.Running) &&
                ((mServerStatus == Status.PendingStop) || (mServerStatus == Status.PendingRestart)))
            {
                ServerStatus = Status.Running;
            }
        }

        /// <summary>
        /// Sends a broadcast message to all players on the server.
        /// </summary>
        /// <param name="message">Message to send</param>
        public void Broadcast(string message)
        {
            SendCommand("say " + message);
        }

        /// <summary>
        /// Parses commands and executes them.  Anything unknown is sent to the Minecraft server.
        /// </summary>
        /// <param name="command">Command to parse</param>
        public void Execute(string command)
        {
            bool executed = false;
            EMMServerMessage message = new EMMServerMessage(command);
            message.SetUser("console");
            executed = mParser.ParseCommand(message);
            if (!executed)
            {
                SendCommand(command);
            }
        }

        /// <summary>
        /// Helper-method to raise ServerMessage Events from other places.
        /// </summary>
        /// <param name="Message">The message to throw</param>
        public void RaiseServerMessage(string Message)
        {
            RaiseServerMessage(Message, new object[0]);
        }

        /// <summary>
        /// Helper-method to raise ServerMessage Events from other places.
        /// </summary>
        /// <param name="Message">The message to throw</param>
        /// <param name="args">Arguments to pass into FormatString.</param>
        public void RaiseServerMessage(string Message, params object[] args)
        {
            if (ServerMessage != null)
            {
                if (args.Length == 0)
                {
                    ServerMessage(this, new ServerMessageEventArgs(Message));
                }
                else
                {
                    ServerMessage(this, new ServerMessageEventArgs(string.Format(Message, args)));
                }
            }
        }

        /// <summary>
        /// Disables server auto-save by incrementing a 'block' counter. Autosaves are not
        /// resumed until all blocks have been released.  <see cref="UnblockAutoSave"/>
        /// </summary>
        public void BlockAutoSave()
        {
            lock (mAutoSaveLock)
            {
                mAutoSaveBlocks += 1;
                if ((mAutoSaveEnabled) && (mAutoSaveBlocks > 0))
                {
                    SetAutoSave(false);
                }
            }
        }

        /// <summary>
        /// Re-enables server auto-save by decrementing a 'block' counter. Autosaves are not
        /// resumed until all blocks have been released.  <see cref="BlockAutoSave"/>
        /// </summary>
        public void UnblockAutoSave()
        {
            lock (mAutoSaveLock)
            {
                mAutoSaveBlocks -= 1;
                if ((!mAutoSaveEnabled) && (mAutoSaveBlocks == 0))
                {
                    SetAutoSave(true);
                }
            }
        }

        #region Internal Properties

        internal Process ServerProcess
        {
            set { mServerProcess = value; }
            get { return mServerProcess; }
        }

        internal System.IO.StreamWriter CommandInjector
        {
            set { mCommandInjector = value; }
            get { return mCommandInjector; }
        }

        internal Status ServerStatus
        {
            get { return mServerStatus; }
            set { 
                mServerStatus = value;
                if (StatusChanged != null)
                {
                    StatusChanged(this, new ServerMessageEventArgs(mServerStatus.ToString()));
                }
            }
        }

        #endregion

        #region Public Constructors

        /// <summary>
        /// Server Constructor
        /// </summary>
        /// <remarks>Defaults to using a config file in the same location as the executing assembly.</remarks>
        public EMMServer()
        {
            mSettings = new Config(this);
 
            mMinecraftSettings = new MCServerProperties(this);
            mMinecraftWhitelist = new SettingsFile(this, Path.Combine(mSettings.MinecraftRoot, "white-list.txt"), ' ');

            mParser = new CommandParser(this);
            mScheduler = new Scheduler.SchedulerManager(this);
            mPowerManager = new PowerManager(this);
            mUserManager = new UserManager(this);

            mServerSaving = false;
            ServerStatus = Status.Stopped;
            mSavedUsers = new ArrayList();
            mOnlineUsers = new ArrayList();
            mAutoSaveBlocks = 0;
            mAutoSaveEnabled = true;
            
            // See if we need to swap in a new config file, and load current config.
            mMinecraftSettings.LookForNewSettings();
            mMinecraftWhitelist.LookForNewSettings();

            mScheduler.LoadSchedule(Path.Combine(mSettings.ServerManagerRoot, "scheduler.xml"));
            mScheduler.Start();

            mUserManager.UpdateAllPositionsFromFile();
            mUserManager.MonitorUserFiles();
        }

        #endregion

        /// <summary>
        /// Enables or disables server auto-save.
        /// </summary>
        /// <param name="enabled">True turns on auto-save, false turns it off</param>
        private void SetAutoSave(bool enabled)
        {
            if (enabled)
            {
                SendCommand("save-on");
                Thread.Sleep(1000); // bit of a hack to give the server a chance to respond
            }
            else
            {
                SendCommand("save-off");
                Thread.Sleep(1000); // bit of a hack to give the server a chance to respond
            }
        }

        /// <summary>
        /// Sends an arbitrary command to the Minecraft server.
        /// </summary>
        /// <param name="Command">Command to send</param>
        internal void SendCommand(string Command)
        {
            if ((mServerStatus == Status.Running) || (mServerStatus == Status.PendingStop) || (mServerStatus == Status.PendingRestart))
            {
                mCommandInjector.WriteLine(Command);
            }
        }


        internal void Whisper(Data.User User, string Message)
        {
            SendCommand(string.Format("tell {0} {1}", User.Username, Message));
        }


        /// <summary>
        /// Called whenever the server issues a message.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="OutLine"></param>
        internal void ServerOutputHandler(object sender, DataReceivedEventArgs OutLine)
        {
            if (OutLine.Data == null)
            {
                return;
            }

            EMMServerMessage M = new EMMServerMessage(OutLine.Data);

            switch (M.Type)
            {
                case EMMServerMessage.MessageTypes.AutoSaveEnabled:
                    mAutoSaveEnabled = true;
                    break;

                case EMMServerMessage.MessageTypes.AutoSaveDisabled:
                    mAutoSaveEnabled = false;
                    break;

                case EMMServerMessage.MessageTypes.ErrorPortBusy:
                    OnServerError("Error starting server: port " + mMinecraftSettings.ServerPort + " in use");
                    ServerStatus = Status.Failed;
                    mStatusMessage = M.Message;
                    mPowerManager.ForceShutdown();
                    break;

                case EMMServerMessage.MessageTypes.SaveStarted:
                    mServerSaving = true;
                    break;

                case EMMServerMessage.MessageTypes.SaveComplete:
                    mServerSaving = false;
                    break;

                case EMMServerMessage.MessageTypes.StartupComplete:
                    mOnlineUsers = new ArrayList();
                    OnServerStarted("Server started");
                    break;

                case EMMServerMessage.MessageTypes.UserLoggedIn:
                    OnUserJoined(M);
                    break;

                case EMMServerMessage.MessageTypes.ServerCommand:
                case EMMServerMessage.MessageTypes.TriedServerCommand:
                    mParser.ParseCommand(M);
                    break;

                case EMMServerMessage.MessageTypes.UserList:
                    int oldUserCount = mOnlineUsers.Count;
                    mOnlineUsers = new ArrayList(M.Data["userlist"].Split(','));
                    if (oldUserCount > 0 && mOnlineUsers.Count == 0)
                    {
                        OnServerReachZeroUsers();
                    }
                    break;

                case EMMServerMessage.MessageTypes.UserLoggedOut:
                case EMMServerMessage.MessageTypes.UserFloating:
                    mOnlineUsers.Remove(M.Data["username"]);
                    if (mOnlineUsers.Count == 0)
                    {
                        OnServerReachZeroUsers();
                    }
                    break;
            }

            // raise an InfoMessage Event too
            RaiseServerMessage(M.Message);
        }

        #region Server Events

        /// <summary>
        /// Called when the Minecraft server process terminates.
        /// </summary>
        /// <remarks>
        /// Don't put any logic in here, keep it in the standard onServerStopped event handler.</remarks>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        internal void ServerExited(object sender, System.EventArgs args)
        {
            mOnlineUsers.Clear();
            OnServerStopped("Server Stopped");
            RaiseServerMessage("Stopped");
        }
        
        /// <summary>
        /// Called when the minecraft server has fully started.
        /// </summary>
        /// <remarks>
        /// Raises event ServerStarted.
        /// </remarks>
        /// <param name="Message"></param>
        private void OnServerStarted(string Message)
        {
            ServerStatus = Status.Running;
            mMinecraftSettings.Load();
            if (ServerStarted != null)
            {
                ServerStarted(this, new ServerMessageEventArgs(Message));
            }
            RaiseServerMessage("Started");
        }

        /// <summary>
        /// Called when a user connects
        /// </summary>
        /// <param name="Message"></param>
        internal void OnUserJoined(EMMServerMessage Message)
        {
            Data.User user = mUserManager.OnUserJoinedMessage(Message);            
            mOnlineUsers.Add(user.Username);
        }

        /// <summary>
        /// Called when the Minecraft server has stopped.
        /// </summary>
        /// <remarks>Raises event <see cref="ServerStopped"/>.</remarks>
        /// <param name="Message"></param>
        internal void OnServerStopped(string Message)
        {
            ServerStatus = Status.Stopped;
            mOnlineUsers.Clear();
            if (ServerStopped != null)
            {
                ServerStopped(this, new ServerMessageEventArgs(Message));
            }
            OnServerReachZeroUsers();
            mServerProcess = null;

            // Update the Biome Data
            EnigmaMM.Engine.Commands.Mappers.Mapper mapper = new EnigmaMM.Engine.Commands.Mappers.BiomeExtractor();
            mapper.RenderMap();
        }

        /// <summary>
        /// Called when the last user logs out.
        /// </summary>
        private void OnServerReachZeroUsers()
        {
            if (mServerStatus == Status.PendingRestart)
            {
                mPowerManager.RestartServer(false);
            }
            if (mServerStatus == Status.PendingStop)
            {

                mPowerManager.StopServer(false, -1, false);
            }
        }

        /// <summary>
        /// Called when the minecraft server reports an error.
        /// </summary>
        /// <remarks>
        /// Raises event ServerError.
        /// </remarks>
        /// <param name="Message">The error message.</param>
        private void OnServerError(string Message)
        {
            if (ServerError != null)
            {
                ServerError(this, new ServerMessageEventArgs(Message));
            }
        }

        #endregion

        /// <summary>
        /// Releases all the resources used by the MCServer.
        /// </summary>
        public void Dispose()
        {
            mPowerManager.StopServer(false, 1, true);
            if (mServerProcess != null)
            {
                mServerProcess.Dispose();
            }
        }

    }
}

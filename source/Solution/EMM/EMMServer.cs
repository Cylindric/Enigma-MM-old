﻿using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using EnigmaMM.Interfaces;

namespace EnigmaMM
{
    /// <summary>
    /// The main Server Manager class.
    /// Keeps track of the server listener, and manages the Minecraft process.
    /// </summary>
    public class EMMServer: IServer
    {
        private const int COMMAND_TIMEOUT_MS = 5000;

        private Process mServerProcess;
        private Status mServerStatus;
        private string mStatusMessage;
        private bool mOnlineUserListReady;
        private CommsServer mCommsServer;
        private CommandParser mParser;
        private bool mServerSaving;
        private Scheduler.SchedulerManager mScheduler;

        // Java and Minecraft Server settings
        private System.IO.StreamWriter mCommandInjector;
        private MCServerProperties mServerProperties;
        private MCServerWarps mServerWarps;
        private bool mServerRunningHMod;
        private int mHModversion;
        private ArrayList mSavedUsers;
        private ArrayList mOnlineUsers;
        private int mAutoSaveBlocks;
        private bool mAutoSaveEnabled;

        #region Server Events

        /// <summary>
        /// Standard event handler for server messages.
        /// </summary>
        /// <param name="Message">the message</param>
        //public delegate void ServerMessageEventHandler(string Message);
        
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

        /// <summary>
        /// Valid status-states for the server manager's Minecraft instance.
        /// </summary>
        public enum Status
        {
            Starting,
            Running,
            Busy,
            PendingRestart,
            PendingStop,
            Stopping,
            Stopped,
            Failed
        }

        #region Public Properties

        public MCServerProperties ServerProperties
        {
            get { return mServerProperties; }
        }

        public MCServerWarps ServerWarps
        {
            get { return mServerWarps; }
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

        public bool Listening
        {
            get { return mCommsServer.Listening; }
        }

        #endregion

        private Status ServerStatus
        {
            set { 
                mServerStatus = value;
                if (StatusChanged != null)
                {
                    StatusChanged(this, new ServerMessageEventArgs(mServerStatus.ToString()));
                }
            }
        }

        /// <summary>
        /// Server Constructor
        /// </summary>
        /// <remarks>Defaults to using a config file in the same location as the executing assembly.</remarks>
        public EMMServer():
            this(Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase.Substring(8)), "settings.conf")){}

        /// <summary>
        /// Server Constructor
        /// </summary>
        public EMMServer(string mainSettingsFile)
        {
            Settings.Initialise(mainSettingsFile);

            mServerProperties = new MCServerProperties();
            mCommsServer = new CommsServer();
            mParser = new CommandParser(this);
            mScheduler = new Scheduler.SchedulerManager(this);

            ServerStatus = Status.Stopped;
            mOnlineUserListReady = false;
            mServerSaving = false;
            mServerRunningHMod = false;
            mHModversion = 0;
            mSavedUsers = new ArrayList();
            mOnlineUsers = new ArrayList();
            mAutoSaveBlocks = 0;
            mAutoSaveEnabled = true;
            
            if (Settings.AlphaVespucciInstalled)
            {
                MapManager.Register("av", new AlphaVespucci(this));
            }
            if (Settings.OverviewerInstalled)
            {
                MapManager.Register("overviewer", new Overviewer(this));
            }

            // See if we need to swap in a new config file, and load current config.
            ReloadConfig();

            mScheduler.LoadSchedule();
        }

        /// <summary>
        /// Starts the CommsServer listening for new connections.
        /// </summary>
        public void StartCommsServer()
        {
            if (Settings.ServerListenIp.Equals("none", StringComparison.CurrentCultureIgnoreCase))
            {
                RaiseServerMessage("Bypassing RCON due to user preference");
            }
            else
            {
                mCommsServer.StartListener();
                mCommsServer.MessageReceived += OnRemoteCommandReceived;
            }
        }

        /// <summary>
        /// Stops the CommsServer from listening for new connections.
        /// </summary>
        public void StopCommsServer()
        {
            mCommsServer.StopListener();
        }

        /// <summary>
        /// Reloads the Minecraft server properties files.
        /// </summary>
        public void ReloadConfig()
        {
            mServerProperties.LookForNewSettings();
            if (mServerProperties.WarpLocation != "")
            {
                mServerWarps = new MCServerWarps(mServerProperties.WarpLocation);
                mServerWarps.LookForNewSettings();
            }
        }

        /// <summary>
        /// Starts the Minecraft server process.
        /// </summary>
        /// <remarks>
        /// Note that the server is started asynchronously, so CurrentStatus
        /// should be queried to determine when (if!) the server successfully started.
        /// </remarks>
        public void StartServer()
        {
            if (mServerStatus == Status.Running)
            {
                RaiseServerMessage("Server already running, cannot start!");
                return;
            }

            ServerStatus = Status.Starting;
            ReloadConfig();

            if (Directory.Exists(Settings.MinecraftRoot) == false)
            {
                RaiseServerMessage("ERROR");
                RaiseServerMessage("Could not find Minecraft root directory");
                RaiseServerMessage("Check that configuration option 'MinecraftRoot' is correct");
                RaiseServerMessage("Looking for: " + Settings.MinecraftRoot);
                ServerStatus = Status.Failed;
                mStatusMessage = string.Format("Couldn't find Minecraft directory in {0}", Settings.MinecraftRoot);
                return;
            }
            if (File.Exists(Path.Combine(Settings.MinecraftRoot, Settings.ServerJar)) == false)
            {
                RaiseServerMessage("ERROR");
                RaiseServerMessage("Could not find the Minecraft server file");
                RaiseServerMessage("Check that configuration option 'ServerJar' is correct");
                RaiseServerMessage("Looking for: " + Path.Combine(Settings.MinecraftRoot, Settings.ServerJar));
                ServerStatus = Status.Failed;
                mStatusMessage = string.Format("Couldn't find Minecraft server at {0}", Path.Combine(Settings.MinecraftRoot, Settings.ServerJar));
                return;
            }

            string cmdArgs = "";
            if (Settings.JavaHeapInit > 0)
            {
                cmdArgs += "-Xms" + Settings.JavaHeapInit + "M ";
            }
            if (Settings.JavaHeapMax > 0)
            {
                cmdArgs += "-Xmx" + Settings.JavaHeapMax + "M ";
            }
            cmdArgs += "-jar \"" + Settings.ServerJar + "\" ";
            cmdArgs += "nogui ";

            // Configure the main server process
            mServerProcess = new Process();
            if (Settings.ServerJar.EndsWith(".exe"))
            {
                mServerProcess.StartInfo.FileName = Path.Combine(Settings.MinecraftRoot, Settings.ServerJar);
            }
            else
            {
                mServerProcess.StartInfo.FileName = Settings.JavaExec;
            }
            mServerProcess.StartInfo.CreateNoWindow = true;
            mServerProcess.StartInfo.WorkingDirectory = Settings.MinecraftRoot;
            mServerProcess.StartInfo.Arguments = cmdArgs;
            mServerProcess.StartInfo.UseShellExecute = false;
            mServerProcess.StartInfo.RedirectStandardError = true;
            mServerProcess.StartInfo.RedirectStandardInput = true;
            mServerProcess.StartInfo.RedirectStandardOutput = true;
            mServerProcess.EnableRaisingEvents = true;

            // Wire up an event handler to catch messages out of the process
            // Minecraft uses a mix of standard output and error output, with important messages 
            // on both.  Therefore, we just wire up both to a single handler.
            mServerProcess.OutputDataReceived += new DataReceivedEventHandler(ServerOutputHandler);
            mServerProcess.ErrorDataReceived += new DataReceivedEventHandler(ServerOutputHandler);
            mServerProcess.Exited += new EventHandler(ServerExited);

            // Start the server process
            SetOnlineUserList();
            mServerProcess.Start();

            // Wire up the writer to send messages to the process
            mCommandInjector = mServerProcess.StandardInput;
            mCommandInjector.AutoFlush = true;

            // Start listening for output
            mServerProcess.BeginOutputReadLine();
            mServerProcess.BeginErrorReadLine();

            RaiseServerMessage("Server starting...");
        }

        /// <summary>
        /// Shuts down the running Server.
        /// </summary>
        /// <remarks>Returns immediately, without waiting for the server to actually stop.</remarks>
        public void StopServer()
        {
            StopServer(-1, false);
        }

        /// <summary>
        /// Shuts down the running Server.
        /// </summary>
        /// <param name="timeout">
        /// Time in milliseconds to wait for the command to complete.
        /// Set to zero to wait forever, or -1 to return immediately, thus essentially running the command asynchronously.
        /// </param>
        /// <param name="force">
        /// If set to true, if the server is still running after the timeout it will be forcefully terminated.
        /// </param>
        public void StopServer(int timeout, bool force)
        {
            bool neverTimeout = (timeout == 0);

            if ((mServerStatus == Status.Running) || (mServerStatus == Status.PendingStop) || (mServerStatus == Status.PendingRestart))
            {
                SendCommand("stop");
                ServerStatus = Status.Stopping;
                while (((timeout > 0) || (neverTimeout)) && (mServerStatus != Status.Stopped))
                {
                    timeout -= 100;
                    Thread.Sleep(100);
                }
            }
            if (force)
            {
                ForceShutdown();
            }
        }

        /// <summary>
        /// Performs a simple restart of the server.
        /// </summary>
        /// <remarks>
        /// Same as StopServer() followed by StartServer().
        /// </remarks>
        public void RestartServer()
        {
            StopServer(0, false);
            StartServer();
        }

        /// <summary>
        /// Performs a graceful shutdown of the server.  
        /// </summary>
        /// <remarks>
        /// This will put the server in the "pending shutdown" state, whereby it waits
        /// until all users have logged out, then shuts down the server.
        /// </remarks>
        public void GracefulStop()
        {
            if (mOnlineUsers.Count == 0)
            {
                StopServer();
            }
            else
            {
                ServerStatus = Status.PendingStop;
            }
        }

        /// <summary>
        /// Aborts a pending stop operation.
        /// </summary>
        public void AbortPendingStop()
        {
            if ((mServerStatus == Status.Running) && (mServerStatus == Status.PendingStop))
            {
                ServerStatus = Status.Running;
            }
        }
        
        /// <summary>
        /// Performs a graceful restart of the server.
        /// </summary>
        /// <remarks>
        /// This will put the server in the "pending restart" state, whereby it waits
        /// until all users have logged out, then restarts the server.
        /// </remarks>
        public void GracefulRestart()
        {
            if (mOnlineUsers.Count == 0)
            {
                RestartServer();
            }
            else
            {
                ServerStatus = Status.PendingRestart;
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
        /// Aborts a pending restart.
        /// </summary>
        public void AbortPendingRestart()
        {
            if ((mServerStatus == Status.Running) && (mServerStatus == Status.PendingRestart))
            {
                ServerStatus = Status.Running;
            }
        }

        /// <summary>
        /// Initiate a backup of the server.
        /// </summary>
        public void Backup()
        {
            RaiseServerMessage("Starting backup...");
            BlockAutoSave();
            using (Backup backup = new Backup(this))
            {
                backup.PerformBackup();
            }
            UnblockAutoSave();
            RaiseServerMessage("Backup complete.");
        }

        /// <summary>
        /// Forcibly shut down the server by terminating the process.
        /// </summary>
        private void ForceShutdown()
        {
            try
            {
                if (mServerProcess != null)
                {
                    mServerProcess.Kill();
                }
            }
            catch (InvalidOperationException e)
            {
                // Task is probably already killed
            }
            finally
            {
                if (mServerProcess != null)
                {
                    mServerProcess.Dispose();
                }
                OnServerStopped("Server Killed");
            }
        }

        /// <summary>
        /// Forces a reload of the online-user list by issuing a server 'list' command.
        /// </summary>
        /// <remarks>
        /// Note that this method blocks until the server replies.  If a user list refresh
        /// is required but does not need to be guaranteed current, simply use a call of
        /// SendCommand("list") and the online user list will be up-to-date as soon as possible.
        /// </remarks>
        public bool RefreshOnlineUserList()
        {
            int waited = 0;
            bool result = true;
            mOnlineUserListReady = false;

            SendCommand("list");
            while (!mOnlineUserListReady)
            {
                if (waited > COMMAND_TIMEOUT_MS)
                {
                    result = false;
                    break;
                }
                waited += 100;
                Thread.Sleep(100);
            }
            return result;
        }

        /// <summary>
        /// Sends an arbitrary command to the Minecraft server.
        /// </summary>
        /// <param name="Command">Command to send</param>
        public void SendCommand(string Command)
        {
            if ((mServerStatus == Status.Running) || (mServerStatus == Status.PendingStop) || (mServerStatus == Status.PendingRestart))
            {
                mCommandInjector.WriteLine(Command);
            }
        }

        /// <summary>
        /// Generates all maps.
        /// </summary>
        public void GenerateMaps(string[] args)
        {
            BlockAutoSave();
            MapManager.RenderMaps(args);
            UnblockAutoSave();
        }

        /// <summary>
        /// Disables server auto-save by incrementing a 'block' counter. Autosaves are not
        /// resumed until all blocks have been released.  <see cref="UnblockAutoSave"/>
        /// </summary>
        private void BlockAutoSave()
        {
            mAutoSaveBlocks += 1;
            if ((mAutoSaveEnabled) && (mAutoSaveBlocks > 0))
            {
                SetAutoSave(false);
            }
        }

        /// <summary>
        /// Re-enables server auto-save by decrementing a 'block' counter. Autosaves are not
        /// resumed until all blocks have been released.  <see cref="BlockAutoSave"/>
        /// </summary>
        private void UnblockAutoSave()
        {
            mAutoSaveBlocks -= 1;
            if ((!mAutoSaveEnabled) && (mAutoSaveBlocks == 0))
            {
                SetAutoSave(true);
            }
        }

        /// <summary>
        /// Populates mSavedUsers with details taken from the World's 'players' directory.
        /// </summary>
        public void LoadSavedUserInfo()
        {
            mSavedUsers.Clear();
            if (Directory.Exists(Path.Combine(mServerProperties.WorldPath, "players")))
            {
                foreach (string fileName in Directory.GetFiles(Path.Combine(mServerProperties.WorldPath, "players")))
                {
                    SavedUser user = new SavedUser();
                    user.LoadData(fileName);
                    mSavedUsers.Add(user);
                }
            }
        }

        /// <summary>
        /// Called whenever the server issues a message.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="OutLine"></param>
        private void ServerOutputHandler(object sender, DataReceivedEventArgs OutLine)
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
                    OnServerError("Error starting server: port " + mServerProperties.ServerPort + " in use");
                    ServerStatus = Status.Failed;
                    mStatusMessage = M.Message;
                    ForceShutdown();
                    break;

                case EMMServerMessage.MessageTypes.HModBanner:
                    RaiseServerMessage("Hey0 hMod detected");
                    mServerRunningHMod = true;
                    int.TryParse(M.Data["version"], out mHModversion);
                    break;

                case EMMServerMessage.MessageTypes.SaveStarted:
                    mServerSaving = true;
                    break;

                case EMMServerMessage.MessageTypes.SaveComplete:
                    mServerSaving = false;
                    LoadSavedUserInfo();
                    break;

                case EMMServerMessage.MessageTypes.StartupComplete:
                    mOnlineUsers = new ArrayList();
                    OnServerStarted("Server started");
                    break;

                case EMMServerMessage.MessageTypes.UserLoggedIn:
                    mOnlineUsers.Add(M.Data["username"]);
                    break;

                case EMMServerMessage.MessageTypes.UserLoggedOut:
                    mOnlineUsers.Remove(M.Data["username"]);
                    if (mOnlineUsers.Count == 0)
                    {
                        OnServerReachZeroUsers();
                    }
                    break;

            }

            // send the output to the comms server
            if (mCommsServer.Listening)
            {
                mCommsServer.SendData(M.Message);
            }

            // raise an InfoMessage Event too
            RaiseServerMessage(M.Message);
        }

        private void SetOnlineUserList()
        {
            SetOnlineUserList("");
        }

        private void SetOnlineUserList(string userlist)
        {
            mOnlineUsers.Clear();
            if (userlist.Length > 0)
            {
                mOnlineUsers.AddRange(userlist.Split(','));
            }
            mOnlineUsers.Sort();
            mOnlineUserListReady = true;
        }

        #region Server Events

        internal void OnRemoteCommandReceived(string command)
        {
            mParser.ParseCommand(command);
        }

        /// <summary>
        /// Helper-method to raise ServerMessage Events from other places.
        /// </summary>
        /// <param name="Message">The message to throw</param>
        public void RaiseServerMessage(string Message)
        {
            if (ServerMessage != null)
            {
                ServerMessage(this, new ServerMessageEventArgs(Message));
            }
        }
        
        /// <summary>
        /// Called when the Minecraft server process terminates.
        /// </summary>
        /// <remarks>
        /// Don't put any logic in here, keep it in the standard onServerStopped event handler.</remarks>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void ServerExited(object sender, System.EventArgs args)
        {
            SetOnlineUserList();
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
            mServerProperties.Load();
            LoadSavedUserInfo();
            if (ServerStarted != null)
            {
                ServerStarted(this, new ServerMessageEventArgs(Message));
            }
            RaiseServerMessage("Started");
        }

        /// <summary>
        /// Called when the Minecraft server has stopped.
        /// </summary>
        /// <remarks>Raises event ServerStopped.</remarks>
        /// <param name="Message"></param>
        private void OnServerStopped(string Message)
        {
            ServerStatus = Status.Stopped;
            SetOnlineUserList();
            if (ServerStopped != null)
            {
                ServerStopped(this, new ServerMessageEventArgs(Message));
            }
            mServerProcess = null;
        }

        /// <summary>
        /// Called when the last user logs out.
        /// </summary>
        private void OnServerReachZeroUsers()
        {
            if (mServerStatus == Status.PendingRestart)
            {
                RestartServer();
            }
            if (mServerStatus == Status.PendingStop)
            {
                StopServer();
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
            StopServer(1, true);
            if (mServerProcess != null)
            {
                mServerProcess.Dispose();
            }
        }
    }
}

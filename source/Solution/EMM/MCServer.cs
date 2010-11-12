using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading;
using System.IO;
using System.Collections;

namespace EnigmaMM
{
    /// <summary>
    /// The main Server Manager class.
    /// Keeps track of the server listener, and manages the Minecraft process.
    /// </summary>
    public class MCServer
    {
        private const int COMMAND_TIMEOUT_MS = 5000;

        private Process mServerProcess;
        private Status mServerStatus;
        private string mStatusMessage;
        private bool mOnlineUserListReady = false;
        private CommsServer mCommsServer;
        private CommandParser mParser;
        private bool mServerSaving = false;


        // Java and Minecraft Server settings
        private System.IO.StreamWriter mCommandInjector;
        private MCServerProperties mServerProperties;
        private MCServerWarps mServerWarps;
        private string mJavaExec = "java.exe";
        private string mServerRoot = "";
        private string mServerJar = "minecraft_server.jar";
        private int mJavaHeapInit = 1024;
        private int mJavaHeapMax = 1024;
        private bool mServerRunningHMod = false;
        private int mHModversion = 0;
        private ArrayList mSavedUsers = new ArrayList();
        private ArrayList mOnlineUsers = new ArrayList();

        private int mAutoSaveBlocks = 0;
        private bool mAutoSaveEnabled = false;

        // Map objects and settings
        private AlphaVespucci mMapAlphaVespucci;
        private bool mAlphaVespucciInstalled = false;
        private Overviewer mMapOverviewer;
        private bool mOverviewerInstalled = false;
        private string mMapRoot;

        // Events raised at key server events
        public delegate void ServerMessageEventHandler(string Message);
        public event ServerMessageEventHandler ServerStopped;
        public event ServerMessageEventHandler ServerStarted;
        public event ServerMessageEventHandler ServerMessage;
        public event ServerMessageEventHandler ServerError;
        public event ServerMessageEventHandler StatusChanged;

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

        public string JavaExec
        {
            set { mJavaExec = value; }
        }
        public string ServerRoot
        {
            set { mServerRoot = value; }
        }
        public string ServerJar
        {
            set { mServerJar = value; }
        }
        public int JavaHeapInit
        {
            set { mJavaHeapInit = value; }
        }
        public int JavaHeapMax
        {
            set { mJavaHeapMax = value; }
        }
        public string MapRoot
        {
            set { mMapRoot = value; }
        }
        public bool AlphaVespucciInstalled
        {
            set { mAlphaVespucciInstalled = value; }
        }
        public bool OverviewerInstalled
        {
            set { mOverviewerInstalled = value; }
        }

        public int OnlineUserCount
        {
            get { return mOnlineUsers.Count; }
        }

        public ArrayList OnlineUserList
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
                    StatusChanged("");
                }
            }
        }


        /// <summary>
        /// Server Constructor
        /// </summary>
        public MCServer()
        {
            Settings.Initialise(Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "settings.conf"));

            mServerProperties = new MCServerProperties();
            mCommsServer = new CommsServer();
            mParser = new CommandParser(this);
            mMapAlphaVespucci = new AlphaVespucci(this);
            mMapOverviewer = new Overviewer(this);

            ServerStatus = Status.Stopped;
            mServerRoot = Settings.MinecraftRoot;
            mJavaExec = Settings.JavaExec;
            mServerJar = Settings.ServerJar;
            mJavaHeapInit = Settings.JavaHeapInit;
            mJavaHeapMax = Settings.JavaHeapMax;
            mMapRoot = Settings.MapRoot;

            AlphaVespucciInstalled = Settings.AlphaVespucciInstalled;
            OverviewerInstalled = Settings.OverviewerInstalled;

            // See if we need to swap in a new config file, and load current config.
            ReloadConfig();

            // Determine if the communications listener should be started.
            StartCommsServer();
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
                ServerMessage("Server already running, cannot start!");
                return;
            }

            ReloadConfig();

            string cmdArgs = "";
            if (mJavaHeapInit > 0)
            {
                cmdArgs += "-Xms" + mJavaHeapInit + "M ";
            }
            if (mJavaHeapMax > 0)
            {
                cmdArgs += "-Xmx" + mJavaHeapMax + "M ";
            }
            cmdArgs += "-jar \"" + mServerJar + "\" ";
            cmdArgs += "nogui ";

            // Configure the main server process
            if (Directory.Exists(mServerRoot) == false)
            {
                throw new FileNotFoundException("Could not fine Minecraft root: " + mServerRoot);
            }
            if (File.Exists(Path.Combine(mServerRoot, mServerJar)) == false)
            {
                throw new FileNotFoundException("Could not fine Minecraft server jar: " + Path.Combine(mServerRoot, mServerJar));
            }
            mServerProcess = new Process();
            mServerProcess.StartInfo.WorkingDirectory = mServerRoot;
            mServerProcess.StartInfo.FileName = mJavaExec;
            mServerProcess.StartInfo.Arguments = cmdArgs;
            mServerProcess.StartInfo.UseShellExecute = false;
            mServerProcess.StartInfo.CreateNoWindow = true;
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
            ServerStatus = Status.Starting;
            SetOnlineUserList();
            mServerProcess.Start();

            // Wire up the writer to send messages to the process
            mCommandInjector = mServerProcess.StandardInput;
            mCommandInjector.AutoFlush = true;

            // Start listening for output
            mServerProcess.BeginOutputReadLine();
            mServerProcess.BeginErrorReadLine();

            ServerMessage("Server starting...");
        }


        public void StopServer()
        {
            StopServer(0, false);
        }

        public void StopServer(int timeout)
        {
            StopServer(timeout, false);
        }

        /// <summary>
        /// Shuts down the running Server.
        /// </summary>
        /// <param name="timeout">
        /// Time in milliseconds to wait for the command to complete.
        /// Set to zero to return immediately, thus essentially running the command asynchronously.
        /// </param>
        /// <param name="force">
        /// If set to true, and a timeout is specified, if the server is still running after 
        /// the timeout it will be forcefully terminated.
        /// </param>
        public void StopServer(int timeout, bool force)
        {
            bool forceable = (timeout > 0);

            if (mServerStatus == Status.Running)
            {
                ServerStatus = Status.Stopping;
                SendCommand("stop");
                while ((timeout > 0) && (mServerStatus != Status.Stopped))
                {
                    timeout -= 100;
                    Thread.Sleep(100);
                }
                if ((forceable) && (mServerStatus != Status.Stopped))
                {
                    ForceShutdown();
                }
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
            StopServer();
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
        private void AutoSave(bool enabled)
        {
            if (enabled)
            {
                SendCommand("save-on");
            }
            else
            {
                SendCommand("save-off");
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


        public void Backup()
        {
            ServerMessage("Starting backup...");
            BlockAutoSave();
            using (Backup backup = new Backup(this))
            {
                backup.PerformBackup();
            }
            UnblockAutoSave();
            ServerMessage("Backup complete.");
        }


        private void ForceShutdown()
        {
            mServerProcess.Kill();
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


        public void SendCommand(string Command)
        {
            if (mServerStatus != Status.Stopped)
            {
                mCommandInjector.WriteLine(Command);
            }
        }


        public void GenerateMapAV()
        {
            if (!mAlphaVespucciInstalled)
            {
                ServerMessage("Skipping AlphaVespucci Maps, not installed.");
            }
            else
            {
                ServerMessage("Generating AlphaVespucci Maps...");
                BlockAutoSave();
                mMapAlphaVespucci.RenderMap("obleft", "day", "mainmap", true);
                UnblockAutoSave();
                ServerMessage("Done.");
            }
        }


        public void GenerateMapAVExtra()
        {
            if (!mAlphaVespucciInstalled)
            {
                ServerMessage("Skipping more AlphaVespucci Maps, not installed.");
            }
            else
            {
                ServerMessage("Generating more AlphaVespucci Maps...");
                BlockAutoSave();
                mMapAlphaVespucci.RenderMap("obleft", "night", "nightmap");
                mMapAlphaVespucci.RenderMap("obleft", "cave", "caves");
                mMapAlphaVespucci.RenderMap("obleft", "cavelimit 15", "surfacecaves");
                mMapAlphaVespucci.RenderMap("obleft", "whitelist \"Diamond ore\"", "resource-diamond");
                mMapAlphaVespucci.RenderMap("obleft", "whitelist \"Redstone ore\"", "resource-redstone");
                mMapAlphaVespucci.RenderMap("obleft", "night -whitelist \"Torch\"", "resource-torch");
                mMapAlphaVespucci.RenderMap("flat", "day", "flatmap");
                UnblockAutoSave();
                ServerMessage("Done.");
            }
        }


        public void GenerateMapOverviewer()
        {
            if (!mOverviewerInstalled)
            {
                ServerMessage("Skipping Overviewer Map, not installed.");
            }
            else
            {
                ServerMessage("Generating Overviewer Map...");
                BlockAutoSave();
                mMapOverviewer.RenderMap();
                UnblockAutoSave();
                ServerMessage("Done.");
            }
        }

        
        public void GenerateMaps()
        {
            BlockAutoSave();
            GenerateMapOverviewer();
            GenerateMapAV();
            GenerateMapAVExtra();
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
                AutoSave(false);
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
                AutoSave(true);
            }
        }


        /// <summary>
        /// Populates mSavedUsers with details taken from the World's 'players' directory.
        /// </summary>
        public void LoadSavedUserInfo()
        {
            mSavedUsers.Clear();
            foreach (string fileName in Directory.GetFiles(Path.Combine(mServerProperties.WorldPath, "players")))
            {
                SavedUser user = new SavedUser();
                user.LoadData(fileName);
                mSavedUsers.Add(user);
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

            MCServerMessage M = new MCServerMessage(OutLine.Data);

            switch (M.Type)
            {
                case MCServerMessage.MessageType.AutoSaveEnabled:
                    mAutoSaveEnabled = true;
                    break;

                case MCServerMessage.MessageType.AutoSaveDisabled:
                    mAutoSaveEnabled = false;
                    break;

                case MCServerMessage.MessageType.ErrorPortBusy:
                    OnServerError("Error starting server: port " + mServerProperties.ServerPort + " in use");
                    ServerStatus = Status.Failed;
                    mStatusMessage = M.Message;
                    ForceShutdown();
                    break;

                case MCServerMessage.MessageType.HModBanner:
                    ServerMessage("Hey0 hMod detected");
                    mServerRunningHMod = true;
                    int.TryParse(M.Data, out mHModversion);
                    break;

                case MCServerMessage.MessageType.SaveStarted:
                    mServerSaving = true;
                    break;

                case MCServerMessage.MessageType.SaveComplete:
                    mServerSaving = false;
                    LoadSavedUserInfo();
                    break;

                case MCServerMessage.MessageType.StartupComplete:
                    mOnlineUsers = new ArrayList();
                    OnServerStarted("Server started");
                    break;

                case MCServerMessage.MessageType.UserCount:
                    if ((mOnlineUsers.Count == 0) && (mServerStatus == Status.PendingRestart))
                    {
                        RestartServer();
                    }
                    if ((mOnlineUsers.Count == 0) && (mServerStatus == Status.PendingStop))
                    {
                        StopServer();
                    }
                    break;

                case MCServerMessage.MessageType.UserList:
                    SetOnlineUserList(M.Data);
                    break;

                case MCServerMessage.MessageType.UserLoggedIn:

                    break;
            }

            // send the output to the comms server
            if (mCommsServer.Listening)
            {
                mCommsServer.SendData(M.Message);
            }

            // raise an InfoMessage Event too
            if (ServerMessage != null)
            {
                ServerMessage(M.Message);
            }
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
        internal void RaiseServerMessage(string Message)
        {
            if (ServerMessage != null)
            {
                ServerMessage(Message);
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
                ServerStarted(Message);
            }
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
                ServerStopped(Message);
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
                ServerError(Message);
            }
        }

        #endregion

    }
}

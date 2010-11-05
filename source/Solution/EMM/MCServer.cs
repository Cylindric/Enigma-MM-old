using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading;
using System.IO;
using System.Collections;

namespace EnigmaMM
{
    public class MCServer
    {
        private Process mServerProcess;
        private Status mServerStatus;
        private string mStatusMessage;
        private bool mOnlineUserListReady = false;
        private string mOnlineUserList = "";
        private int mUsersOnline = 0;

        // Java and Minecraft Server settings
        private MCServerProperties mServerProperties = new MCServerProperties();
        private MCServerWarps mServerWarps;
        private string mJavaExec = "java.exe";
        private string mServerRoot = "";
        private string mServerJar = "minecraft_server.jar";
        private int mJavaHeapInit = 1024;
        private int mJavaHeapMax = 1024;
        private bool mServerRunningHey0 = false;
        private int mHey0version = 0;
        private ArrayList mSavedUsers = new ArrayList();

        private int mAutoSaveBlocks = 0;
        private bool mAutoSaveEnabled = false;

        // Map objects and settings
        private AlphaVespucci mMapAlphaVespucci;
        private bool mAlphaVespucciInstalled = false;
        private Overviewer mMapOverviewer;
        private bool mOverviewerInstalled = false;
        private string mMapRoot;

        private System.IO.StreamWriter ioWriter;

        // Events raised at key Minecraft events
        public delegate void ServerMessageEventHandler(string Message);
        public event ServerMessageEventHandler ServerStopped;
        public event ServerMessageEventHandler ServerStarted;
        public event ServerMessageEventHandler ServerMessage;
        public event ServerMessageEventHandler ServerError;

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

        /// <summary>
        /// Server Constructor
        /// </summary>
        public MCServer()
        {
            mServerStatus = Status.Stopped;
            mMapAlphaVespucci = new AlphaVespucci(this);
            mMapOverviewer = new Overviewer(this);
        }



        public void ReloadConfig()
        {
            // See if we need to swap in a new config file
            mServerProperties.LookForNewSettings();
            if (mServerProperties.WarpLocation != "")
            {
                mServerWarps = new MCServerWarps(mServerProperties.WarpLocation);
                mServerWarps.LookForNewSettings();
            }
        }

        /// <summary>
        /// Helper-method to raise ServerMessage Events from other places.
        /// </summary>
        /// <param name="Message">The message to throw</param>
        internal void RaiseServerMessage(string Message)
        {
            ServerMessage(Message);
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
            mServerProcess.StartInfo.CreateNoWindow = false;
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
            mServerStatus = Status.Starting;
            mServerProcess.Start();

            // Wire up the writer to send messages to the process
            ioWriter = mServerProcess.StandardInput;
            ioWriter.AutoFlush = true;

            // Start listening for output
            mServerProcess.BeginOutputReadLine();
            mServerProcess.BeginErrorReadLine();

            ServerMessage("Server starting...");
        }


        /// <summary>
        /// Shuts down the running Server.
        /// </summary>
        public void StopServer()
        {
            if (mServerStatus == Status.Running)
            {
                SendCommand("stop");
                while (mServerStatus != Status.Stopped)
                {
                    Thread.Sleep(100);
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
            if (mUsersOnline == 0)
            {
                StopServer();
            }
            else
            {
                mServerStatus = Status.PendingStop;
            }
        }


        /// <summary>
        /// Aborts a pending stop operation.
        /// </summary>
        public void AbortPendingStop()
        {
            if ((mServerStatus == Status.Running) && (mServerStatus == Status.PendingStop))
            {
                mServerStatus = Status.Running;
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
            if (mUsersOnline == 0)
            {
                RestartServer();
            }
            else
            {
                mServerStatus = Status.PendingRestart;
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
                mServerStatus = Status.Running;
            }
        }



        public void Backup()
        {
            Backup backup = new Backup(this);
            backup.PerformBackup();
        }



        private void ForceShutdown()
        {
            mServerProcess.Kill();
        }



        public string OnlineUsers()
        {
            mOnlineUserListReady = false;
            SendCommand("list");
            while (!(mOnlineUserListReady))
            {
                Thread.Sleep(100);
            }
            return mOnlineUserList;
        }



        public void SendCommand(string Command)
        {
            if (mServerStatus == Status.Running)
            {
                ioWriter.WriteLine(Command);
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



        private void BlockAutoSave()
        {
            mAutoSaveBlocks += 1;
            if ((mAutoSaveEnabled) && (mAutoSaveBlocks > 0))
            {
                AutoSave(false);
            }
        }

        private void UnblockAutoSave()
        {
            mAutoSaveBlocks -= 1;
            if ((!mAutoSaveEnabled) && (mAutoSaveBlocks == 0))
            {
                AutoSave(true);
            }
        }

        public void LoadSavedUserInfo()
        {
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
            string T = null;
            if ((OutLine.Data != null))
            {
                T = OutLine.Data;
                MCServerMessage M = new MCServerMessage(T);

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
                        mServerStatus = Status.Failed;
                        mStatusMessage = T;
                        ForceShutdown();
                        break;

                    case MCServerMessage.MessageType.Hey0Banner:
                        ServerMessage("Hey0 mod detected");
                        mServerRunningHey0 = true;
                        int.TryParse(M.Data, out mHey0version);
                        AutoSave(true);
                        break;

                    case MCServerMessage.MessageType.SaveComplete:
                        LoadSavedUserInfo();
                        break;

                    case MCServerMessage.MessageType.StartupComplete:
                        OnServerStarted("Server started");
                        break;

                    case MCServerMessage.MessageType.UserCount:
                        mUsersOnline = 0;
                        int.TryParse(M.Data, out mUsersOnline);
                        if ((mUsersOnline == 0) && (mServerStatus == Status.PendingRestart))
                        {
                            RestartServer();
                        }
                        if ((mUsersOnline == 0) && (mServerStatus == Status.PendingStop))
                        {
                            StopServer();
                        }
                        break;

                    case MCServerMessage.MessageType.UserList:
                        mOnlineUserList = M.Data;
                        mOnlineUserListReady = true;
                        break;

                }

                // raise an InfoMessage Event too
                if (ServerMessage != null)
                {
                    ServerMessage(OutLine.Data);
                }
            }
        }



        /// <summary>
        /// Called when the server process terminates.
        /// </summary>
        /// <remarks>
        /// Don't put any logic in here, keep it in the standard onServerStopped event handler.</remarks>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void ServerExited(object sender, System.EventArgs args)
        {
            OnServerStopped("Server Stopped");
        }

        protected virtual void OnServerStarted(string Message)
        {
            mServerStatus = Status.Running;
            mServerProperties.Load();
            LoadSavedUserInfo();
            if (ServerStarted != null)
            {
                ServerStarted(Message);
            }
        }

        protected virtual void OnServerStopped(string Message)
        {
            mServerStatus = Status.Stopped;
            if (ServerStopped != null)
            {
                ServerStopped(Message);
            }
        }

        protected virtual void OnServerError(string Message)
        {
            if (ServerError != null)
            {
                ServerError(Message);
            }
        }

    }
}

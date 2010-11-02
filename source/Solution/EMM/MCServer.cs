﻿using System;
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

            ServerMessage("Server started.");
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


        private void AutoSave(bool Enabled)
        {
            if (mServerRunningHey0)
            {
                if (Enabled)
                {
                    SendCommand("save-on");
                }
                else
                {
                    SendCommand("save-off");
                }
                Thread.Sleep(1000);
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
            if (mAlphaVespucciInstalled)
            {
                AutoSave(false);
                ServerMessage("Generating AlphaVespucci Maps...");
                mMapAlphaVespucci.RenderMaps("obleft", "day", "mainmap", true);
                ServerMessage("Done.");
                AutoSave(true);
            }
        }


        public void GenerateMapAVExtra()
        {
            ServerMessage("Generating more AlphaVespucci Maps...");
            mMapAlphaVespucci.RenderMaps("obleft", "night", "nightmap");
            mMapAlphaVespucci.RenderMaps("obleft", "cave", "caves");
            mMapAlphaVespucci.RenderMaps("obleft", "cavelimit 15", "surfacecaves");
            mMapAlphaVespucci.RenderMaps("obleft", "whitelist \"Diamond ore\"", "resource-diamond");
            mMapAlphaVespucci.RenderMaps("obleft", "whitelist \"Redstone ore\"", "resource-redstone");
            mMapAlphaVespucci.RenderMaps("obleft", "night -whitelist \"Torch\"", "resource-torch");
            mMapAlphaVespucci.RenderMaps("flat", "day", "flatmap");
            ServerMessage("Done.");
        }


        public void GenerateMapOverviewer()
        {
            if (mOverviewerInstalled)
            {
                AutoSave(false);
                mMapOverviewer.RenderMaps();
                AutoSave(true);
            }
        }

        
        public void GenerateMaps()
        {
            GenerateMapOverviewer();
            GenerateMapAV();
            GenerateMapAVExtra();
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

                if (MsgIsUserCount(T))
                {
                    mUsersOnline = ExtractPlayerCount(T);
                    if ((mUsersOnline == 0) && (mServerStatus == Status.PendingRestart))
                    {
                        RestartServer();
                    }
                    if ((mUsersOnline == 0) && (mServerStatus == Status.PendingStop))
                    {
                        StopServer();
                    }
                }
                else if (MsgIsSaveComplete(T))
                {
                    LoadSavedUserInfo();
                }
                else if (MsgIsUserList(T))
                {
                    ExtractUsers(T);
                    mOnlineUserListReady = true;
                }
                else if (MsgIsServerStarted(T))
                {
                    OnServerStarted("Server started");
                }
                else if (MsgIsHey0(T))
                {
                    ServerMessage("Hey0 mod detected");
                    mServerRunningHey0 = true;
                    mHey0version = ExtractHey0Version(T);
                }
                else if (MsgIsServerErrPortBusy(T))
                {
                    OnServerError("Error starting server: port " + mServerProperties.ServerPort + " in use");
                    mServerStatus = Status.Failed;
                    mStatusMessage = T;
                    ForceShutdown();
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
            mServerProperties.LoadServerProperties();
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

        private bool MsgIsServerStarted(string msg)
        {
            string regex = @"^(?<timestamp>.+?)\[INFO]\ Done!.*?$";
            return Regex.IsMatch(msg, regex);
        }

        private bool MsgIsServerErrPortBusy(string msg)
        {
            string regex = @"^(?<timestamp>.+?)\[WARNING]\ \*\*\*\*\ FAILED\ TO\ BIND\ TO\ PORT!.*?$";
            return Regex.IsMatch(msg, regex);
        }

        private bool MsgIsUserList(string msg)
        {
            string regex = @"^(?<timestamp>.+?)\[INFO]\ Connected\ players:\ .*?$";
            return Regex.IsMatch(msg, regex);
        }

        private bool MsgIsUserCount(string msg)
        {
            string regex = @"^Player\ count:\ (?<count>\d+)$";
            return Regex.IsMatch(msg, regex);
        }

        private bool MsgIsSaveComplete(string msg)
        {
            string regex = @"^(?<timestamp>.+?)\[INFO]\ CONSOLE:\ Save\ Complete\.$";
            return Regex.IsMatch(msg, regex);
        }

        private bool MsgIsHey0(string msg)
        {
            string regex = @"^(?<timestamp>.+?)\[INFO]\ Hey0\ Server\ Mod\ Build\ .*?$";
            return Regex.IsMatch(msg, regex);
        }

        private int ExtractPlayerCount(string msg)
        {
            int count = 0;
            string regex = @"^Player\ count:\ (?<count>\d+)$";
            MatchCollection matches = Regex.Matches(msg, regex);
            if (matches.Count > 0)
            {
                int.TryParse(matches[0].Groups["count"].Value, out count);
            }
            return count;
        }

        private int ExtractHey0Version(string msg)
        {
            int version = 0;
            string regex = @"^(?<timestamp>.+?)\[INFO]\ Hey0\ Server\ Mod\ Build\ (?<version>.*?)$";
            MatchCollection matches = Regex.Matches(msg, regex);
            if (matches.Count > 0)
            {
                int.TryParse(matches[0].Groups["version"].Value, out version);
            }
            return version;
        }

        private string ExtractUsers(string msg)
        {
            string regex = @"^(?<timestamp>.+?)\ \[INFO]\ Connected\ players:\ (?<userlist>.*?)$";
            MatchCollection matches = Regex.Matches(msg, regex);
            if (matches.Count > 0)
            {
                return matches[0].Groups["userlist"].Value;
            }
            else
            {
                return "";
            }
        }

    }


}

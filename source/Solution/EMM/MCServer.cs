using System;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Threading;

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
        private MCServerProperties mServerProperties = new MCServerProperties();

        private string mJavaExec = "java.exe";
        private string mServerRoot = "";
        private string mServerJar = "minecraft_server.jar";
        private int mJavaHeapInit = 1024;
        private int mJavaHeapMax = 1024;

        private System.IO.StreamWriter ioWriter;

        public event ServerMessageEventHandler ServerMessage;
        public delegate void ServerMessageEventHandler(string Message);

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



        /// <summary>
        /// Server Constructor
        /// </summary>
        public MCServer()
        {
            mServerStatus = Status.Stopped;
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

            string cmdArgs = null;
            if (mJavaHeapInit > 0)
            {
                cmdArgs += "-Xms" + mJavaHeapInit + "M ";
            }
            if (mJavaHeapMax > 0)
            {
                cmdArgs += "-Xmx" + mJavaHeapMax + "M ";
            }
            cmdArgs = "-jar \"" + mServerJar + "\" ";
            cmdArgs = cmdArgs + "nogui ";

            // Configure the main server process
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
                else if (MsgIsUserList(T))
                {
                    ExtractUsers(T);
                    mOnlineUserListReady = true;
                }
                else if (MsgIsServerStarted(T))
                {
                    mServerStatus = Status.Running;
                }
                else if (MsgIsServerErrPortBusy(T))
                {
                    mServerStatus = Status.Failed;
                    mStatusMessage = T;
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
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void ServerExited(object sender, System.EventArgs args)
        {
            mServerStatus = Status.Stopped;
            ServerMessage("Server exited");
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

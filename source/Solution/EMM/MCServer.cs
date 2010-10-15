using System.Diagnostics;
using System.Configuration;
using System;

namespace EnigmaMM
{
    public class MCServer
    {

        private Process m_ServerProcess;
        private Status m_ServerStatus;
        private string m_StatusMessage;
        private bool m_OnlineUserListReady = false;
        private string m_OnlineUserList = "";

        private string m_JavaExec = "java.exe";
        private string m_ServerRoot = "";
        private string m_ServerJar = "minecraft_server.jar";
        private int m_JavaHeapInit = 1024;
        private int m_JavaHeapMax = 1024;

        private System.IO.StreamWriter ioWriter;

        public event InfoMessageEventHandler InfoMessage;
        public delegate void InfoMessageEventHandler(string Message);

        public event LogMessageEventHandler LogMessage;
        public delegate void LogMessageEventHandler(string Message);

        public enum Status
        {
            Starting,
            Running,
            Busy,
            Stopping,
            Stopped,
            Failed
        }

        public Status CurrentStatus
        {
            get { return m_ServerStatus; }
        }

        public string LastStatusMessage
        {
            get { return m_StatusMessage; }
        }

        public string ServerRoot
        {
            set { m_ServerRoot = value; }
        }
        public string ServerJar
        {
            set { m_ServerJar = value; }
        }
        public int JavaHeapInit
        {
            set { m_JavaHeapInit = value; }
        }
        public int JavaHeapMax
        {
            set { m_JavaHeapMax = value; }
        }


        /// <summary>
        /// Server Constructor
        /// </summary>
        public MCServer()
        {
            m_ServerStatus = Status.Stopped;
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
            
            string cmdArgs = null;
            if (m_JavaHeapInit > 0)
            {
                cmdArgs += "-Xms" + m_JavaHeapInit + "M ";
            }
            if (m_JavaHeapMax > 0)
            {
                cmdArgs += "-Xmx" + m_JavaHeapMax + "M ";
            }
            cmdArgs = "-jar \"" + m_ServerJar + "\" ";
            cmdArgs = cmdArgs + "nogui ";

            // Configure the main server process
            m_ServerProcess = new Process();
            m_ServerProcess.StartInfo.WorkingDirectory = m_ServerRoot;
            m_ServerProcess.StartInfo.FileName = m_JavaExec;
            m_ServerProcess.StartInfo.Arguments = cmdArgs;
            m_ServerProcess.StartInfo.UseShellExecute = false;
            m_ServerProcess.StartInfo.CreateNoWindow = false;
            m_ServerProcess.StartInfo.RedirectStandardError = true;
            m_ServerProcess.StartInfo.RedirectStandardInput = true;
            m_ServerProcess.StartInfo.RedirectStandardOutput = true;
            m_ServerProcess.EnableRaisingEvents = true;

            // Wire up an event handler to catch messages out of the process
            m_ServerProcess.OutputDataReceived += new DataReceivedEventHandler(LogOutputHandler);
            m_ServerProcess.ErrorDataReceived += new DataReceivedEventHandler(InfoOutputHandler);
            m_ServerProcess.Exited += new EventHandler(ServerExited);

            // Start the server process
            m_ServerStatus = Status.Starting;
            m_ServerProcess.Start();

            // Wire up the writer to send messages to the process
            ioWriter = m_ServerProcess.StandardInput;
            ioWriter.AutoFlush = true;

            // Start listening for output
            m_ServerProcess.BeginOutputReadLine();
            m_ServerProcess.BeginErrorReadLine();

        }

        public void Shutdown()
        {
            SendCommand("stop");
        }

        private void ForceShutdown()
        {
            m_ServerProcess.Kill();
        }

        public string OnlineUsers()
        {
            m_OnlineUserListReady = false;
            SendCommand("list");
            while (!(m_OnlineUserListReady))
            {
                System.Threading.Thread.Sleep(100);
            }
            return m_OnlineUserList;
        }

        public void SendCommand(string Command)
        {
            if (m_ServerStatus == Status.Running)
            {
                ioWriter.WriteLine(Command);
            }
        }


        /// <summary>
        /// Called whenever the server issues a "LOG" message.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="OutLine"></param>
        private void LogOutputHandler(object sender, DataReceivedEventArgs OutLine)
        {
            if ((OutLine.Data != null))
            {
                if (LogMessage != null)
                {
                    LogMessage(OutLine.Data);
                }
            }
        }


        /// <summary>
        /// Called whenever the server issues an "INFO" message.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="OutLine"></param>
        private void InfoOutputHandler(object sender, DataReceivedEventArgs OutLine)
        {
            string T = null;
            if ((OutLine.Data != null))
            {
                T = OutLine.Data;

                if (T.Contains(Properties.Settings.Default.SrvReady))
                {
                    m_ServerStatus = Status.Running;

                }
                else if (T.Contains(Properties.Settings.Default.SrvPortBusy))
                {
                    m_ServerStatus = Status.Failed;
                    m_StatusMessage = T;

                }
                else if (T.Contains(Properties.Settings.Default.SrvUsers))
                {
                    m_OnlineUserList = T.Substring(T.IndexOf(Properties.Settings.Default.SrvUsers) + Properties.Settings.Default.SrvUsers.Length);
                    m_OnlineUserListReady = true;

                }

                if (InfoMessage != null)
                {
                    InfoMessage(OutLine.Data);
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
            m_ServerStatus = Status.Stopped;
        }

    
    }


}

using System;
using System.Threading;
using EnigmaMM.Interfaces;
using System.IO;
using System.Diagnostics;

namespace EnigmaMM
{
    class PowerManager
    {
        private EMMServer mServer;

        internal PowerManager(EMMServer server)
        {
            mServer = server;
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
            if (mServer.ServerStatus == Status.Running)
            {
                mServer.RaiseServerMessage("Server already running, cannot start!");
                return;
            }

            mServer.ServerStatus = Status.Starting;
            mServer.MinecraftSettings.LookForNewSettings();

            if (Directory.Exists(mServer.Settings.MinecraftRoot) == false)
            {
                mServer.RaiseServerMessage("ERROR");
                mServer.RaiseServerMessage("Could not find Minecraft root directory");
                mServer.RaiseServerMessage("Check that configuration option 'MinecraftRoot' is correct");
                mServer.RaiseServerMessage("Looking for: " + mServer.Settings.MinecraftRoot);
                mServer.ServerStatus = Status.Failed;
                return;
            }
            if (File.Exists(Path.Combine(mServer.Settings.MinecraftRoot, mServer.Settings.ServerJar)) == false)
            {
                mServer.RaiseServerMessage("ERROR");
                mServer.RaiseServerMessage("Could not find the Minecraft server file");
                mServer.RaiseServerMessage("Check that configuration option 'ServerJar' is correct");
                mServer.RaiseServerMessage("Looking for: " + Path.Combine(mServer.Settings.MinecraftRoot, mServer.Settings.ServerJar));
                mServer.ServerStatus = Status.Failed;
                return;
            }

            string cmdArgs = "";
            if (mServer.Settings.JavaHeapInit > 0)
            {
                cmdArgs += "-Xms" + mServer.Settings.JavaHeapInit + "M ";
            }
            if (mServer.Settings.JavaHeapMax > 0)
            {
                cmdArgs += "-Xmx" + mServer.Settings.JavaHeapMax + "M ";
            }
            cmdArgs += "-jar \"" + mServer.Settings.ServerJar + "\" ";
            cmdArgs += "nogui ";

            // Configure the main server process
            mServer.ServerProcess = new Process();
            if (mServer.Settings.ServerJar.EndsWith(".exe"))
            {
                mServer.ServerProcess.StartInfo.FileName = Path.Combine(mServer.Settings.MinecraftRoot, mServer.Settings.ServerJar);
            }
            else
            {
                mServer.ServerProcess.StartInfo.FileName = mServer.Settings.JavaExec;
            }
            mServer.ServerProcess.StartInfo.CreateNoWindow = true;
            mServer.ServerProcess.StartInfo.WorkingDirectory = mServer.Settings.MinecraftRoot;
            mServer.ServerProcess.StartInfo.Arguments = cmdArgs;
            mServer.ServerProcess.StartInfo.UseShellExecute = false;
            mServer.ServerProcess.StartInfo.RedirectStandardError = true;
            mServer.ServerProcess.StartInfo.RedirectStandardInput = true;
            mServer.ServerProcess.StartInfo.RedirectStandardOutput = true;
            mServer.ServerProcess.EnableRaisingEvents = true;

            // Wire up an event handler to catch messages out of the process
            // Minecraft uses a mix of standard output and error output, with important messages 
            // on both.  Therefore, we just wire up both to a single handler.
            // The messages seen in the Minecraft GUI are the ones from the ErrorData stream, the
            // additional ones only seen from the console are OutputData.
            mServer.ServerProcess.OutputDataReceived += new DataReceivedEventHandler(mServer.ServerOutputHandler);
            mServer.ServerProcess.ErrorDataReceived += new DataReceivedEventHandler(mServer.ServerOutputHandler);
            mServer.ServerProcess.Exited += new EventHandler(mServer.ServerExited);

            // Start the server process
            mServer.ServerProcess.Start();

            // Wire up the writer to send messages to the process
            mServer.CommandInjector = mServer.ServerProcess.StandardInput;
            mServer.CommandInjector.AutoFlush = true;

            // Start listening for output
            mServer.ServerProcess.BeginOutputReadLine();
            mServer.ServerProcess.BeginErrorReadLine();

            mServer.RaiseServerMessage("Server starting...");
        }

        /// <summary>
        /// Shuts down the running Server.
        /// </summary>
        /// <param name="graceful">If true, this will put the server in the 
        /// "pending shutdown" state, whereby it waits until all users have
        /// logged out, then shuts down the server.</param>
        /// <param name="timeout">Time in milliseconds to wait for the command
        /// to complete.  Set to zero to wait forever, or -1 to return 
        /// immediately, thus essentially running the command asynchronously.
        /// </param>
        /// <param name="force">If set to true, if the server is still running
        /// after the timeout it will be forcefully terminated.</param>
        internal void StopServer(bool graceful, int timeout, bool force)
        {
            bool neverTimeout = (timeout == 0);

            if ((graceful) && (mServer.Users.Count > 0))
            {
                mServer.ServerStatus = Status.PendingStop;
                return;
            }

            if ((mServer.ServerStatus == Status.Running) || (mServer.ServerStatus == Status.PendingStop) || (mServer.ServerStatus == Status.PendingRestart))
            {
                mServer.SendCommand("stop");
                mServer.ServerStatus = Status.Stopping;
                while (((timeout > 0) || (neverTimeout)) && (mServer.ServerStatus != Status.Stopped))
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
        /// <param name="graceful">If true, this will put the server in the
        /// "pending restart" state, whereby it waits until all users have 
        /// logged out, then restarts the server.</param>
        internal void RestartServer(bool graceful)
        {
            if ((graceful == true) && (mServer.Users.Count > 0))
            {
                mServer.ServerStatus = Status.PendingRestart;
                return;
            }

            StopServer(false, 0, false);
            StartServer();
        }

        /// <summary>
        /// Forcibly shut down the server by terminating the process.
        /// </summary>
        internal void ForceShutdown()
        {
            try
            {
                if (mServer.ServerProcess != null)
                {
                    mServer.ServerProcess.Kill();
                }
            }
            catch (InvalidOperationException)
            {
                // Task is probably already killed
            }
            finally
            {
                if (mServer.ServerProcess != null)
                {
                    mServer.ServerProcess.Dispose();
                }
                mServer.OnServerStopped("Server Killed");
            }
        }

    }
}

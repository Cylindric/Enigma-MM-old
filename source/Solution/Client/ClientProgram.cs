using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;

namespace EnigmaMM
{
    class ClientProgram
    {
        private static CLIHelper mCLI = new CLIHelper();
        private static Client mClient = new Client();
 
        static void Main(string[] args)
        {
            Settings.Initialise(Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "settings.conf"));
            
            bool StartCLI = true;

            mClient.MessageReceived += HandleMessageReceived;
            mClient.ServerIP = Settings.ServerIp;
            mClient.ServerPort = Settings.ServerPort;
            mClient.Username = Settings.ServerUsername;
            mClient.Password = Settings.ServerPassword;
            mClient.StartClient();

            // If any commands were passed on the command-line, execute them and then quit
            if (args.Length > 0)
            {
                StartCLI = false;
                foreach (string arg in args)
                {
                    HandleCommand(arg);
                }
            }

            // Start a new CLI helper thread to catch user input
            // After this we might start getting user commands through HandleCommands
            if (StartCLI)
            {
                mCLI.RaiseCommandReceivedEvent += HandleCommand;
                Thread CLIThread = new Thread(new ThreadStart(mCLI.StartListening));
                CLIThread.Start();
                // wait for the CLI thread to start
                while (!CLIThread.IsAlive)
                {
                    Thread.Sleep(1);
                }
            }
        }

        
        /// <summary>
        /// Handle a command from the CLI.
        /// </summary>
        /// <param name="Sender"></param>
        /// <param name="e"></param>
        private static void HandleCommand(string Command)
        {
            if (Command == "quit")
            {
                // intercept quit command
                mCLI.StopListening();
                // TODO: move this to the server end as well
            }
            else
            {
                mClient.SendData(Command);
            }
        }


        private static void HandleMessageReceived(string Message)
        {
            mCLI.WriteLine(Message);
        }
    }
}

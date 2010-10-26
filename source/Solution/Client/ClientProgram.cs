using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace EnigmaMM
{
    class ClientProgram
    {
        private static CLIHelper mCLI;
        private static Client mClient;

        static void Main(string[] args)
        {
            mClient = new Client();
            mClient.MessageReceived += HandleMessageReceived;
            mClient.StartClient();

            // Start a new CLI helper thread to catch user input
            // After this we might start getting user commands through HandleCommands
            mCLI = new CLIHelper();
            mCLI.RaiseCommandReceivedEvent += HandleCommand;
            Thread CLIThread = new Thread(new ThreadStart(mCLI.StartListening));
            CLIThread.Start();
            while (!CLIThread.IsAlive)
            {
                Thread.Sleep(1);
            }
        }

        
        /// <summary>
        /// Handle a command from the CLI.
        /// </summary>
        /// <param name="Sender"></param>
        /// <param name="e"></param>
        private static void HandleCommand(object Sender, CommandEventArgs e)
        {
            mClient.SendData(e.Command + "\n");
        }


        private static void HandleMessageReceived(string Message)
        {
            mCLI.WriteLine(Message);
        }
    }
}

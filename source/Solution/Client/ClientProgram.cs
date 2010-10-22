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


            
            //Client C;
            
            //C = new Client();

            //for (int client = 0; client < 10; client++)
            //{
            //    C.StartClient();
            //    System.Threading.Thread.Sleep(1000);
            //    C.StopClient();
            //    System.Threading.Thread.Sleep(1000);
            //}
            
            //// Loop and send test commands
            //for (int client = 0; client < 10; client++)
            //{
            //    Console.WriteLine("Starting client");
            //    C = new Client();
            //    C.StartClient();
            //    for (int i = 0; i < 10; i++)
            //    {
            //        Console.WriteLine("Sending packet of data");
            //        //C.SendData("This is a really long test message sure to exceed the length of the receiving buffer " + i + "\n");
            //        C.SendData("A little bit " + i + "\n");
            //        System.Threading.Thread.Sleep(5000);
            //    }
            //    C.StopClient();
            //    Console.WriteLine("Stopped client");
            //}
       }

        
        /// <summary>
        /// Handle a command from the CLI.
        /// </summary>
        /// <param name="Sender"></param>
        /// <param name="e"></param>
        private static void HandleCommand(object Sender, CommandEventArgs e)
        {
            //TODO: something with e.Command
        }

        private static void HandleMessageReceived(string Message)
        {
            mCLI.WriteLine(Message);
        }
    }
}

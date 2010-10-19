using System;
using System.Threading;
namespace EnigmaMM
{
    class ServerProgram
    {
        //static Server mServer;
        static MCServer mMinecraft;
        static CLIHelper mCLI;
        static bool mKeepRunning;

        static void Main(string[] args)
        {
            //Console.WriteLine("Starting server listener");
            //mServer = new Server();
            //mServer.StartListener();

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
            
            Console.WriteLine("Getting Minecraft server object");
            mMinecraft = new MCServer();
            mMinecraft.ServerRoot = "D:\\Minecraft\\MCServerRoot\\Server";
            mMinecraft.LogMessage += HandleServerOutput;
            mMinecraft.InfoMessage += HandleServerOutput;
            mMinecraft.StartServer();


            mKeepRunning = true;
            while (mKeepRunning)
            {
                System.Threading.Thread.Sleep(100);
            }


            mMinecraft.Shutdown();
            mCLI.StopListening();
            
            Console.WriteLine("Done.");
        }



        /// <summary>
        /// Handle a command from the CLI.
        /// </summary>
        /// <remarks>
        /// Commands fall into one of several categories:
        /// a) commands that are processed by the server manager
        /// b) commands that are intended for Minecraft but handled by the manager
        /// c) commands that are passed straight on to Minecraft as-is
        /// </remarks>
        /// <param name="Sender"></param>
        /// <param name="e"></param>
        private static void HandleCommand(object Sender, CommandEventArgs e)
        {
            switch (e.Command)
            {
                case ("quit"):
                    mKeepRunning = false;
                    break;

                case ("start"):
                    mMinecraft.StartServer();
                    break;

                case ("restart"):
                    mMinecraft.Shutdown();
                    mMinecraft.StartServer();
                    break;

                case ("stop"):
                    mMinecraft.Shutdown();
                    break;

                default:
                    mMinecraft.SendCommand(e.Command);
                    break;
            }
        }



        private static void HandleServerOutput(string Message)
        {
            Console.WriteLine("SRV: " + Message);
        }



    }
}

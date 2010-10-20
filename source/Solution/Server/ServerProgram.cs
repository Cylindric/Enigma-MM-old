using System;
using System.Threading;

namespace EnigmaMM
{
    class ServerProgram
    {
        //static Server mServer;
        static MCServer mMinecraft;
        static CLIHelper mCLI;
        static CommandParser mParser;
        public static bool mKeepRunning;

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
            
            mCLI.WriteLine("Getting Minecraft server object");
            mMinecraft = new MCServer();
            mMinecraft.ServerRoot = config.Default.ServerRoot;
            mMinecraft.JavaExec = config.Default.ServerJar;
            mMinecraft.ServerJar = config.Default.ServerJar;
            mMinecraft.JavaHeapInit = config.Default.JavaHeapInit;
            mMinecraft.JavaHeapMax = config.Default.JavaHeapMax;

            mMinecraft.ServerMessage += HandleServerOutput;
            mMinecraft.StartServer();

            mParser = new CommandParser(mMinecraft);

            // Just loop until something sets mKeepRunning to false.
            // The loop sleeps for a little to stop it hogging the CPU
            mKeepRunning = true;
            while (mKeepRunning)
            {
                System.Threading.Thread.Sleep(100);
            }

            // Make sure the Minecraft server is stopped if we're exiting
            mParser.ParseCommand("stop");

            // Stop listening for CLI commands
            mCLI.StopListening();
            
            mCLI.WriteLine("Done.");
        }



        /// <summary>
        /// Handle a command from the CLI.
        /// </summary>
        /// <param name="Sender"></param>
        /// <param name="e"></param>
        private static void HandleCommand(object Sender, CommandEventArgs e)
        {
            mParser.ParseCommand(e.Command);
        }



        private static void HandleServerOutput(string Message)
        {
            mCLI.WriteLine("SRV: " + Message);
        }



    }
}

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

            // TODO: Get current exe path
            Config.Initialize(@"D:\Dev\Enigma-MM\source\Solution\Build\settings.xml");

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
            mMinecraft.ServerRoot = Config.ServerRoot;
            mMinecraft.JavaExec = Config.JavaExec;
            mMinecraft.ServerJar = Config.ServerJar;
            mMinecraft.JavaHeapInit = Config.JavaHeapInit;
            mMinecraft.JavaHeapMax = Config.JavaHeapMax;

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

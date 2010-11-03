using System.Threading;
using System.IO;
using System.Diagnostics;

namespace EnigmaMM
{
    class ServerProgram
    {
        static Server mServer;
        static MCServer mMinecraft;
        static CLIHelper mCLI;
        static CommandParser mParser;
        
        public static bool mKeepRunning;

        static void Main(string[] args)
        {
            Settings.Initialise(Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "settings.conf"));

            // Start the server up and begin listening for connections
            mServer = new Server();
            mServer.MessageReceived += HandleClientCommand;
            mServer.RemoteConnection += HandleClientConnected;
            mServer.RemoteDisconnection += HandleClientConnected;
            mServer.ServerPort = Settings.ServerPort;
            mServer.Username = Settings.ServerUsername;
            mServer.Password = Settings.ServerPassword;
            mServer.StartListener();

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
            
            mMinecraft = new MCServer();
            mMinecraft.ServerRoot = Settings.MinecraftRoot;
            mMinecraft.JavaExec = Settings.JavaExec;
            mMinecraft.ServerJar = Settings.ServerJar;
            mMinecraft.JavaHeapInit = Settings.JavaHeapInit;
            mMinecraft.JavaHeapMax = Settings.JavaHeapMax;

            mMinecraft.MapRoot = Settings.MapRoot;
            mMinecraft.AlphaVespucciInstalled = Settings.AlphaVespucciInstalled;
            mMinecraft.OverviewerInstalled = Settings.OverviewerInstalled;

            // See if we need to swap in a new config file, and load current config
            mMinecraft.ReloadConfig();

            mMinecraft.ServerMessage += HandleServerOutput;
            mMinecraft.ServerError += HandleServerError;
            mMinecraft.ServerStarted += HandleServerStarted;
            mMinecraft.ServerStopped += HandleServerStopped;

            mParser = new CommandParser(mMinecraft);

            // If any commands were passed on the command-line, execute them and then quit
            if (args.Length > 0)
            {
                foreach (string arg in args)
                {
                    mParser.ParseCommand(arg);
                }
            }


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
        private static void HandleCommand(string Command)
        {
            mParser.ParseCommand(Command);
        }

        private static void HandleClientCommand(string Command)
        {
            mParser.ParseCommand(Command);
        }


        private static void HandleClientConnected(string Command)
        {
            mCLI.WriteLine("Host: Client connected (now " + mServer.Connections + ")");
        }


        private static void HandleClientDisconnected(string Command)
        {
            mCLI.WriteLine("Host: Client disconnected (now " + mServer.Connections + ")");
        }


        private static void HandleServerOutput(string Message)
        {
            // Echo the message to the console
            mCLI.WriteLine("SRV: " + Message);

            // Pass all messages back out to clients
            mServer.SendData(Message);
        }
        

        private static void HandleServerError(string Message)
        {
            mCLI.WriteLine("Server error! " + Message);
        }


        private static void HandleServerStarted(string Message)
        {
            mCLI.WriteLine("Server started");
        }


        private static void HandleServerStopped(string Message)
        {
            mCLI.WriteLine("Server stopped");
        }


    }
}

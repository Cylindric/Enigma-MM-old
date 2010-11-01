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
            Config.Initialize(Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "settings.xml"));

            // Start the server up and begin listening for connections
            mServer = new Server();
            mServer.CommandReceived += HandleClientCommand;
            mServer.ClientConnected += HandleClientConnected;
            mServer.ClientDisconnected += HandleClientConnected;
            mServer.ServerPort = Config.ServerPort;
            mServer.Username = Config.ServerUsername;
            mServer.Password = Config.ServerPassword;
            mServer.StartListener();
            Debug.WriteLine("Well, here we are");

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
            mMinecraft.ServerRoot = Config.MinecraftRoot;
            mMinecraft.JavaExec = Config.JavaExec;
            mMinecraft.ServerJar = Config.ServerJar;
            mMinecraft.JavaHeapInit = Config.JavaHeapInit;
            mMinecraft.JavaHeapMax = Config.JavaHeapMax;

            mMinecraft.MapRoot = Config.MapRoot;
            mMinecraft.AlphaVespucciInstalled = Config.AlphaVespucciInstalled;
            mMinecraft.OverviewerInstalled = Config.OverviewerInstalled;

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
            mCLI.WriteLine("Host: Client connected (now " + mServer.ConnectedClients + ")");
        }


        private static void HandleClientDisconnected(string Command)
        {
            mCLI.WriteLine("Host: Client disconnected (now " + mServer.ConnectedClients + ")");
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

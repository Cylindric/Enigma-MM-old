using System;
namespace EnigmaMM
{
    class ServerProgram
    {
        static Server mServer;
        static CLIHelper mCLI;
        static bool mKeepRunning;

        static void Main(string[] args)
        {
            Console.WriteLine("Starting server listener");
            mServer = new Server();
            mServer.StartListener();

            // After this we might start getting user commands through HandleCommands
            mCLI = new CLIHelper();
            mCLI.RaiseCommandReceivedEvent += HandleCommand;
            mCLI.StartListening();

            //Console.WriteLine("Getting Minecraft server object");
            //MCServer MC = new MCServer();
            //MC.ServerRoot = "D:\\Minecraft\\MCServerBase\\Server1

            mKeepRunning = true;
            while (mKeepRunning)
            {
                System.Threading.Thread.Sleep(100);
            }

            Console.WriteLine("Done.");
        }



        private static void HandleCommand(object Sender, CommandEventArgs e)
        {
            Console.WriteLine("Received command: " + e.Message);
            switch (e.Message)
            {
                case ("quit"):
                    mKeepRunning = false;
                    break;
            }
        }



    }
}

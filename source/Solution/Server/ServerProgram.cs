using System;
namespace EnigmaMM
{
    class ServerProgram
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting server listener");
            Server L = new Server();
            L.StartListener();

            // After this we might start getting user commands through HandleCommands
            CLIHelper cli = new CLIHelper();
            cli.RaiseCommandReceivedEvent += HandleCommand;
            cli.StartListening();

            //Console.WriteLine("Getting Minecraft server object");
            //MCServer MC = new MCServer();
            //MC.ServerRoot = "D:\\Minecraft\\MCServerBase\\Server1

            bool KeepRunning = true;
            while (KeepRunning)
            {
                System.Threading.Thread.Sleep(100);
            }

            Console.WriteLine("Done.");
        }



        private static void HandleCommand(object Sender, CommandEventArgs e)
        {
            Console.WriteLine("Received command: " + e.Message);
        }
    }
}

using System;
namespace EnigmaMM
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting server listener");
            Server L = new Server();
            L.StartListener();

            Console.WriteLine("Getting Minecraft server object");
            MCServer MC = new MCServer();
            MC.ServerRoot = "D:\\Minecraft\\MCServerBase\\Server1";

            bool KeepRunning = true;
            while (KeepRunning)
            {
                System.Threading.Thread.Sleep(100);
            }

            Console.WriteLine("Done.");
        }
    }
}

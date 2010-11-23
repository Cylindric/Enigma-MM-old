using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace MinecraftSimulator
{
    class Program
    {
        private static CLIHelper mCli = new CLIHelper();
        private static Simulator mSim = new Simulator();

        static void Main(string[] args)
        {
            mCli.RaiseCommandReceivedEvent += HandleCommand;
            mSim.RaiseCommandReceivedEvent += HandleServerOutput;

            Thread CLIThread = new Thread(new ThreadStart(mCli.Start));
            Thread SIMThread = new Thread(new ThreadStart(mSim.Start));
            CLIThread.Start();
            SIMThread.Start();

            SIMThread.Join();
            mCli.StopListening();
            CLIThread.Join();
        }
        
        private static void HandleCommand(string Command)
        {
            //Console.WriteLine(Command);
            mSim.SendCommand(Command);
        }

        private static void HandleServerOutput(string Message)
        {
            Console.WriteLine(Message);
        }
    }
}

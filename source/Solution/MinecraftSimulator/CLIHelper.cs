using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MinecraftSimulator
{    
    public class CLIHelper
    {
        private bool mKeepRunning = true;

        public delegate void ServerMessageEventHandler(string Message);
        public event ServerMessageEventHandler RaiseCommandReceivedEvent;

        public void Start()
        {
            string line;
            while (mKeepRunning)
            {
                line = Console.In.ReadLine();
                OnCommandReceivedEvent(line);
                System.Threading.Thread.Sleep(100);
            }
        }

        public void WriteLine(string Message)
        {
            Console.WriteLine(Message);
        }

        public void StopListening()
        {
            mKeepRunning = false;
        }

        protected virtual void OnCommandReceivedEvent(string Message)
        {
            if (RaiseCommandReceivedEvent != null)
            {
                RaiseCommandReceivedEvent(Message);
            }

        }

    }
}

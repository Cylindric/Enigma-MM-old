using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EnigmaMM
{    
    public class CLIHelper
    {
        private bool mKeepRunning = true;

        public delegate void ServerMessageEventHandler(string Message);
        public event ServerMessageEventHandler RaiseCommandReceivedEvent;

        public void StartListening()
        {
            string cmd = "";
            ConsoleKeyInfo key = default(ConsoleKeyInfo);

            while (mKeepRunning)
            {
                if ((Console.KeyAvailable))
                {
                    key = Console.ReadKey();
                    switch (key.Key)
                    {
                        case ConsoleKey.Backspace:
                            cmd = cmd.Substring(0, Math.Max(cmd.Length - 1, 0));
                            break;
                        case ConsoleKey.Enter:
                            OnCommandReceivedEvent(cmd);
                            cmd = "";
                            break;
                        default:
                            cmd = cmd + key.KeyChar;
                            break;
                    }
                }
                else
                {
                    System.Threading.Thread.Sleep(100);
                }
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

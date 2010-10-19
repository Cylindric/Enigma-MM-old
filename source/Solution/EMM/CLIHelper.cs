using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EnigmaMM
{

    public class CommandEventArgs : EventArgs
    {
        public CommandEventArgs(string s)
        {
            command = s;
        }
        private string command;

        public string Command
        {
            get { return command; }
            set { command = value; }
        }
    }

    
    
    public class CLIHelper
    {
        private bool mKeepRunning = true;

        public event EventHandler<CommandEventArgs> RaiseCommandReceivedEvent;

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
                            OnCommandReceivedEvent(new CommandEventArgs(cmd));
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


        public void StopListening()
        {
            mKeepRunning = false;
        }


        protected virtual void OnCommandReceivedEvent(CommandEventArgs e)
        {
            EventHandler<CommandEventArgs> handler = RaiseCommandReceivedEvent;
            if(handler != null)
            {
                handler(this, e);
            }

        }

    }
}

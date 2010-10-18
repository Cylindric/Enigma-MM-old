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
            message = s;
        }
        private string message;

        public string Message
        {
            get { return message; }
            set { message = value; }
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
                            cmd = cmd.Substring(0, cmd.Length - 1);
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

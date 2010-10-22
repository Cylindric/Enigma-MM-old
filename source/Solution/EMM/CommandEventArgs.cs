using System;

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
}

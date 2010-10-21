using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EnigmaMM
{
    public class ServerMessageEventArgs : EventArgs
    {
        public ServerMessageEventArgs(string s)
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
}

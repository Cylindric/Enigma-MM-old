using System;

namespace EnigmaMM.Engine
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnigmaMM;

namespace EnigmaMM
{
    class CLI
    {
        private MCServer m_Server;

        public CLI(MCServer ServerInstance)
        {
            m_Server = ServerInstance;
            m_Server.ServerMessage += new MCServer.ServerMessageEventHandler(this.Server_OnMessageReceived);
        }

        public void WriteLine(string Tag, string Message)
        {
            Console.WriteLine(Tag + ": " + Message);
        }

        public void WriteLine()
        {
            WriteLine("Console", "");
        }

        public void WriteLine(string Message)
        {
            WriteLine("Console", Message);
        }

        private void Server_OnMessageReceived(string Message)
        {
            WriteLine("SRV", Message);
        }

    }
}

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
            m_Server.LogMessage += new MCServer.LogMessageEventHandler(this.Server_OnLogMessage);
            m_Server.InfoMessage += new MCServer.InfoMessageEventHandler(this.Server_OnInfoMessage);
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

        private void Server_OnLogMessage(string Message) 
        {
            WriteLine("LOG", Message);
        }

        private void Server_OnInfoMessage(string Message)
        {
            WriteLine("INF", Message);
        }

    }
}

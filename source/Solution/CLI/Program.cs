using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnigmaMM;
using System.Configuration;

namespace EnigmaMM
{
    class Program
    {

        private static CLI m_Con;
        private static MCServer m_Server;

        static void Main(string[] args)
        {
            m_Server = new MCServer();
            
            m_Con = new CLI(m_Server);

            Console.WriteLine("Initialising server...");
            m_Server.ServerRoot = Properties.Settings.Default.ServerRoot;
            m_Server.ServerJar = Properties.Settings.Default.ServerJar;
            m_Server.JavaHeapInit = Properties.Settings.Default.JavaMemInit;
            m_Server.JavaHeapMax = Properties.Settings.Default.JavaMemMax;

            m_Con.WriteLine("Starting server..");
            m_Server.StartServer();
            while (m_Server.CurrentStatus != MCServer.Status.Running)
            {
                System.Threading.Thread.Sleep(500);
                if (m_Server.CurrentStatus == MCServer.Status.Failed)
                {
                    break;
                }
            }

            if (m_Server.CurrentStatus == MCServer.Status.Running)
            {
                m_Con.WriteLine("started.");
            }
            else
            {
                m_Con.WriteLine("Failed to start server.");
                m_Con.WriteLine();
            }

            string cmd = "";
            ConsoleKeyInfo key = default(ConsoleKeyInfo);

            while (m_Server.CurrentStatus == MCServer.Status.Running)
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
                            ExecuteCommand(cmd);
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

        private static void ExecuteCommand(string Command)
        {
            if (Command.Equals("list"))
            {
                m_Con.WriteLine("Online users: " + m_Server.OnlineUsers());
            }
            else
            {
                m_Server.SendCommand(Command);
            }
        }

    }

}

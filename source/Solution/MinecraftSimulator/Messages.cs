using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml;
using System.IO;

namespace MinecraftSimulator
{
    class Messages
    {
        private Simulator mSim;
        private Random mRand = new Random();
        public Dictionary<string, string> mMessages { private set; get; }

        internal Messages(Simulator sim)
        {
            mSim = sim;
            PopulateRules();
        }

        private void PopulateRules()
        {
            mMessages = new Dictionary<string, string>();

            XmlDocument xml = new XmlDocument();
            xml.Load(Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "messages.simulator.xml"));
            XmlNodeList nodeList = xml.DocumentElement.SelectNodes("/messages/message");
            foreach (XmlNode message in nodeList)
            {
                XmlNode name = message.SelectSingleNode("name");
                XmlNode rule = message.SelectSingleNode("text");
                mMessages.Add(name.InnerText, rule.InnerText);
            }
        }

        internal void StartupMessages()
        {
            SendMessage(mMessages["Startup"]);
            if (mSim.LowMem)
            {
                SendMessage(mMessages["NotEnoughMemory"]);
                SendMessage(mMessages["MemorySettingHelp"]);
            }
            SendMessage(mMessages["LoadingProps"]);
            SendMessage(mMessages["StartingServerPort"]);
            SendMessage(mMessages["PreparingWorld"].Replace("{world}", "world"));
            SendMessage(mMessages["PreparingRegion"]);
        }

        internal void PrepareSpawnArea()
        {
            for (int i = 0; i < 100; i += 5)
            {
                SendMessage(mMessages["PreparingSpawn"].Replace("{percent}", i.ToString()));
                if (!mSim.Fast)
                {
                    Thread.Sleep(mRand.Next(100, 1000));
                }
            }
            SendMessage(mMessages["StartupComplete"]);
        }

        internal void Lagging()
        {
            SendMessage(mMessages["Overloaded"]);
        }

        internal void UnknownCommand()
        {
            SendMessage(mMessages["UnknownCommand"]);
        }

        internal void Save()
        {
            if (!mSim.Fast)
            {
                Thread.Sleep(200);
            }
            SendMessage(mMessages["SaveStart"]);
            if (!mSim.Fast)
            {
                Thread.Sleep(2000);
            }
            SendMessage(mMessages["SaveComplete"]);
        }

        internal void SaveOn()
        {
            SendMessage(mMessages["AutoSaveEnabled"]);
        }

        internal void SaveOff()
        {
            SendMessage(mMessages["AutoSaveDisabled"]);
        }

        internal void ConnectedPlayers()
        {
            SendMessage(mMessages["UserList"].Replace("{userlist}", String.Join(", ", mSim.Players.ToArray())));
        }
        internal void Stop()
        {
            SendMessage("INFO", "CONSOLE: Stopping the server..");
            SendMessage("INFO", "Stopping server");
            SendMessage("INFO", "Saving chunks");
        }

        internal void PlayerJoined(string username)
        {
            string ipaddress = string.Format("{0}.{1}.{2}.{3}:{4}", mRand.Next(1, 254), mRand.Next(1, 254), mRand.Next(1, 254), mRand.Next(1, 254), mRand.Next(1, 63000));

            SendMessage(mMessages["UserLoggedIn"]
                .Replace("{username}", username)
                .Replace("{address}", ipaddress)
                .Replace("{entityid}", mRand.Next(1, 5000).ToString()));

            SendMessage(mMessages["UserCount"].Replace("{count}", mSim.Players.Count.ToString()));
        }

        internal void PlayerQuit(string username)
        {
            SendMessage(mMessages["UserLoggedOut"]
                .Replace("{username}", username)
                .Replace("{reason}", "disconnect.quitting"));

            SendMessage(mMessages["UserCount"].Replace("{count}", mSim.Players.Count.ToString()));
        }

        internal void PlayerDisconnected(string username)
        {
            SendMessage("java.net.SocketException: Connection reset by peer: socket write error");
            SendMessage("        at java.net.SocketOutputStream.socketWrite0(Native Method)");
            SendMessage("        at java.net.SocketOutputStream.socketWrite(Unknown Source)");
            SendMessage("        at java.net.SocketOutputStream.write(Unknown Source)");
            SendMessage("        at java.io.DataOutputStream.write(Unknown Source)");
            SendMessage("        at ju.a(SourceFile:105)");
            SendMessage("        at bs.e(SourceFile:142)");
            SendMessage("        at bs.d(SourceFile:15)");
            SendMessage("        at jt.run(SourceFile:85)");
            SendMessage("java.net.SocketException: Software caused connection abort: recv failed");
            SendMessage("        at java.net.SocketInputStream.socketRead0(Native Method)");
            SendMessage("        at java.net.SocketInputStream.read(Unknown Source)");
            SendMessage("        at java.net.SocketInputStream.read(Unknown Source)");
            SendMessage("        at java.io.FilterInputStream.read(Unknown Source)");
            SendMessage("        at ju.b(SourceFile:95)");
            SendMessage("        at bs.f(SourceFile:157)");
            SendMessage("        at bs.c(SourceFile:15)");
            SendMessage("        at js.run(SourceFile:68)");

            SendMessage(mMessages["UserLoggedOut"]
                .Replace("{username}", username)
                .Replace("{reason}", "disconnect.genericReason"));

            SendMessage(mMessages["UserCount"].Replace("{count}", mSim.Players.Count.ToString()));
        }

        public void SendMessage(string message)
        {
            mSim.SendMessage(message.Replace("{timestamp}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")));
        }

        private void SendMessage(string category, string message)
        {
            mSim.SendMessage(string.Format("{0} [{1}] {2}", DateTime.Now, category, message));
        }
    }
}

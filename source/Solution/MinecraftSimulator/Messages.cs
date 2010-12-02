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
                .Replace("{reason}", "Quitting"));

            SendMessage(mMessages["UserCount"].Replace("{count}", mSim.Players.Count.ToString()));
        }

        internal void PlayerDisconnected(string username)
        {
            SendMessage("java.net.SocketException: Connection reset");
            SendMessage("   at java.net.SocketInputStream.read(Unknown Source)");
            SendMessage("   at java.net.SocketInputStream.read(Unknown Source)");
            SendMessage("   at java.io.FilterInputStream.read(Unknown Source)");
            SendMessage("   at io.b(io.java:47)");
            SendMessage("   at bh.f(SourceFile:147)");
            SendMessage("   at bh.c(SourceFile:9)");
            SendMessage("   at im.run(SourceFile:57)");

            SendMessage(mMessages["UserLoggedOut"]
                .Replace("{username}", username)
                .Replace("{reason}", "Internal exception: java.net.SocketException"));

            SendMessage(mMessages["UserCount"].Replace("{count}", mSim.Players.Count.ToString()));
        }

        //[WARNING] **** SERVER IS RUNNING IN OFFLINE/INSECURE MODE!
        //[WARNING] The server will make no attempt to authenticate usernames. Beware.
        //[WARNING] While this makes the game possible to play without internet access, it also opens up the ability for hackers to connect with any username they choose.
        //[WARNING] To change this, set "online-mode" to "true" in the server.settings file.

        //[INFO] Disconnecting Player [/127.0.0.1:61459]: Outdated client!

        //[INFO] To run the server without a gui, start it like this:
        //[INFO]    java -Xmx1024M -Xms1024M -jar minecraft_server.jar nogui
        //[INFO] Console commands:
        //[INFO]    help  or  ?               shows this message
        //[INFO]    kick <player>             removes a player from the server
        //[INFO]    ban <player>              bans a player from the server
        //[INFO]    pardon <player>           pardons a banned player so that they can connect again
        //[INFO]    ban-ip <ip>               bans an IP address from the server
        //[INFO]    pardon-ip <ip>            pardons a banned IP address so that they can connect again
        //[INFO]    op <player>               turns a player into an op
        //[INFO]    deop <player>             removes op status from a player
        //[INFO]    tp <player1> <player2>    moves one player to the same location as another player
        //[INFO]    give <player> <id> [num]  gives a player a resource
        //[INFO]    tell <player> <message>   sends a private message to a player
        //[INFO]    stop                      gracefully stops the server
        //[INFO]    save-all                  forces a server-wide level save
        //[INFO]    save-off                  disables terrain saving (useful for backup scripts)
        //[INFO]    save-on                   re-enables terrain saving
        //[INFO]    list                      lists all currently connected players
        //[INFO]    say <message>             broadcasts a message to all players

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

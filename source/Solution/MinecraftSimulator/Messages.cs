using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace MinecraftSimulator
{
    class Messages
    {
        private Simulator mSim;
        private Random mRand = new Random();

        internal Messages(Simulator sim)
        {
            mSim = sim;
        }

        internal void StartupMessages()
        {
            SendMessage("INFO", "Starting minecraft server version 0.2.2_01");
            if (mSim.LowMem)
            {
                SendMessage("WARNING", "**** NOT ENOUGH RAM!");
                SendMessage("WARNING", "To start the server with more ram, launch it as java -Xmx1024M -Xms1024M -jar minecraft_server.jar");
            }
            SendMessage("INFO", "Loading properties");
            SendMessage("INFO", "Starting Minecraft server on *:25565");
            SendMessage("INFO", "Preparing level \"world\"");
            SendMessage("INFO", "Preparing start region");
        }

        internal void PrepareSpawnArea()
        {
            for (int i = 0; i < 100; i += 5)
            {
                SendMessage(string.Format("Preparing spawn area: {0}%", i));
                if (!mSim.Fast)
                {
                    Thread.Sleep(mRand.Next(100, 1000));
                }
            }
            SendMessage("INFO", "Done! For help, type \"help\" or \"?\"");
        }

        internal void Lagging()
        {
            SendMessage("WARNING", "Can't keep up! Did the system time change, or is the server overloaded?");
        }

        internal void UnknownCommand()
        {
            SendMessage("INFO", "Unknown console command. Type \"help\" for help.");
        }

        internal void Save()
        {
            if (!mSim.Fast)
            {
                Thread.Sleep(200);
            }
            SendMessage("INFO", "CONSOLE: Forcing save..");
            if (!mSim.Fast)
            {
                Thread.Sleep(2000);
            }
            SendMessage("INFO", "CONSOLE: Save complete.");
        }

        internal void SaveOn()
        {
            SendMessage("INFO", "CONSOLE: Enabling level saving..");
        }

        internal void SaveOff()
        {
            SendMessage("INFO", "CONSOLE: Disabling level saving..");
        }

        internal void ConnectedPlayers()
        {
            SendMessage("INFO", string.Format("Connected players:{0}", string.Join(", ", mSim.Players)));
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
            SendMessage("INFO", string.Format("{0} [/{1}] logged in with entity id {2}", username, ipaddress, mRand.Next(1,5000)));
            SendMessage("INFO", string.Format("Player count: {0}", mSim.Players.Count));
        }

        internal void PlayerQuit(string username)
        {
            SendMessage("INFO", string.Format("{0} lost connection: Quitting", username));
            SendMessage("INFO", string.Format("Player count: {0}", mSim.Players.Count));
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
            SendMessage("INFO", string.Format("{0} lost connection: Internal exception: java.net.SocketException", username));
            SendMessage("INFO", string.Format("Player count: {0}", mSim.Players.Count));
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

    private void SendMessage(string category, string message)
        {
            mSim.SendMessage(string.Format("{0} [{1}] {2}", DateTime.Now, category, message));
        }

        private void SendMessage(string message)
        {
            mSim.SendMessage(message);
        }
    }
}

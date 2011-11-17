using System.IO;
using EnigmaMM.Engine;

namespace EnigmaMM
{
    public class MCServerProperties : SettingsFile
    {

        public MCServerProperties(EMMServer server) : base(server, Path.Combine(server.Settings.MinecraftRoot, "server.properties"), '=')
        {
        }

        // Standard server settings
        public bool Monsters
        {
            get { return GetBool("monsters"); }
        }

        public bool OnlineMode
        {
            get { return GetBool("online-mode"); }
        }

        public int MaxPlayers
        {
            get { return GetInt("max-players"); }
        }

        public int ServerPort
        {
            get { return GetInt("server-port"); }
        }

        public string LevelName
        {
            get { return GetString("level-name"); }
        }

        public string ServerIp
        {
            get { return GetString("server-ip"); }
        }

        public string WorldPath
        {
            get { return Path.Combine(mServer.Settings.MinecraftRoot, LevelName); }
        }

    }
}

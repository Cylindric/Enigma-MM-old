using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

namespace EnigmaMM
{
    public class MCServerProperties: SettingsFile
    {

        public MCServerProperties() : base(Path.Combine(Settings.MinecraftRoot, "server.properties"), '=')
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
            get { return Path.Combine(Settings.MinecraftRoot, LevelName); }
        }

        // Hey0 Extensions
        public string WarpLocation
        {
            get { return GetString("warplocation"); }
        }

    }
}

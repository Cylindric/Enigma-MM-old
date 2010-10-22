﻿using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace EnigmaMM
{
    public class MCServerProperties
    {
        private Dictionary<string, string> mSettings = new Dictionary<string, string>();
        private bool mSettingsNeedSaving = false;

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
            get { return Path.Combine(Config.MinecraftRoot, LevelName); }
        }


        /// <summary>
        /// Checks for a server.new.properties file, and if it exists swaps it in for
        /// server.properties.  The current properties file is copied to server.old.properties.
        /// </summary>
        public void LookForNewSettings()
        {
            string currentSettings = Path.Combine(Config.MinecraftRoot, "server.properties");
            string newSettings = Path.Combine(Config.MinecraftRoot, "server.new.properties");
            string oldSettings = Path.Combine(Config.MinecraftRoot, "server.old.properties");

            if (File.Exists(newSettings))
            {
                if (File.Exists(oldSettings))
                {
                    File.Delete(oldSettings);
                }
                if (File.Exists(currentSettings))
                {
                    File.Move(currentSettings, oldSettings);
                }
                File.Move(newSettings, currentSettings);
            }
            ParseServerProperties();
        }


        private void ParseServerProperties()
        {
            // TODO: load these values from the server file
            mSettings.Add("level-name", "world");
            mSettings.Add("server-port", "25565");
            mSettings.Add("server-ip", "");
            mSettings.Add("online-mode", "true");
            mSettings.Add("monsters", "false");
            mSettings.Add("max-players", "20");
            
            mSettings.Add("spawn-protection-size", "16");
            mSettings.Add("disalloweditems", "");
            mSettings.Add("alloweditems", "");
            mSettings.Add("itemstxtlocation", "items.txt");
            mSettings.Add("group-txt-location", "groups.txt");
            mSettings.Add("whitelist", "false");
            mSettings.Add("homelocation", "homes.txt");
            mSettings.Add("logging", "false");
            mSettings.Add("kitstxtlocation", "kits.txt");
            mSettings.Add("whitelist-txt-location", "whitelist.txt");
            mSettings.Add("itemspawnblacklist", "");
            mSettings.Add("whitelist-message", "Not on whitelist.");
            mSettings.Add("warplocation", "warps.txt");
            mSettings.Add("plugins", "");
            mSettings.Add("reservelist-txt-location", "reservelist.txt");
            mSettings.Add("admintxtlocation", "users.txt");
            mSettings.Add("save-homes", "true");
            mSettings.Add("data-source", "flatfile");
            mSettings.Add("motd", "Type /help for a list of commands.");
        }


        public string GetString(string key)
        {
            string value = "";
            if (mSettings.ContainsKey(key))
            {
                value = mSettings[key];
            }
            return value;
        }

        public int GetInt(string key)
        {
            int value = 0;
            int.TryParse(GetString(key), out value);
            return value;
        }

        public bool GetBool(string key)
        {
            bool value = false;
            bool.TryParse(GetString(key), out value);
            return value;
        }

    }
}

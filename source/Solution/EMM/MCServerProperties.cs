using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

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
            LoadServerProperties();
        }



        /// <summary>
        /// Loads the current server.properties.
        /// </summary>
        /// <remarks>
        /// It is possible that the server.properties file doesn't exist, usually because it's the first
        /// time the server has been started.  This method should be called <em>after</em> the Minecraft server
        /// has started, to be sure to get something.</remarks>
        /// <returns>true if a file was found and loaded, else false.</returns>
        public bool LoadServerProperties()
        {
            string currentSettings = Path.Combine(Config.MinecraftRoot, "server.properties");

            if (File.Exists(currentSettings) == false)
            {
                return false;
            }

            mSettingsNeedSaving = false;
            StreamReader Sr;
            string S;
            Sr = File.OpenText(currentSettings);
            S = Sr.ReadLine();
            string[] vars;
            string key;
            string value;
            while (S != null)
            {
                S = S.Trim();

                if (S.StartsWith("#"))
                {
                    // comment lines can be ignored
                }
                else
                {
                    // Split the config line into a key and a value, separated by a "="
                    // If the value contains an "=" symbol, then we need to be sure to join
                    // them all together again
                    key = "";
                    value = "";
                    vars = S.Split('=');
                    if (vars.Length > 0)
                    {
                        key = vars[0];
                        if (vars.Length >= 1)
                        {
                            value = string.Join("=", vars, 1, vars.Length - 1);
                        }
                        value = value.Trim();
                        SetValue(key, value);
                    }
                }
                S = Sr.ReadLine();
            }
            Sr.Close();
            return true;
        }


        private void SaveServerProperties()
        {
            if (mSettingsNeedSaving)
            {
                string newSettings = Path.Combine(Config.MinecraftRoot, "server.new.properties");
                StreamWriter Sw;
                Sw = File.CreateText(newSettings);
                foreach (KeyValuePair<string, string> setting in mSettings)
                {
                    Sw.WriteLine(setting.Key + '=' + setting.Value);
                }
                Sw.Close();
            }
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

        public void SetValue(string key, int value)
        {
            SetValue(key, value.ToString());
        }

        public void SetValue(string key, bool value)
        {
            SetValue(key, value.ToString().ToLower());
        }

        public void SetValue(string key, string value)
        {
            if (mSettings.ContainsKey(key))
            {
                if (mSettings[key] != value)
                {
                    mSettings[key] = value;
                    mSettingsNeedSaving = true;
                }
            }
            else
            {
                mSettings.Add(key, value);
                mSettingsNeedSaving = true;
            }
        }

    }
}

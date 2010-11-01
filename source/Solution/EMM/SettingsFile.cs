using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace EnigmaMM
{
    public class SettingsFile
    {
        protected Dictionary<string, string> mSettings = new Dictionary<string, string>();

        private string mSettingsFile = "";
        private bool mSettingsNeedSaving = false;
        private char mSeparator = '=';

        protected SettingsFile(string fileName, char separator)
        {
            mSettingsFile = fileName;
            mSeparator = separator;
        }


        /// <summary>
        /// Checks for a new.settingsfile file, and if it exists swaps it in for
        /// settingsfile.  The current properties file is copied to old.settingsfile.
        /// </summary>
        public void LookForNewSettings()
        {
            string currentSettings = Path.Combine(Config.MinecraftRoot, mSettingsFile);
            string newSettings = Path.Combine(Config.MinecraftRoot, "new." + mSettingsFile);
            string oldSettings = Path.Combine(Config.MinecraftRoot, "old." + mSettingsFile);

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
            string currentSettings = Path.Combine(Config.MinecraftRoot, mSettingsFile);

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
                    vars = S.Split(mSeparator);
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
                string newSettings = Path.Combine(Config.MinecraftRoot, "new." + mSettingsFile);
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

﻿using System;
using System.Collections.Generic;
using System.IO;
using EnigmaMM.Engine;

namespace EnigmaMM
{
    public class SettingsFile 
    {
        protected Dictionary<string, string> mSettings = new Dictionary<string, string>();
        protected EMMServer mServer;

        private TimeSpan RELOADINTERVAL = new TimeSpan(0, 0, 5);

        private string mSettingsFile = "";
        private bool mSettingsNeedSaving = false;
        private char mSeparator = '=';
        private bool mAutoReload = false;
        private DateTime mLastReload;
        private bool mLoaded = false;

        public Dictionary<string, string> Values
        {
            get { return mSettings; }
        }

        public string Filename
        {
            get { return mSettingsFile; }
        }

        public bool AutoLoad
        {
            get { return mAutoReload; }
            set { mAutoReload = value; }
        }

        public bool Loaded
        {
            get { return mLoaded; }
        }

        public SettingsFile(EMMServer server, string fileName, char separator)
        {
            mServer = server;
            mSettingsFile = fileName;
            mSeparator = separator;
        }

        /// <summary>
        /// Checks for a new.settingsfile file, and if it exists swaps it in for
        /// settingsfile.  The current properties file is copied to old.settingsfile.
        /// </summary>
        public void LookForNewSettings()
        {
            string currentSettings = mSettingsFile;
            string newSettings = Path.Combine(Path.GetDirectoryName(mSettingsFile), "new." + Path.GetFileName(mSettingsFile));
            string oldSettings = Path.Combine(Path.GetDirectoryName(mSettingsFile), "old." + Path.GetFileName(mSettingsFile));

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
            Load();
        }


        /// <summary>
        /// Loads the current server.properties.
        /// </summary>
        /// <remarks>
        /// It is possible that the server.properties file doesn't exist, usually because it's the first
        /// time the server has been started.  This method should be called <em>after</em> the Minecraft server
        /// has started, to be sure to get something.</remarks>
        /// <returns>true if a file was found and loaded, else false.</returns>
        public bool Load()
        {
            if (File.Exists(mSettingsFile) == false)
            {
                return false;
            }

            mSettingsNeedSaving = false;

            string S;
            using (FileStream fileStream = new FileStream(mSettingsFile, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (StreamReader sr = new StreamReader(fileStream))
                {
                    while (!sr.EndOfStream)
                    {
                        S = sr.ReadLine();
                        S = S.Trim();
                        string[] vars;
                        string key;
                        string value;

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
                                    value = string.Join(mSeparator.ToString(), vars, 1, vars.Length - 1);
                                }
                                value = value.Trim();
                                SetValue(key, value);
                            }
                        }
                    }
                }
            }
            mLastReload = DateTime.Now;
            mLoaded = true;
            return true;
        }


        private void Save()
        {
            if (mSettingsNeedSaving)
            {
                string newSettings = Path.Combine(Path.GetDirectoryName(mSettingsFile), "new." + Path.GetFileName(mSettingsFile));
                StreamWriter Sw;
                Sw = File.CreateText(newSettings);
                foreach (KeyValuePair<string, string> setting in mSettings)
                {
                    Sw.WriteLine(setting.Key + mSeparator + setting.Value);
                }
                Sw.Close();
            }
        }


        public string GetString(string key, string defaultValue)
        {
            // If auto-reloading is enabled, and we haven't just loaded it.
            if ((mAutoReload) && (mLastReload.Add(RELOADINTERVAL) < DateTime.Now))
            {
                DateTime fileModifiedDate = File.GetLastWriteTime(mSettingsFile);
                if (fileModifiedDate > mLastReload)
                {
                    Load();
                }
            }

            string value = defaultValue;
            if (mSettings.ContainsKey(key))
            {
                value = mSettings[key];
            }
            return value;
        }


        public string GetString(string key)
        {
            return GetString(key, "");
        }

        public int GetInt(string key, int defaultValue)
        {
            int value = defaultValue;
            int.TryParse(GetString(key, defaultValue.ToString()), out value);
            return value;
        }

        public int GetInt(string key)
        {
            return GetInt(key, 0);
        }

        public bool GetBool(string key, bool defaultValue)
        {
            bool value = defaultValue;
            bool.TryParse(GetString(key), out value);
            return value;
        }

        public bool GetBool(string key)
        {
            return GetBool(key, false);
        }

        public string GetRootedPath(string root, string key, string defaultValue)
        {
            string path = GetString(key, defaultValue);
            if (path.StartsWith("."))
            {
                path = Path.Combine(root, path);
                path = Path.GetFullPath(path);
            }
            return path;
        }

        public string GetRootedPath(string root, string key)
        {
            return GetRootedPath(root, key, "");
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

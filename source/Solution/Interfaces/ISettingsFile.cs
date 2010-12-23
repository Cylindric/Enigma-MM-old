using System.Collections.Generic;

namespace EnigmaMM.Interfaces
{
    public interface ISettingsFile
    {
        Dictionary<string, string> Values { get; }
        string Filename { get; }
        bool AutoLoad { get; }
        bool Monsters { get; }
        bool OnlineMode { get; }
        int MaxPlayers { get; }
        int ServerPort { get; }
        string LevelName { get; }
        string ServerIp { get; }
        string WorldPath { get; }

        void LookForNewSettings();
        bool Load();


        //string GetString(string key, string defaultValue);
        //string GetString(string key);
        //int GetInt(string key, int defaultValue);
        //int GetInt(string key);
        //bool GetBool(string key, bool defaultValue);
        //bool GetBool(string key);
        //string GetRootedPath(string root, string key, string defaultValue);
        //string GetRootedPath(string root, string key);
        //void SetValue(string key, int value);
        //void SetValue(string key, bool value);
        //void SetValue(string key, string value);
    }
}

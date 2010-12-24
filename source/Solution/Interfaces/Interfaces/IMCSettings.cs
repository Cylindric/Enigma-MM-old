using System.Collections.Generic;

namespace EnigmaMM.Interfaces
{
    public interface IMCSettings
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
    }
}

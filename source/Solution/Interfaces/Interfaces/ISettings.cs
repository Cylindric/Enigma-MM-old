using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EnigmaMM.Interfaces
{
    public interface ISettings
    {
        /// <summary>
        /// Loads the settings.
        /// </summary>
        void LookForNewSettings();

        bool Loaded { get; }

        /// <summary>
        /// Returns the filename of the currently-used settings file.
        /// </summary>
        string Filename { get; }

        string GetString(string key, string defaultValue);
        string GetString(string key);
        int GetInt(string key, int defaultValue);
        int GetInt(string key);
        bool GetBool(string key, bool defaultValue);
        bool GetBool(string key);
        string GetRootedPath(string root, string key, string defaultValue);
        string GetRootedPath(string root, string key);
    }
}

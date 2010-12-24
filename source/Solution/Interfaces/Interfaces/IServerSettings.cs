using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EnigmaMM.Interfaces
{
    public interface IServerSettings
    {
        /// <summary>
        /// Initialises the Settings class and loads the settings from the file specified.
        /// </summary>
        /// <param name="fileName">The full path to the settings file.</param>
        void Initialise(string fileName);

        bool Loaded { get; }

        /// <summary>
        /// Returns the filename of the currently-used settings file.
        /// </summary>
        string Filename { get; }

        /// <summary>
        /// Returns the full path to the Server Manager.
        /// </summary>
        string ServerManagerRoot { get; }

        /// <summary>
        /// Returns the full path to the directory to use for cache files.
        /// </summary>
        string CacheRoot { get; }

        /// <summary>
        /// Returns the full path to the folder to use for backups.
        /// </summary>
        string BackupRoot { get; }

        /// <summary>
        /// Returns the full path to the folder where Minecraft is installed.
        /// </summary>
        string MinecraftRoot { get; }

        /// <summary>
        /// Returns the executable to execute the Java files.
        /// </summary>
        string JavaExec { get; }

        /// <summary>
        /// Returns the jar to use to launch Minecraft.
        /// </summary>
        string ServerJar { get; }

        /// <summary>
        /// Returns the initial amount of memory, in megabytes, to allocate to the Java heap.
        /// </summary>
        int JavaHeapInit { get; }

        /// <summary>
        /// Returns the maximum amount of memory, in megabytes, to allow the Java heap.
        /// </summary>
        int JavaHeapMax { get; }

        string MapRoot { get; }

        int OptimisePng { get; }

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

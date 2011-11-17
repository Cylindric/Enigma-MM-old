using System.IO;
using EnigmaMM.Engine;

namespace EnigmaMM
{
    /// <summary>
    /// Utility class for handling server manager configuration.
    /// </summary>
    public class Settings
    {
        private SettingsFile mSettings;
        private EMMServer mServer;

        public Settings(EMMServer server)
        {
            mServer = server;
        }

        /// <summary>
        /// Initialises the Settings class and loads the settings from the file specified.
        /// </summary>
        /// <param name="fileName">The full path to the settings file.</param>
        public void Initialise(string fileName)
        {
            mSettings = new SettingsFile(mServer, fileName, '=');
            mSettings.AutoLoad = true;
            mSettings.Load();
        }

        public bool Loaded
        {
            get { return mSettings != null;}
        }

        /// <summary>
        /// Returns the filename of the currently-used settings file.
        /// </summary>
        public string Filename
        {
            get { return mSettings.Filename; }
        }

        /// <summary>
        /// Returns the full path to the Server Manager.
        /// </summary>
        /// <remarks>Is always the location of the main settings file.</remarks>
        public string ServerManagerRoot
        {
            get { return Path.GetDirectoryName(mSettings.Filename); }
        }

        /// <summary>
        /// Returns the full path to the directory to use for cache files.
        /// </summary>
        /// <remarks>
        /// Can be specified relative to ServerManagerRoot or as an absolute path.
        /// Defaults to <code>.\Cache</code>.
        /// </remarks>
        /// <example>.\Cache</example>
        /// <example>C:\MC\Cache</example>
        public string CacheRoot
        {
            get { return mSettings.GetRootedPath(ServerManagerRoot, "CacheRoot", @".\Cache"); }
        }

        /// <summary>
        /// Returns the full path to the folder to use for backups.
        /// </summary>
        /// <remarks>
        /// Can be specified relative to ServerManagerRoot or as an absolute path.
        /// Defaults to <code>.\Backups</code>.
        /// </remarks>
        /// <example>.\Backups</example>
        /// <example>C:\MC\Backups</example>
        /// <example>\\Servername\backups\Minecraft</example>
        public string BackupRoot
        {
            get { return mSettings.GetRootedPath(ServerManagerRoot, "BackupRoot", @".\Backups"); }
        }

        /// <summary>
        /// Returns the full path to the folder where Minecraft is installed.
        /// </summary>
        /// <remarks>
        /// Can be specified relative to ServerManagerRoot or as an absolute path.
        /// Defaults to <code>.\Minecraft</code>.
        /// </remarks>
        /// <example>.\Minecraft</example>
        /// <example>C:\MC\Minecraft</example>
        public string MinecraftRoot
        {
            get { return mSettings.GetRootedPath(ServerManagerRoot, "MinecraftRoot", @".\Minecraft"); }
        }

        /// <summary>
        /// Returns the executable to execute the Java files.
        /// </summary>
        /// <remarks>
        /// Must be either the full path to the executable, 
        /// or the executable name must be in the global system PATH.
        /// Defaults to <code>java.exe</code>.
        /// </remarks>
        /// <example>java.exe</example>
        /// <example>c:\Program Files\Java\jre6\bin\java.exe</example>
        public string JavaExec
        {
            get { return mSettings.GetString("JavaExec", "java.exe"); }
        }

        /// <summary>
        /// Returns the jar to use to launch Minecraft.
        /// </summary>
        /// <remarks>
        /// Must be the name of a Java jar file that is in the MinecraftRoot
        /// Defaults to <code>minecraft_server.jar</code>.
        /// </remarks>
        /// <example>minecraft_server.jar</example>
        /// <example>Minecraft_Mod.jar</example>
        public string ServerJar
        {
            get { return mSettings.GetRootedPath(MinecraftRoot, "ServerJar", "minecraft_server.jar"); }
        }

        /// <summary>
        /// Returns the initial amount of memory, in megabytes, to allocate to the Java heap.
        /// </summary>
        /// <remarks>Defaults to <code>1024</code> (1Gb).</remarks>
        public int JavaHeapInit
        {
            get { return mSettings.GetInt("JavaHeapInit", 1024); }
        }

        /// <summary>
        /// Returns the maximum amount of memory, in megabytes, to allow the Java heap.
        /// </summary>
        /// <remarks>Defaults to <code>1024</code> (1Gb).</remarks>
        public int JavaHeapMax
        {
            get { return mSettings.GetInt("JavaHeapMax", 1024); }
        }

        public string MapRoot
        {
            get { return mSettings.GetRootedPath(ServerManagerRoot, "MapRoot", @".\Maps"); }
        }

        public string GetString(string key, string defaultValue)
        {
            return mSettings.GetString(key, defaultValue);
        }

        public string GetString(string key)
        {
            return mSettings.GetString(key);
        }

        public int GetInt(string key, int defaultValue)
        {
            return mSettings.GetInt(key, defaultValue);
        }

        public int GetInt(string key)
        {
            return mSettings.GetInt(key);
        }

        public bool GetBool(string key, bool defaultValue)
        {
            return mSettings.GetBool(key, defaultValue);
        }

        public bool GetBool(string key)
        {
            return mSettings.GetBool(key);
        }

        public string GetRootedPath(string root, string key, string defaultValue)
        {
            return mSettings.GetRootedPath(root, key, defaultValue);
        }

        public string GetRootedPath(string root, string key)
        {
            return mSettings.GetRootedPath(root, key);
        }
    }

}

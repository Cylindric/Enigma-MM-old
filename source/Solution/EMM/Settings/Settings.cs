using System.IO;

namespace EnigmaMM
{
    /// <summary>
    /// Utility class for handling server manager configuration.
    /// </summary>
    /// <remarks>Should always be statically accessed.</remarks>
    public class Settings
    {
        private static SettingsFile mSettings;

        /// <summary>
        /// Initialises the Settings class and loads the settings from the file specified.
        /// </summary>
        /// <param name="fileName">The full path to the settings file.</param>
        public static void Initialise(string fileName)
        {
            mSettings = new SettingsFile(fileName, '=');
            mSettings.AutoLoad = true;
            mSettings.Load();
        }

        public static bool Loaded
        {
            get { return mSettings != null;}
        }

        #region Server Settings


        /// <summary>
        /// Returns the filename of the currently-used settings file.
        /// </summary>
        public static string Filename
        {
            get { return mSettings.Filename; }
        }


        /// <summary>
        /// Returns the full path to the Server Manager.
        /// </summary>
        /// <remarks>Is always the location of the main settings file.</remarks>
        public static string ServerManagerRoot
        {
            get { return Path.GetDirectoryName(mSettings.Filename); }
        }


        /// <summary>
        /// Returns the IP address of the server to connect to when in client-mode.
        /// </summary>
        /// <remarks>Defaults to <code>localhost</code>.</remarks>
        public static string ClientConnectIp
        {
            get { return mSettings.GetString("ClientConnectIp", "localhost"); }
        }


        /// <summary>
        /// Returns the Port address of the server to connect to when in client-mode.
        /// </summary>
        /// <remarks>Defaults to <code>8221</code>.</remarks>
        public static int ClientConnectPort
        {
            get { return mSettings.GetInt("ClientConnectPort", 8221); }
        }


        /// <summary>
        /// Returns the IP address that the server should listen on when in server-mode.
        /// </summary>
        /// <remarks>
        /// Defaults to <code>none</code>.
        /// Set to <c>any</c> to listen on all interface, <c>none</c> to disable.
        /// </remarks>
        public static string ServerListenIp
        {
            get { return mSettings.GetString("ServerListenIp", "none"); }
        }


        /// <summary>
        /// Returns the Port that the server should listen on when in server-mode.
        /// </summary>
        /// <remarks>Defaults to <code>8221</code>.</remarks>
        public static int ServerListenPort
        {
            get { return mSettings.GetInt("ServerListenPort", 8221); }
        }


        /// <summary>
        /// Returns the Username that the server and client should use to communicate.
        /// </summary>
        /// <remarks>Defaults to the invalid <code>changeme</code>.</remarks>
        public static string ServerUsername
        {
            get { return mSettings.GetString("ServerUsername", "changeme"); }
        }


        /// <summary>
        /// Returns the Password that the server and client should use to communicate.
        /// </summary>
        /// <remarks>Defaults to the invalid <code>changeme</code>.</remarks>
        public static string ServerPassword
        {
            get { return mSettings.GetString("ServerPassword", "changeme"); }
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
        public static string CacheRoot
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
        public static string BackupRoot
        {
            get { return mSettings.GetRootedPath(ServerManagerRoot, "BackupRoot", @".\Backups"); }
        }

        #endregion

        #region Minecraft Server Settings

        /// <summary>
        /// Returns the full path to the folder where Minecraft is installed.
        /// </summary>
        /// <remarks>
        /// Can be specified relative to ServerManagerRoot or as an absolute path.
        /// Defaults to <code>.\Minecraft</code>.
        /// </remarks>
        /// <example>.\Minecraft</example>
        /// <example>C:\MC\Minecraft</example>
        public static string MinecraftRoot
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
        public static string JavaExec
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
        public static string ServerJar
        {
            get { return mSettings.GetRootedPath(MinecraftRoot, "ServerJar", "minecraft_server.jar"); }
        }


        /// <summary>
        /// Returns the initial amount of memory, in megabytes, to allocate to the Java heap.
        /// </summary>
        /// <remarks>Defaults to <code>1024</code> (1Gb).</remarks>
        public static int JavaHeapInit
        {
            get { return mSettings.GetInt("JavaHeapInit", 1024); }
        }

        /// <summary>
        /// Returns the maximum amount of memory, in megabytes, to allow the Java heap.
        /// </summary>
        /// <remarks>Defaults to <code>1024</code> (1Gb).</remarks>
        public static int JavaHeapMax
        {
            get { return mSettings.GetInt("JavaHeapMax", 1024); }
        }

        #endregion

        #region General Map Settings

        public static string MapRoot
        {
            get { return mSettings.GetRootedPath(ServerManagerRoot, "MapRoot", @".\Maps"); }
        }

        public static int OptimisePng
        {
            get { return mSettings.GetInt("OptimisePng", 3); }
        }
        #endregion

        #region AlphaVespucci Settings

        public static bool AlphaVespucciInstalled
        {
            get { return mSettings.GetBool("AlphaVespucciInstalled", false); }
        }

        public static string AlphaVespucciRoot
        {
            get { return mSettings.GetRootedPath(ServerManagerRoot, "AlphaVespucciRoot", @".\AlphaVespucci"); }
        }

        #endregion

        #region Overviewer Settings
        
        public static bool OverviewerInstalled
        {
            get { return mSettings.GetBool("MinecraftOverviewerInstalled", false); }
        }

        public static string OverviewerRoot
        {
            get { return mSettings.GetRootedPath(ServerManagerRoot, "MinecraftOverviewerRoot", @".\Overviewer"); }
        }


        /// <summary>
        /// Returns the full path to the Python executable.
        /// </summary>
        /// <remarks>
        /// Should be a full path, unless python.exe is in the PATH.
        /// Defaults to <code>C:\Program Files\Python27\python.exe</code>.
        /// </remarks>
        /// <example>C:\Program Files\Python27\python.exe</example>
        /// <example>python.exe</example>
        public static string PythonExe
        {
            get { return mSettings.GetString("PythonExe", @"C:\Program Files\Python27\python.exe"); }
        }

        #endregion

    }

}

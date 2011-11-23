using System.IO;
using System.Linq;
using EnigmaMM.Engine;
using System.Data.SqlServerCe;

namespace EnigmaMM
{
    /// <summary>
    /// Utility class for handling server manager configuration.
    /// </summary>
    public class Config
    {
        private EMMServer mServer;
        private string mServerManagerRoot;

        public Config(EMMServer server)
        {
            mServer = server;
            mServerManagerRoot = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase.Substring(8))); // strip the "file:///" from the front of the codebase
        }

        /// <summary>
        /// Returns the full path to the Server Manager.
        /// </summary>
        /// <remarks>Is always the location of the main server manager exec.</remarks>
        public string ServerManagerRoot
        {
            get { return mServerManagerRoot; }
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
            get { return ReadConfigPath("backup_path"); }
        }

        /// <summary>
        /// Returns the full path to the folder to use for data cache.
        /// </summary>
        /// <remarks>
        /// Can be specified relative to ServerManagerRoot or as an absolute path.
        /// Defaults to <code>.\Cache\</code>.
        /// </remarks>
        /// <example>.\Cache</example>
        /// <example>C:\MC\Cache</example>
        /// <example>\\Servername\Cache\samplecache</example>
        public string CacheRoot
        {
            get { return ReadConfigPath("cache_path"); }
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
            get { return ReadConfigPath("minecraft_path"); }
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
            get { return ReadConfigPath("java_exe"); }
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
            get { return Path.Combine(MinecraftRoot, "minecraft_server.jar"); }
        }

        /// <summary>
        /// Returns the initial amount of memory, in megabytes, to allocate to the Java heap.
        /// </summary>
        /// <remarks>Defaults to <code>1024</code> (1Gb).</remarks>
        public int JavaHeapInit
        {
            get { return ReadConfigInt("java_heap_init"); }
        }

        /// <summary>
        /// Returns the maximum amount of memory, in megabytes, to allow the Java heap.
        /// </summary>
        /// <remarks>Defaults to <code>1024</code> (1Gb).</remarks>
        public int JavaHeapMax
        {
            get { return ReadConfigInt("java_heap_max"); }
        }

        public string MapOutputPath
        {
            get { return ReadConfigPath("map_output_path"); }
        }

        public string ReadConfig(string key)
        {
            return Manager.GetContext.Configs.Single(c => c.Key == key).Value;
        }

        public int ReadConfigInt(string key)
        {
            int outputValue = 0;
            int.TryParse(ReadConfig(key), out outputValue);
            return outputValue;
        }

        public string ReadConfigPath(string key)
        {
            string root = mServerManagerRoot;
            string path = ReadConfig(key);
            if (path.StartsWith("."))
            {
                path = Path.Combine(root, path);
                path = Path.GetFullPath(path);
            }
            return path;
        }

    }

}

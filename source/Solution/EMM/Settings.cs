using System.IO;

namespace EnigmaMM
{
    public  class Settings
    {
        private static SettingsFile mSettings;

        public static void Initialise(string fileName)
        {
            mSettings = new SettingsFile(fileName, '=');
            mSettings.Load();
        }

        #region Server Settings
        
        public static string ServerRoot
        {
            get { return Path.GetFullPath(mSettings.GetString("ServerRoot", @"..\")); }
        }

        public static string ServerIp
        {
            get { return mSettings.GetString("ServerIp", "localhost"); }
        }

        public static int ServerPort
        {
            get { return mSettings.GetInt("ServerPort", 8221); }
        }

        public static string ServerUsername
        {
            get { return mSettings.GetString("ServerUsername", "changeme"); }
        }

        public static string ServerPassword
        {
            get { return mSettings.GetString("ServerPassword", "changeme"); }
        }

        public static string CacheRoot
        {
            get { return mSettings.GetRootedPath(ServerRoot, "CacheRoot", @".\Cache"); }
        }

        #endregion

        #region General Map Settings

        public static string MapRoot
        {
            get { return mSettings.GetRootedPath(ServerRoot, "MapRoot", @".\Maps"); }
        }

        public static int OptimisePng
        {
            get { return mSettings.GetInt("OptimisePng", 3); }
        }
        #endregion

        #region Minecraft Server Settings

        public static string MinecraftRoot
        {
            get { return mSettings.GetRootedPath(ServerRoot, "MinecraftRoot", @".\Minecraft"); }
        }

        public static string JavaExec {
            get { return mSettings.GetString("JavaExec", "java.exe"); }
        }

        public static string ServerJar
        {
            get { return mSettings.GetString("ServerJar", "minecraft_server.jar"); }
        }

        public static int JavaHeapInit
        {
            get { return mSettings.GetInt("JavaHeapInit", 1024); }
        }

        public static int JavaHeapMax
        {
            get { return mSettings.GetInt("JavaHeapMax", 1024); }
        }

        #endregion

        #region AlphaVespucci Settings

        public static bool AlphaVespucciInstalled
        {
            get { return mSettings.GetBool("AlphaVespucciInstalled", false); }
        }

        public static string AlphaVespucciRoot
        {
            get { return mSettings.GetRootedPath(ServerRoot, "AlphaVespucciRoot", @".\AlphaVespucci"); }
        }

        #endregion

        #region Overviewer Settings
        
        public static bool OverviewerInstalled
        {
            get { return mSettings.GetBool("MinecraftOverviewerInstalled", false); }
        }

        public static string OverviewerRoot
        {
            get { return mSettings.GetRootedPath(ServerRoot, "MinecraftOverviewerRoot", @".\Overviewer"); }
        }

        public static string PythonExe
        {
            get { return mSettings.GetRootedPath(ServerRoot, "PythonExe", @"C:\Program Files\Python27\python.exe"); }
        }

        #endregion

    }

}

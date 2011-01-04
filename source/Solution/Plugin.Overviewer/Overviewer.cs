using System.Diagnostics;
using System.IO;
using EnigmaMM.Interfaces;

namespace EnigmaMM.Plugin.Implementation
{
    class Overviewer
    {
        private IServer mServer;

        public Overviewer(IServer server)
        {
            mServer = server;
        }

        public void RenderMap()
        {
            string exePath;
            string outputPath;
            string cachePath;
            string worldPath;

            mServer.RaiseServerMessage("OVERVIEWER: Creating map");

            exePath = mServer.Settings.GetRootedPath(mServer.Settings.ServerManagerRoot, "MinecraftOverviewerRoot");
            exePath = Path.Combine(exePath, "gmap.exe");
            outputPath = Path.Combine(mServer.Settings.MapRoot, "Overviewer");
            cachePath = Path.Combine(mServer.Settings.CacheRoot, "overviewer");
            worldPath = mServer.MinecraftSettings.WorldPath;

            if (!Directory.Exists(mServer.Settings.CacheRoot))
            {
                throw new DirectoryNotFoundException("Cache path missing: " + mServer.Settings.CacheRoot);
            }

            if (!Directory.Exists(worldPath))
            {
                throw new DirectoryNotFoundException("World path missing: " + worldPath);
            }

            if (!Directory.Exists(cachePath))
            {
                Directory.CreateDirectory(cachePath);
            }

            string cmd = string.Format(
                "-p 1 --cachedir \"{0}\" \"{1}\" \"{2}\"",
                cachePath, mServer.MinecraftSettings.WorldPath, outputPath
            );

            Process p = new Process();
            p.StartInfo.FileName = exePath;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = false;
            p.StartInfo.Arguments = cmd;
            p.Start();
            p.PriorityClass = ProcessPriorityClass.BelowNormal;
            p.WaitForExit();

            mServer.RaiseServerMessage("Map complete.");
        }

    }
}
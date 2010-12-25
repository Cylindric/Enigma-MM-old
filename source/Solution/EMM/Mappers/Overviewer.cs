using System.IO;
using System.Diagnostics;
using System.Text;
using System.Collections.Generic;
using EnigmaMM.Interfaces;

namespace EnigmaMM.Mappers
{
    class Overviewer : Mapper
    {
        public Overviewer(IServer server) : base(server, "overviewer")
        {
            mExePath = server.Settings.GetRootedPath(server.Settings.ServerManagerRoot, "MinecraftOverviewerRoot");
            mExePath = Path.Combine(mExePath, "gmap.exe");
            mOutputPath = Path.Combine(mServer.Settings.MapRoot, "Overviewer");
        }

        public override void Render()
        {
            RenderMap();
        }

        public override void Render(string type)
        {
            RenderMap();
        }

        protected override void RenderMap()
        {
            base.RenderMap();
            mServer.RaiseServerMessage("OVERVIEWER: Creating map");

            if (!Directory.Exists(mServer.Settings.CacheRoot))
            {
                throw new DirectoryNotFoundException("Cache path missing: " + mServer.Settings.CacheRoot);
            }
            if (!Directory.Exists(mCachePath))
            {
                Directory.CreateDirectory(mCachePath);
            }

            string cmd = string.Format(
                "-p 1 --cachedir \"{0}\" \"{1}\" \"{2}\"",
                mCachePath, mServer.MinecraftSettings.WorldPath, mOutputPath
            );

            Process p = new Process();
            p.StartInfo.FileName = mExePath;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = false;
            p.StartInfo.Arguments = cmd;
            p.Start();
            p.PriorityClass = ProcessPriorityClass.BelowNormal;
            p.WaitForExit();

            mServer.RaiseServerMessage("OVERVIEWER: Done.");
        }

    }
}
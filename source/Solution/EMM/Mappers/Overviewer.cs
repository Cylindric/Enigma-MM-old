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
            mExePath = Path.Combine(Settings.OverviewerRoot, "gmap.exe");
            mOutputPath = Path.Combine(Settings.MapRoot, "Overviewer");
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
            mMinecraft.RaiseServerMessage("OVERVIEWER: Creating map");

            if (!Directory.Exists(Settings.CacheRoot))
            {
                throw new DirectoryNotFoundException("Cache path missing: " + Settings.CacheRoot);
            }
            if (!Directory.Exists(mCachePath))
            {
                Directory.CreateDirectory(mCachePath);
            }

            string cmd = string.Format(
                "-p 1 --cachedir \"{0}\" \"{1}\" \"{2}\"",
                mCachePath, mMinecraft.ServerProperties.WorldPath, mOutputPath
            );

            Process p = new Process();
            p.StartInfo.FileName = mExePath;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = false;
            p.StartInfo.Arguments = cmd;
            p.Start();
            p.PriorityClass = ProcessPriorityClass.BelowNormal;
            p.WaitForExit();

            mMinecraft.RaiseServerMessage("OVERVIEWER: Done.");
        }

    }
}
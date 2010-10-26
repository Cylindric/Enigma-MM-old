using System.IO;
using System.Diagnostics;

namespace EnigmaMM
{
    class Overviewer : Mapper
    {
        public Overviewer(MCServer server) : base(server, "overviewer")
        {
            mExePath = Config.OverviewerRoot;
            mExePath = Path.Combine(mExePath, "gmap.py");
            mCachePath = Path.Combine(Config.CacheRoot, "Overviewer");
        }


        public override void RenderMaps()
        {
            base.RenderMaps();
            mMinecraft.RaiseServerMessage("OVERVIEWER: Creating map");

            if (!Directory.Exists(Config.CacheRoot))
            {
                throw new DirectoryNotFoundException("Cache path missing: " + Config.CacheRoot);
            }
            if (!Directory.Exists(mCachePath))
            {
                Directory.CreateDirectory(mCachePath);
            }

            string cmd = string.Format(
                "\"{0}\" -p 1 --cachedir \"{1}\" \"{2}\" \"{3}\"",
                mExePath, mCachePath, mMinecraft.ServerProperties.WorldPath, mOutputPath
            );

            Process p = new Process();
            p.StartInfo.FileName = Config.PythonExe;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.Arguments = cmd;
            p.Start();
            p.WaitForExit();

            mMinecraft.RaiseServerMessage("OVERVIEWER: Done.");
        }
    }
}
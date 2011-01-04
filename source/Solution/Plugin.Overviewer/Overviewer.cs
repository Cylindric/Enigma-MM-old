using System.Diagnostics;
using System.IO;
using EnigmaMM.Interfaces;
using Interfaces.BaseClasses;

namespace EnigmaMM.Plugin.Implementation
{
    public class Overviewer : PluginMapper
    {
        public Overviewer()
        {
            base.Name = "Overviewer";
            base.Tag = "overviewer";
        }

        public override void Initialise(IServer server)
        {
            base.Initialise(server);

            ExePath = server.Settings.GetRootedPath(server.Settings.ServerManagerRoot, "MinecraftOverviewerRoot");
            ExePath = Path.Combine(ExePath, "gmap.exe");
        }

        public override void Render()
        {
            VerifyPath(CachePath, false);
            VerifyPath(OutputPath, false);

            string cache = Path.Combine(CachePath, Tag);
            string output = Path.Combine(OutputPath, Tag);

            VerifyPath(cache, true);
            VerifyPath(output, true);

            string cmd = string.Format(
                "-p 1 --cachedir \"{0}\" \"{1}\" \"{2}\"",
                cache, WorldPath, output
            );

            Process p = new Process();
            p.StartInfo.FileName = ExePath;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = false;
            p.StartInfo.Arguments = cmd;
            p.Start();
            p.PriorityClass = ProcessPriorityClass.BelowNormal;
            p.WaitForExit();
        }
    }
}
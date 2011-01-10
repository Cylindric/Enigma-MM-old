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
        }

        public override void Render()
        {
            ExePath = PluginSettings.GetRootedPath(Server.Settings.ServerManagerRoot, "ExePath", @".\Overviewer\gmap.exe");
            if (!File.Exists(ExePath))
            {
                Server.RaiseServerMessage("Minecraft Overviewer not found.  Expected in {0}", ExePath);
                return;
            }

            Server.RaiseServerMessage("{0}: Rendering map...", this.Name);

            VerifyPath(CachePath, false);
            VerifyPath(OutputPath, false);

            ExePath = PluginSettings.GetRootedPath(Server.Settings.ServerManagerRoot, "ExePath", @".\Overviewer\gmap.exe");

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

            Server.RaiseServerMessage("{0}: Done.", this.Name);
        }
    }
}
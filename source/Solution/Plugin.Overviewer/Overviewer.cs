using System.Diagnostics;
using System.IO;
using EnigmaMM.Interfaces;
using Interfaces.BaseClasses;

namespace EnigmaMM.Plugin.Implementation
{
    public class Overviewer : MapperPlugin
    {
        public Overviewer()
        {
            base.Name = "Overviewer";
            base.Tag = "overviewer";
        }

        public override void Render()
        {
            // Get and check that the Executable exists
            string exeFile = PluginSettings.GetRootedPath(Server.Settings.ServerManagerRoot, "ExePath", @".\Overviewer\gmap.exe");
            if (!File.Exists(exeFile))
            {
                Server.RaiseServerMessage("Minecraft Overviewer not found.  Expected in {0}", exeFile);
                return;
            }

            // Check the world data exists
            VerifyPath(WorldPath, false);

            // Check the output path
            VerifyPath(Path.GetDirectoryName(OutputPath), false);
            VerifyPath(OutputPath, true);

            // Check the cache path
            VerifyPath(Path.GetDirectoryName(CachePath), false);
            VerifyPath(CachePath, true);

            Server.RaiseServerMessage("{0}: Rendering map...", this.Name);

            string cmd = string.Format(
                "-p 1 --cachedir \"{0}\" \"{1}\" \"{2}\"",
                CachePath, WorldPath, OutputPath
            );

            Process p = new Process();
            p.StartInfo.FileName = exeFile;
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
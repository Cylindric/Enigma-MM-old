using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnigmaMM.Interfaces;
using Interfaces.BaseClasses;
using System.IO;
using System.Diagnostics;

namespace EnigmaMM.Plugin.Implementation
{
    public class c10t : PluginMapper
    {
        public c10t()
        {
            base.Name = "c10t";
            base.Tag = "c10t";
        }

        public override void Render()
        {
            // Get and check that the Executable exists
            string exeFile = PluginSettings.GetRootedPath(Server.Settings.ServerManagerRoot, "ExePath", @".\c10t\c10t.exe");
            if (!File.Exists(exeFile))
            {
                Server.RaiseServerMessage("c10t not found.  Expected in {0}", exeFile);
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
                "--world \"{0}\" --output \"{1}\"",
                WorldPath, OutputPath
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
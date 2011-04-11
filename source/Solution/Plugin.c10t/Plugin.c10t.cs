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
            Render("main");
        }

        public override void Render(params string[] args)
        {
            // If no args specified, just run the default task
            if (args.Length == 0)
            {
                Render("main");
            }

            foreach (string arg in args)
            {
                RenderMap(arg);
            }
        }


        private void RenderMap(string type) 
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

            string OutputFile = Path.Combine(OutputPath, type+".png");

            // Check the core cache path
            VerifyPath(Path.GetDirectoryName(CachePath), false);
            VerifyPath(CachePath, true);

            Server.RaiseServerMessage("{0}: Rendering map...", this.Name);

            string renderParameters = PluginSettings.GetString(type + ".RenderParams", "--isometric --show-players");
            string cmd = string.Format(
                "--world \"$WORLD\" --output \"$OUTPUTFILE\" --cache-dir \"$CACHE\" --cache-key {0} {1}",
                type, renderParameters
            );

            cmd = cmd.Replace("$WORLD", WorldPath);
            cmd = cmd.Replace("$CACHE", CachePath);
            cmd = cmd.Replace("$OUTPUTPATH", OutputPath);
            cmd = cmd.Replace("$OUTPUTFILE", OutputFile);

            Process p = new Process();
            p.StartInfo.FileName = exeFile;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = false;
            p.StartInfo.Arguments = cmd;
            p.Start();
            p.PriorityClass = ProcessPriorityClass.BelowNormal;
            p.WaitForExit();

            int smallWidth = PluginSettings.GetInt(type + ".smallWidth", 250);
            string smallFile = Path.Combine(OutputPath, type + "-small.png");
            this.Resize(OutputFile, smallFile, smallWidth);

            Server.RaiseServerMessage("{0}: Done.", this.Name);
        }
    }
    
}
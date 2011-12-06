using System.Diagnostics;
using System.IO;

namespace EnigmaMM.Engine.Commands.Mappers
{
    public class Overviewer : Mapper
    {
        public override void RenderMap()
        {
            EMMServer Server = Manager.Server;

            string OutputPath = Server.Settings.MapOutputPath;
            if (OutputPath.StartsWith("."))
            {
                OutputPath = Path.Combine(Server.Settings.ServerManagerRoot, OutputPath);
                OutputPath = Path.GetFullPath(OutputPath);
            }
            OutputPath = Path.Combine(OutputPath, "overviewer");


            string CachePath = Server.Settings.CacheRoot;
            if (CachePath.StartsWith("."))
            {
                CachePath = Path.Combine(Server.Settings.ServerManagerRoot, CachePath);
                CachePath = Path.GetFullPath(CachePath);
            }
            CachePath = Path.Combine(CachePath, "overviewer");


            string exeFile = Server.Settings.ReadConfigPath("overviewer_exe");
            if (exeFile.StartsWith("."))
            {
                exeFile = Path.Combine(Server.Settings.ServerManagerRoot, exeFile);
                exeFile = Path.GetFullPath(exeFile);
                exeFile = string.Format("cmd");
            }
            if (!File.Exists(exeFile))
            {
                Server.RaiseServerMessage("Overviewer not found.  Expected in {0}", exeFile);
                return;
            }

            // Check the world exists
            string WorldPath = Server.MinecraftSettings.WorldPath;
            if (!Directory.Exists(WorldPath))
            {
                throw new DirectoryNotFoundException("World path missing: " + WorldPath);
            }

            // Check the output path
            if (!Directory.Exists(Path.GetDirectoryName(OutputPath)))
            {
                throw new DirectoryNotFoundException("Output path missing: " + Path.GetDirectoryName(OutputPath));
            }
            if (!Directory.Exists(OutputPath))
            { 
                Directory.CreateDirectory(OutputPath);
            }

            Server.RaiseServerMessage("Overviewer: Rendering map...");

            string cmd = string.Format(
                "\"$WORLD\" " +
                "\"$OUTPUTPATH\" "
            );

            cmd = cmd.Replace("$EXEPATH", Path.GetDirectoryName(exeFile));
            cmd = cmd.Replace("$WORLD", WorldPath);
            cmd = cmd.Replace("$OUTPUTPATH", OutputPath);

            Process p = new Process();
            p.StartInfo.FileName = exeFile;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = false;
            p.StartInfo.Arguments = cmd;
            p.Start();
            p.PriorityClass = ProcessPriorityClass.BelowNormal;
            p.WaitForExit();

            Server.RaiseServerMessage("overviewer: Done.");
        }

    }
    
}
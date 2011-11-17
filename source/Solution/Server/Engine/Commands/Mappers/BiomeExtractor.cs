using System.Diagnostics;
using System.IO;

namespace EnigmaMM.Engine.Commands.Mappers
{
    public class BiomeExtractor : Mapper
    {
        public override void RenderMap()
        {
            EMMServer Server = Manager.Server;

            if (Server.CurrentStatus != Status.Stopped)
            {
                Server.RaiseServerMessage("BiomeExtractor cannot run - server is running");
                return;
            }

            string exeFile = Server.Settings.ReadConfigPath("biomeextractor_exe");
            if (exeFile.StartsWith("."))
            {
                exeFile = Path.Combine(Server.Settings.ServerManagerRoot, exeFile);
                exeFile = Path.GetFullPath(exeFile);
            }
            if (!File.Exists(exeFile))
            {
                Server.RaiseServerMessage("BiomeExtractor not found.  Expected in {0}", exeFile);
                return;
            }

            // Check the world exists
            string WorldPath = Server.MinecraftSettings.WorldPath;
            if (!Directory.Exists(WorldPath))
            {
                throw new DirectoryNotFoundException("World path missing: " + WorldPath);
            }

            Server.RaiseServerMessage("BiomeExtractor: Updating hints...");

            string cmd = string.Format(
                "-jar \"$EXEPATH\" " +
                "-nogui " +
                "\"$WORLD\" "
            );

            cmd = cmd.Replace("$EXEPATH", exeFile);
            cmd = cmd.Replace("$WORLD", WorldPath);

            Process p = new Process();
            p.StartInfo.FileName = "java.exe";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = false;
            p.StartInfo.Arguments = cmd;
            p.Start();
            p.PriorityClass = ProcessPriorityClass.BelowNormal;
            p.WaitForExit();

            Server.RaiseServerMessage("BiomeExtractor: Done.");
        }

    }

}
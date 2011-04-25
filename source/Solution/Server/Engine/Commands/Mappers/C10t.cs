using System.Diagnostics;
using System.IO;
using System.Drawing;

namespace EnigmaMM.Engine.Commands.Mappers
{
    public class C10t
    {
        public C10t()
        {
        }

        public void RenderMap()
        {
            EMMServer Server = Manager.Server;

            string OutputPath = Server.ReadConfig("map_output");
            if (OutputPath.StartsWith("."))
            {
                OutputPath = Path.Combine(Server.Settings.ServerManagerRoot, OutputPath);
                OutputPath = Path.GetFullPath(OutputPath);
            }
            OutputPath = Path.Combine(OutputPath, "c10t");


            string CachePath = Server.ReadConfig("map_cache");
            if (CachePath.StartsWith("."))
            {
                CachePath = Path.Combine(Server.Settings.ServerManagerRoot, CachePath);
                CachePath = Path.GetFullPath(CachePath);
            }
            CachePath = Path.Combine(CachePath, "c10t");


            string exeFile = Server.ReadConfig("c10t_exe");
            if (exeFile.StartsWith("."))
            {
                exeFile = Path.Combine(Server.Settings.ServerManagerRoot, exeFile);
                exeFile = Path.GetFullPath(exeFile);
            }
            if (!File.Exists(exeFile))
            {
                Server.RaiseServerMessage("c10t not found.  Expected in {0}", exeFile);
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


            // Check the core cache path
            if (!Directory.Exists(Path.GetDirectoryName(CachePath)))
            {
                throw new DirectoryNotFoundException("Cache path missing: " + Path.GetDirectoryName(CachePath));
            }
            if (!Directory.Exists(CachePath))
            {
                Directory.CreateDirectory(CachePath);
            }

            Server.RaiseServerMessage("c10t: Rendering map...");

            string OutputFile = Path.Combine(OutputPath, "map.png");
            string cmd = string.Format(
                "--world \"$WORLD\" " +
                "--ttf-path \"$EXEPATH\\font.ttf\" " +
                "--output \"$OUTPUTFILE\" " +
                "--cache-dir \"$CACHE\" " +
                "--cache-key map " +
                "--isometric " +
                "--show-players");

            cmd = cmd.Replace("$EXEPATH", Path.GetDirectoryName(exeFile));
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

            int smallWidth = 0;
            int.TryParse(Server.ReadConfig("map_small_width"), out smallWidth);
            string smallFile = Path.Combine(OutputPath, "map-small.png");
            Resize(OutputFile, smallFile, smallWidth);

            Server.RaiseServerMessage("c10t: Done.");
        }


        protected void Resize(string InputFile, string OutputFile, int destWidth)
        {
            if (!File.Exists(InputFile) || !Directory.Exists(Path.GetDirectoryName(OutputFile)))
            {
                return;
            }

            Bitmap input = new Bitmap(InputFile);

            int sourceWidth = input.Width;
            int sourceHeight = input.Height;

            float ratio = (float)destWidth / (float)sourceWidth;
            int destHeight = (int)(sourceHeight * ratio);

            Bitmap output = new Bitmap(destWidth, destHeight);
            Graphics g = Graphics.FromImage((Image)output);
            g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            g.DrawImage(input, 0, 0, destWidth, destHeight);
            g.Dispose();

            output.Save(OutputFile);
            output.Dispose();
        }
    }
    
}
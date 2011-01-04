using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using EnigmaMM.Interfaces;
using Interfaces.BaseClasses;

namespace EnigmaMM.Plugin.Implementation
{
    public class AlphaVespucci : PluginMapper
    {

        public AlphaVespucci()
        {
            base.Name = "AlphaVespucci";
            base.Tag = "av";
        }

        public override void Initialise(IServer server)
        {
            base.Initialise(server);

            ExePath = server.Settings.GetRootedPath(server.Settings.ServerManagerRoot, "AlphaVespucciRoot");
            ExePath = Path.Combine(ExePath, "alphavespucci.exe");
        }

        public override void Render()
        {
            Render("main");
        }

        public override void Render(params string[] args)
        {
            if (args[0] == "main")
            {
                RenderMap("obleft", "day", "mainmap", true);
            }
            if (args[0] == "extra")
            {
                RenderMap("obleft", "night", "nightmap", false);
                RenderMap("obleft", "cave", "caves", false);
                RenderMap("obleft", "cavelimit 15", "surfacecaves", false);
                RenderMap("obleft", "whitelist \"Diamond ore\"", "resource-diamond", false);
                RenderMap("obleft", "whitelist \"Redstone ore\"", "resource-redstone", false);
                RenderMap("obleft", "night -whitelist \"Torch\"", "resource-torch", false);
                RenderMap("flat", "day", "flatmap", false);
            }
        }

        private void RenderMap(string display, string features, string Filename, bool createHistory)
        {
            Server.RaiseServerMessage(string.Format("AV: Rendering map {0}...", display));

            VerifyPath(WorldPath, false);
            VerifyPath(OutputPath, false);

            string output = Path.Combine(OutputPath, Tag);
            VerifyPath(output, true);

            string cmd = string.Format(
                "-{0} -{1} -path \"{2}\" -fullname \"{4}\" -outputdir \"{3}\"",
                display, features, WorldPath, output, Filename
            );

            Process p = new Process();
            p.StartInfo.FileName = ExePath;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.Arguments = cmd;
            p.Start();
            p.PriorityClass = ProcessPriorityClass.BelowNormal;
            p.WaitForExit();

            // fullFilename now exists, and is the full-size PNG.
            // Optimise it and save a JPEG version
            string fullFilenamePng = Path.Combine(output, Filename + ".png");
            string fullFilenameJpeg = Path.Combine(output, Filename + ".jpg");
            OptimisePNG(fullFilenamePng);
            ToJpeg(fullFilenamePng);

            // save a thumbnail version as a JPEG
            string smallFilenamePng = Path.Combine(output, Filename + "-small.png");
            Resize(fullFilenamePng, smallFilenamePng, 480);
            ToJpeg(smallFilenamePng);
            File.Delete(smallFilenamePng);

            // save a history version
            if (createHistory)
            {
                string HistoryRoot = Path.Combine(output, "History");
                string HistoryFile = Path.Combine(HistoryRoot, string.Format("{0}-{1:yyyy-MM-dd_HH}.jpg", Path.GetFileNameWithoutExtension(fullFilenameJpeg), DateTime.Now));
                if (!Directory.Exists(HistoryRoot))
                {
                    Directory.CreateDirectory(HistoryRoot);
                }
                File.Copy(fullFilenameJpeg, HistoryFile, true);
            }

            Server.RaiseServerMessage("AV: Done.");
        }

        private void ToJpeg(string InputFile)
        {
            string OutputFile = Path.Combine(Path.GetDirectoryName(InputFile), Path.GetFileNameWithoutExtension(InputFile) + ".jpg");
            ToJpeg(InputFile, OutputFile);
        }

        private void ToJpeg(string InputFile, string OutputFile)
        {
            Bitmap input = new Bitmap(InputFile);

            Encoder qualityEncoder = Encoder.Quality;
            long quality = 80;
            EncoderParameter ratio = new EncoderParameter(qualityEncoder, quality);
            EncoderParameters codecParams = new EncoderParameters(1);
            codecParams.Param[0] = ratio;
            ImageCodecInfo jpegCodecInfo = GetEncoderInfo("image/jpeg");
            input.Save(OutputFile, jpegCodecInfo, codecParams);
            input.Dispose();
        }

        private void Resize(string InputFile, string OutputFile, int destWidth)
        {
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

        private void OptimisePNG(string InputFile)
        {
            if (OptimiseLevel > 0)
            {
                Process p = new Process();
                p.StartInfo.FileName = "optipng.exe";
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.Arguments = string.Format("-v -o{1} \"{0}\" ", InputFile, OptimiseLevel);
                p.Start();
                p.PriorityClass = ProcessPriorityClass.BelowNormal;
                p.WaitForExit();
            }
        }

        private ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            for (int i = 0; i < encoders.Length; i++)
            {
                if (encoders[i].MimeType == mimeType)
                {
                    return encoders[i];
                }
            }
            return null;
        }

    }
}

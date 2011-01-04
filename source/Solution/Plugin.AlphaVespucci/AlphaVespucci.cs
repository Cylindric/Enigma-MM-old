using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using EnigmaMM.Interfaces;

namespace EnigmaMM.Plugin.Implementation
{
    public class AlphaVespucci
    {
        private IServer mServer;
        private string mExePath;
        private string mOutputPath;
        private string mWorldPath;
        private int mOptimise = 0;
        private string mTag = 'av';

        public AlphaVespucci(IServer server)
        {
            mServer = server;
        }

        public void RenderMap()
        {
            mServer.RaiseServerMessage("AV: Creating map");
            mServer.RaiseServerMessage("Map complete.");
        }

        public void Render(string type)
        {
            if (type == "main")
            {
                RenderMap("obleft", "day", "mainmap", true);
            }
            if (type == "extra")
            {
                RenderMap("obleft", "night", "nightmap");
                RenderMap("obleft", "cave", "caves");
                RenderMap("obleft", "cavelimit 15", "surfacecaves");
                RenderMap("obleft", "whitelist \"Diamond ore\"", "resource-diamond");
                RenderMap("obleft", "whitelist \"Redstone ore\"", "resource-redstone");
                RenderMap("obleft", "night -whitelist \"Torch\"", "resource-torch");
                RenderMap("flat", "day", "flatmap");
            }
        }

        private void RenderMap(string display, string features, string Filename)
        {
            RenderMap(display, features, Filename, false);
        }

        private void RenderMap(string display, string features, string Filename, bool createHistory)
        {
            string output = Path.Combine(mOutputPath, mTag);

            if (!Directory.Exists(mServer.MinecraftSettings.WorldPath))
            {
                throw new DirectoryNotFoundException("World path missing: " + mServer.MinecraftSettings.WorldPath);
            }
            if (!Directory.Exists(mOutputPath))
            {
                throw new DirectoryNotFoundException("Map output path missing: " + mOutputPath);
            }
            if (!Directory.Exists(output))
            {
                Directory.CreateDirectory(output);
            }
            mServer.RaiseServerMessage(string.Format("AV: Creating map {0} {1}...", display, features));

            string cmd = string.Format(
                "-{0} -{1} -path \"{2}\" -fullname \"{4}\" -outputdir \"{3}\"",
                display, features, mServer.MinecraftSettings.WorldPath, output, Filename
            );

            Process p = new Process();
            p.StartInfo.FileName = mExePath;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.Arguments = cmd;
            p.Start();
            p.PriorityClass = ProcessPriorityClass.BelowNormal;
            p.WaitForExit();

            // fullFilename now exists, and is the full-size PNG.
            // Optimise it and save a JPEG version
            string fullFilenamePng = Path.Combine(mOutputPath, Filename + ".png");
            string fullFilenameJpeg = Path.Combine(mOutputPath, Filename + ".jpg");
            OptimisePNG(fullFilenamePng);
            ToJpeg(fullFilenamePng);

            // save a thumbnail version as a JPEG
            string smallFilenamePng = Path.Combine(mOutputPath, Filename + "-small.png");
            Resize(fullFilenamePng, smallFilenamePng, 480);
            ToJpeg(smallFilenamePng);
            File.Delete(smallFilenamePng);

            // save a history version
            if (createHistory)
            {
                string HistoryRoot = Path.Combine(mOutputPath, "History");
                string HistoryFile = Path.Combine(HistoryRoot, string.Format("{0}-{1:yyyy-MM-dd_HH}.jpg", Path.GetFileNameWithoutExtension(fullFilenameJpeg), DateTime.Now));
                if (!Directory.Exists(HistoryRoot))
                {
                    Directory.CreateDirectory(HistoryRoot);
                }
                File.Copy(fullFilenameJpeg, HistoryFile, true);
            }

            mServer.RaiseServerMessage("AV: Done.");
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
            if (mOptimise > 0)
            {
                Process p = new Process();
                p.StartInfo.FileName = "optipng.exe";
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.Arguments = string.Format("-v -o{1} \"{0}\" ", InputFile, mOptimise);
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

using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System;

namespace EnigmaMM
{
    class AlphaVespucci : Mapper
    {
        public AlphaVespucci(MCServer server) : base(server, "alphavespucci")
        {
            mExePath = Path.Combine(Config.AlphaVespucciRoot, "AlphaVespucci.exe");
        }


        public void RenderMaps(string display, string features, string Filename)
        {
            RenderMaps(display, features, Filename, false);
        }


        public void RenderMaps(string display, string features, string Filename, bool createHistory)
        {
            base.RenderMaps();
            mMinecraft.RaiseServerMessage(string.Format("AV: Creating map {0} {1}...", display, features));

            string cmd = string.Format(
                "-{0} -{1} -path \"{2}\" -fullname \"{4}\" -outputdir \"{3}\"",
                display, features, mMinecraft.ServerProperties.WorldPath, mOutputPath, Filename
            );

            Process p = new Process();
            p.StartInfo.FileName = mExePath;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.Arguments = cmd;
            p.Start();
            p.WaitForExit();

            // save a JPEG-version
            ToJpeg(Path.Combine(mOutputPath, Filename + ".png"), Path.Combine(mOutputPath, Filename + ".jpg"));

            // save a history version
            string HistoryRoot = Path.Combine(mOutputPath, "History");
            if (!Directory.Exists(HistoryRoot))
            {
                Directory.CreateDirectory(HistoryRoot);
            }
            string HistoryFile = Path.Combine(HistoryRoot, string.Format("-{1:yyyy-MM-dd_HH}.jpg", Filename, DateTime.Now));
            File.Copy(Path.Combine(mOutputPath, Filename + ".jpg"), HistoryFile, true);

            mMinecraft.RaiseServerMessage("AV: Done.");
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

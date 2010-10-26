using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace EnigmaMM
{
    class AlphaVespucci
    {
        private MCServer mMinecraft;
        private string mExePath;


        public AlphaVespucci(MCServer server)
        {
            mMinecraft = server;
            mExePath = Config.AlphaVespucciRoot;
            mExePath = Path.Combine(mExePath, "AlphaVespucci.exe");
        }


        public void RenderMap(string display, string features, string Filename, string Filepath)
        {
            mMinecraft.RaiseServerMessage(string.Format("AV: Creating map {0} {1}...", display, features));

            if (!Directory.Exists(mMinecraft.ServerProperties.WorldPath))
            {
                throw new DirectoryNotFoundException("World path missing: " + mMinecraft.ServerProperties.WorldPath);
            }
            if (!Directory.Exists(Filepath))
            {
                throw new DirectoryNotFoundException("Map output path missing: " + Filepath);
            }

            string cmd = string.Format(
                "-{0} -{1} -path \"{2}\" -fullname \"{4}\" -outputdir \"{3}\"",
                display, features, mMinecraft.ServerProperties.WorldPath, Filepath, Filename
            );

            Process p = new Process();
            p.StartInfo.FileName = mExePath;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.Arguments = cmd;
            p.Start();
            p.WaitForExit();

            // save a JPEG-version
            ToJpeg(Path.Combine(Filepath, Filename + ".png"), Path.Combine(Filepath, Filename + ".jpg"));

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

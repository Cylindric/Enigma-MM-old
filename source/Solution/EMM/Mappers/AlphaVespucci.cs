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
        private string mOutputPath;


        public AlphaVespucci(MCServer server)
        {
            mMinecraft = server;
            mExePath = Config.AlphaVespucciRoot;
            mExePath = Path.Combine(mExePath, "AlphaVespucci.exe");
        }


        public void RenderMap(string Mode, string Type, string Filename, string Filepath)
        {
            string cmd = string.Format(
                "-{0} -{1} -path \"{2}\" -fullname \"{4}\" -outputdir \"{3}\"",
                Mode, Type, mMinecraft.ServerProperties.WorldPath,  Filepath, Filename
            );

            Process p = new Process();
            p.StartInfo.FileName = mExePath;
            p.StartInfo.Arguments = cmd;
            p.StartInfo.CreateNoWindow = false;
            p.Start();

            ToJpeg(Path.Combine(Filepath, Filename + ".png"), Path.Combine(Filepath, Filename + ".jpg"));
        }


        private void ToJpeg(string InputFile, string OutputFile)
        {
            EncoderParameter quality = new EncoderParameter(Encoder.Quality, 80);
            ImageCodecInfo jpegCodec = GetEncoderInfo("image/jpeg");

            if (jpegCodec == null)
            {
                return;
            }

            EncoderParameters encoderParams = new EncoderParameters(1);
            encoderParams.Param[0] = quality;

            Bitmap input = new Bitmap(InputFile);
            input.Save(OutputFile, jpegCodec, encoderParams);
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

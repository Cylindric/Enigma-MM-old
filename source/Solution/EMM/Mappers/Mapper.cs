using System.IO;
using System.Diagnostics;

namespace EnigmaMM
{
    class Mapper
    {
        protected MCServer mMinecraft;
        protected string mExePath;
        protected string mCachePath;
        protected string mOutputPath;
        private string mTag = "mapper";

        public Mapper(MCServer server, string tag)
        {
            mTag = tag;

            mMinecraft = server;
            mExePath = Path.Combine(Settings.OverviewerRoot, "gmap.py");
            mCachePath = Path.Combine(Settings.CacheRoot, "Overviewer");
            mOutputPath = Path.Combine(Settings.MapRoot, mTag);
        }


        public virtual void RenderMaps()
        {
            if (!Directory.Exists(mMinecraft.ServerProperties.WorldPath))
            {
                throw new DirectoryNotFoundException("World path missing: " + mMinecraft.ServerProperties.WorldPath);
            }
            if (!Directory.Exists(Settings.MapRoot))
            {
                throw new DirectoryNotFoundException("Map output path missing: " + Settings.MapRoot);
            }
            if (!Directory.Exists(mOutputPath))
            {
                Directory.CreateDirectory(mOutputPath);
            }
        }
    }
}
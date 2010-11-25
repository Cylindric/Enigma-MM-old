using System.IO;
using System.Diagnostics;
using System.Collections.Generic;

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
            mCachePath = Path.Combine(Settings.CacheRoot, mTag);
        }

        public virtual void Render(string type) { }

        public virtual void RenderMap()
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
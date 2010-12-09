using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using EnigmaMM.Interfaces;

namespace EnigmaMM
{
    class Mapper : IMapper
    {
        protected EMMServer mMinecraft;
        protected string mExePath;
        protected string mCachePath;
        protected string mOutputPath;

        private string mTag = "mapper";
        
        public Mapper(EMMServer server, string tag)
        {
            mTag = tag;
            mMinecraft = server;
            mCachePath = Path.Combine(Settings.CacheRoot, mTag);
        }

        public virtual void Render() { }
        public virtual void Render(string type) { }

        protected virtual void RenderMap()
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
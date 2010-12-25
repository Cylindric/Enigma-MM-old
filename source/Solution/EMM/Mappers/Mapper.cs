using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using EnigmaMM.Interfaces;

namespace EnigmaMM.Mappers
{
    class Mapper : IMapper
    {
        protected IServer mServer;
        protected string mExePath;
        protected string mCachePath;
        protected string mOutputPath;

        private string mTag = "mapper";
        
        /// <summary>
        /// Creates a new mapper connected to the specified server, with the 
        /// specified tag identifier.
        /// </summary>
        /// <param name="server">The <see cref="IServer"/> to connect to.</param>
        /// <param name="tag">The identifying tag for the mapper.</param>
        public Mapper(IServer server, string tag)
        {
            mTag = tag;
            mServer = server;
            mCachePath = Path.Combine(mServer.Settings.CacheRoot, mTag);
        }

        /// <summary>
        /// Renders the default map(s) for this renderer.
        /// </summary>
        public virtual void Render() { }

        /// <summary>
        /// Renders the specified map(s) for this renderer based on mapper-specific criteria.
        /// </summary>
        /// <param name="type">type of map to render</param>
        public virtual void Render(string type) { }

        protected virtual void RenderMap()
        {
            if (!Directory.Exists(mServer.MinecraftSettings.WorldPath))
            {
                throw new DirectoryNotFoundException("World path missing: " + mServer.MinecraftSettings.WorldPath);
            }
            if (!Directory.Exists(mServer.Settings.MapRoot))
            {
                throw new DirectoryNotFoundException("Map output path missing: " + mServer.Settings.MapRoot);
            }
            if (!Directory.Exists(mOutputPath))
            {
                Directory.CreateDirectory(mOutputPath);
            }
        }
    }
}
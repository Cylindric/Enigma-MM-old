﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnigmaMM.Interfaces;
using System.IO;

namespace Interfaces.BaseClasses
{
	public abstract class PluginMapper : IMapper
	{
        public string Name { get; set; }
        public string Tag { get; set; }

        public int OptimiseLevel { get; set; }
        protected string WorldPath { get; set; }
        protected string ExePath { get; set; }
        protected string CachePath { get; set; }
        protected string OutputPath { get; set; }
        protected IServer Server { get; set; }

        public PluginMapper()
        {
            Name = "Mapper";
            Tag = "mapper";
            OptimiseLevel = 0;
        }

        public virtual void Initialise(IServer server)
        {
            Server = server;
            WorldPath = server.MinecraftSettings.WorldPath;
            OutputPath = server.Settings.MapRoot;
            CachePath = server.Settings.CacheRoot;
        }

        public virtual void Render(params string[] args)
        {
            Render();
        }

        public abstract void Render();

        protected void VerifyPath(string path, bool create)
        {
            if (!Directory.Exists(path))
            {
                if (create)
                {
                    Directory.CreateDirectory(path);
                }
                else
                {
                    throw new DirectoryNotFoundException("Path missing: " + path);
                }
            }
        }

    }
}
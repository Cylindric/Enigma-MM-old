using System;
using System.Linq;
using EnigmaMM.Interfaces;
using System.Collections.Generic;

namespace EnigmaMM
{
    class MapManager
    {
        private EMMServer mServer;
        private bool mRunning;

        public MapManager(EMMServer server)
        {
            mServer = server;
            mRunning = false;
        }

        public void GenerateMaps(string[] args)
        {
            string specificMapper = "";
            bool mapperFound = false;

            if (mRunning)
            {
                mServer.RaiseServerMessage("Cannot start another mapping task, already mapping.");
                return;
            }

            mRunning = true;

            List<IMapperPlugin> mappers = mServer.Plugins.GetPlugins<IMapperPlugin>();
            if (mappers.Count == 0)
            {
                mServer.RaiseServerMessage("No mapper plugins installed.");
                return;
            }

            // Strip the first element from the args - it's the command itself
            args = args.Where((val, idx) => idx != 0).ToArray();
            
            // Second param, if present, is to limit the mapper to use
            if (args.Count() > 0)
            {
                specificMapper = args[0];
                args = args.Where((val, idx) => idx != 0).ToArray();
            }

            mServer.BlockAutoSave();
            foreach (IMapperPlugin p in mServer.Plugins.GetPlugins<IMapperPlugin>())
            {
                // If a specific mapper was requested, and this isn't it, skip
                if ((specificMapper.Length > 0) && (specificMapper != p.Tag))
                {
                    continue;
                }

                mServer.RaiseServerMessage("Mapping using plugin '{0}'", p.Name);
                try
                {
                    mapperFound = true;
                    p.Render(args);
                }
                catch (Exception e)
                {
                    mServer.RaiseServerMessage("Failed to execute renderer for plugin '{0}', error '{1}'", p.Name, e.Message);
                }
            }
            mServer.UnblockAutoSave();

            if (!mapperFound)
            {
                mServer.RaiseServerMessage("No plugin found with tag '{0}'", specificMapper);
            }

            mRunning = false;
        }
    }
}

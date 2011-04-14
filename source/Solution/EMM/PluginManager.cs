using System.Collections.ObjectModel;
using EnigmaMM.Interfaces;
using System.Reflection;
using System;
using System.Collections;
using System.IO;
using System.Collections.Generic;

namespace EnigmaMM
{
    class PluginManager
    {
        private IServer mServer;
        private Dictionary<string, IPlugin> mPlugins;

        private struct AvailablePlugin
        {
            public string AssemblyPath;
            public string ClassName;
        }

        public PluginManager(IServer server)
        {
            mPlugins = new Dictionary<string, IPlugin>();
            mServer = server;
        }

        public void Load(string path)
        {
            AvailablePlugin[] plugins = FindPlugins(path);
            if ((plugins != null))
            {
                for (int i = 0; i <= plugins.Length - 1; i++)
                {
                    mServer.RaiseServerMessage(string.Format("Loading plugin {0}", plugins[i].ClassName));
                    IPlugin objPlugin = CreateInstance<IPlugin>(plugins[i]);
                    if ((objPlugin != null))
                    {
                        if (mPlugins.ContainsKey(plugins[i].ClassName))
                        {
                            mServer.RaiseServerMessage("Plugin with same name already loaded");
                        }
                        else
                        {
                            mPlugins.Add(plugins[i].ClassName, objPlugin);
                        }
                    }
                }
            }

            // Initialise the plugins
            foreach (KeyValuePair<string, IPlugin> kvp in mPlugins)
            {
                kvp.Value.Initialise(mServer);
            }
        }

        /// <summary>
        /// Return a <c>List</c> of plugins that implement the specified interface.
        /// </summary>
        /// <typeparam name="T">The Interface the plugins should implement.</typeparam>
        /// <returns>A <c>List</c> of plugins of type <c>T</c>.</returns>
        public List<T> GetPlugins<T>()
        {
            List<T> outList = new List<T>();
            foreach (KeyValuePair<string, IPlugin> p in mPlugins)
            {
                if (p.Value.GetType().GetInterface(typeof(T).FullName) != null)
                {
                    outList.Add(((T)p.Value));
                }
            }
            return outList;
        }

        private AvailablePlugin[] FindPlugins(string path)
        {
            ArrayList plugins = new ArrayList();
            string[] assemblyNames = null;
            Assembly assembly = null;

            if (!Directory.Exists(path))
            {
                return null;
            }

            // Go through all DLLs in the directory, attempting to load them
            assemblyNames = Directory.GetFileSystemEntries(path, "*.dll");
            for (int i = 0; i <= assemblyNames.Length - 1; i++)
            {
                try
                {
                    assembly = Assembly.LoadFrom(assemblyNames[i]);
                    ExamineAssembly(assembly, plugins);
                }
                catch (Exception ex)
                {
                    mServer.RaiseServerMessage("Failed to load plugin!");
                    mServer.RaiseServerMessage(ex.Message);
                }
            }

            // Return all the plugins found
            AvailablePlugin[] Results = new AvailablePlugin[plugins.Count];
            if (plugins.Count != 0)
            {
                plugins.CopyTo(Results);
                return Results;
            }
            else
            {
                return null;
            }
        }

        private void ExamineAssembly(Assembly assembly, ArrayList plugins)
        {
            AvailablePlugin plugin = default(AvailablePlugin);

            //Loop through each type in the DLL
            foreach (Type t in assembly.GetTypes())
            {
                //Only look at public types
                if (t.IsPublic == true)
                {
                    //Ignore abstract classes
                    if (!((t.Attributes & TypeAttributes.Abstract) == TypeAttributes.Abstract))
                    {
                        //See if this type implements our interface
                        if (t.GetInterface(typeof(EnigmaMM.Interfaces.IPlugin).FullName) != null)
                        {
                            plugin = new AvailablePlugin();
                            plugin.AssemblyPath = assembly.Location;
                            plugin.ClassName = t.FullName;
                            plugins.Add(plugin);
                        }
                    }
                }
            }
        }

        private T CreateInstance<T>(AvailablePlugin plugin)
        {
            T objPlugin = default(T);

            try
            {
                Assembly objDLL = Assembly.LoadFrom(plugin.AssemblyPath);
                Type t = objDLL.GetType(plugin.ClassName);
                if (t != null)
                {
                    objPlugin = (T)Activator.CreateInstance(t);
                }
            }
            catch (System.Reflection.TargetInvocationException ex)
            {
                mServer.RaiseServerMessage(string.Format("Could not load plugin {0}. {1}", plugin.ClassName, ex.Message));
                return default(T);
            }
            catch (Exception ex)
            {
                mServer.RaiseServerMessage(string.Format("Could not load plugin {0}. {1}", plugin.ClassName, ex.Message));
                return default(T);
            }

            return objPlugin;
        }
    }
}

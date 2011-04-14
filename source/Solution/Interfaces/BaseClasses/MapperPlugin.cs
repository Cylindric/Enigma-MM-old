using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnigmaMM.Interfaces;
using System.IO;

namespace Interfaces.BaseClasses
{
    /// <summary>
    /// A helper base class for implementing IMapper plugins.
    /// </summary>
	public abstract class MapperPlugin : IMapperPlugin
	{
        /// <summary>
        /// Gets and sets the Name of the plugin.
        /// </summary>
        /// <remarks>
        /// This should be set to the name of the plugin, and is 
        /// displayed to users via various messages.
        /// </remarks>
        public string Name { get; set; }

        /// <summary>
        /// Gets and sets the Tag of the plugin.
        /// </summary>
        /// <remarks>
        /// This should be set to a short, file-system safe name.  It 
        /// is used for creating plugin-specific directories in the cache or 
        /// output locations.
        /// </remarks>
        public string Tag { get; set; }

        /// <summary>
        /// The full path to the Minecraft world data.
        /// </summary>
        protected string WorldPath { get; set; }

        /// <summary>
        /// The full path to the Cache location for this plugin.
        /// </summary>
        /// <remarks>This is set by the <see cref="Initialise"/> method from the <c>CachePath</c> value in
        /// the plugin's configuration file, or defaults to a folder named <see cref="Tag"/> 
        /// in the Cache directory under the server root.</remarks>
        protected string CachePath { get; set; }

        /// <summary>
        /// The full path to the Output location for this plugin.
        /// </summary>
        /// <remarks>This is set by the <see cref="Initialise"/> method from the <c>OutputPath</c> value in
        /// the plugin's configuration file, or defaults to a folder named <see cref="Tag"/> 
        /// in the Maps directory under the server root.</remarks>
        protected string OutputPath { get; set; }

        /// <summary>
        /// The <see cref="IServer"/> calling the plugin.
        /// </summary>
        protected IServer Server { get; set; }

        /// <summary>
        /// The <see cref="ISettings"/> configuration for this plugin.
        /// </summary>
        protected ISettings PluginSettings;

        private string mCodeBase;
        private string mSettingsFileName;

        /// <summary>
        /// The default Constructor for the Base Class.
        /// </summary>
        /// <remarks>Sets the <see cref="Name"/> and <see cref="Tag"/> to the filename of the plugin.</remarks>
        public MapperPlugin()
        {
            Name = Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetCallingAssembly().CodeBase);
            mCodeBase = System.Reflection.Assembly.GetCallingAssembly().CodeBase;
            mSettingsFileName = Path.Combine(Path.GetDirectoryName(mCodeBase), Path.GetFileNameWithoutExtension(mCodeBase) + ".conf");
            mSettingsFileName = mSettingsFileName.Substring(mSettingsFileName.IndexOf("\\") + 1);
            Tag = Name;
        }

        /// <summary>
        /// Initialises the plugin and sets up key values.
        /// </summary>
        /// <remarks>
        /// Called by the <c>PluginManager</c> when it is ready to 
        /// initialise the plugin.
        /// </remarks>
        /// <param name="server">The <see cref=">IServer"/> calling the plugin.</param>
        public virtual void Initialise(IServer server)
        {
            Server = server;
            WorldPath = server.MinecraftSettings.WorldPath;

            PluginSettings = server.GetSettings(mSettingsFileName);
            PluginSettings.LookForNewSettings();

            OutputPath = PluginSettings.GetRootedPath(Server.Settings.ServerManagerRoot, "OutputPath", @".\Maps\" + this.Tag);
            CachePath = PluginSettings.GetRootedPath(Server.Settings.ServerManagerRoot, "CachePath", @".\Cache\" + this.Tag);
        }

        /// <summary>
        /// Renders the map(s) with the specified arguments.
        /// </summary>
        /// <remarks>Default defined by the Base Class is to always run the <see cref="Render"/> method.
        /// Override this method in the plugin to implement command arguments for the renderer.</remarks>
        /// <param name="args"></param>
        public virtual void Render(params string[] args)
        {
            Render();
        }

        /// <summary>
        /// Renders the default map(s).
        /// </summary>
        /// <remarks>Must be implemented by the plugin, and should render the default map or maps for the plugin.</remarks>
        public abstract void Render();

        /// <summary>
        /// Verifies that the specified path exists, and optionally creates it if required.
        /// </summary>
        /// <param name="path">The path to verify.</param>
        /// <param name="create">Set to <c>true</c> to attempt creation of missing paths, else set to <c>false</c>.</param>
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

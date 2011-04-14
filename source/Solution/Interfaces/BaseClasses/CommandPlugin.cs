using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnigmaMM.Interfaces;
using System.IO;

namespace Interfaces.BaseClasses
{
    /// <summary>
    /// A helper base class for implementing ICommand plugins.
    /// </summary>
	public abstract class CommandPlugin : ICommandPlugin
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
        public CommandPlugin()
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

            PluginSettings = server.GetSettings(mSettingsFileName);
            PluginSettings.LookForNewSettings();
        }

        /// <summary>
        /// Parse the specified command.
        /// </summary>
        /// <remarks>
        /// Defaults to doing nothing, so should be overriden in the specific plugin implementation.
        /// </remarks>
        /// <param name="command">The command to parse</param>
        /// <returns>true if the plugin actioned the command, else false.</returns>
        public virtual Boolean ParseCommand(string command)
        {
            return false;
        }

    }
}

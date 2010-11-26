using System.IO;

namespace EnigmaMM
{
    /// <summary>
    /// A SettingsFile specifically for handling Minecraft Warp Locations, as defined
    /// by the Hey0 hMod extension.
    /// </summary>
    public class MCServerWarps : SettingsFile
    {
        /// <summary>
        /// Creates a new MCServerWarps configuration object using the specifed warps file.
        /// </summary>
        /// <remarks>
        /// The supplied filename should be just the name of the file.  
        /// It is expected
        /// to be in the MinecraftRoot folder.
        /// </remarks>
        /// <param name="warpsLocation">The name of the warp file.</param>
        public MCServerWarps(string warpsLocation)
            : base(Path.Combine(Settings.MinecraftRoot, warpsLocation), ':')
        {
        }
    }
}

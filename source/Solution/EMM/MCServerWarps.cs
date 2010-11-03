using System.IO;

namespace EnigmaMM
{
    public class MCServerWarps : SettingsFile
    {
        public MCServerWarps(string warpsLocation)
            : base(Path.Combine(Settings.MinecraftRoot, warpsLocation), ':')
        {
        }
    }
}

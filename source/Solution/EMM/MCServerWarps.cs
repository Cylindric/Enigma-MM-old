using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EnigmaMM
{
    public class MCServerWarps : SettingsFile
    {
        public MCServerWarps(string warpsLocation)
            : base(warpsLocation, ':')
        {
        }
    }
}

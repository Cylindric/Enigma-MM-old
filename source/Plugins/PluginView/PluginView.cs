using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.AddIn.Pipeline;

namespace EnigmaMM.PluginView
{
    [AddInBase]
    public abstract class PluginView
    {
        public abstract string SayHello(string name);
    }
}

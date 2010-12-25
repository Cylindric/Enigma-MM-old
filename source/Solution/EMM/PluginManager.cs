using System.Collections.Generic;
using System.IO;
using EnigmaMM.Interfaces;
using System.AddIn.Hosting;
using System.Collections.ObjectModel;
using EnigmaMM.HostViews;

namespace EnigmaMM
{
    class PluginManager
    {

        void Load(string path)
        {
            AddInStore.Rebuild(path);
            Collection<AddInToken> addinTokens = AddInStore.FindAddIns(typeof(PluginHostView), path);

            foreach(AddInToken addinToken in addinTokens)
            {
                PluginHostView addin =
                    addinToken.Activate<PluginHostView>(AddInSecurityLevel.Host);
            }

        }

    }
}

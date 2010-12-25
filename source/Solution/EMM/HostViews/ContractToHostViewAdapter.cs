using System.AddIn.Pipeline;
using EnigmaMM.Interfaces;

namespace EnigmaMM.HostViews
{
    [HostAdapter]
    public class ContractToHostViewAdapter : PluginHostView
    {
        private IPlugin mPlugin;
        private ContractHandle mHandle;

        public ContractToHostViewAdapter(IPlugin contract)
        {
            this.mPlugin = contract;
            mHandle = new ContractHandle(contract);
        }

        public override string SayHello(string name)
        {
            return this.mPlugin.SayHello(name);
        }
    }
}

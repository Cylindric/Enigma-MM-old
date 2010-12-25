using System.AddIn.Contract;
using System.AddIn.Pipeline;

namespace EnigmaMM.Interfaces
{

    [AddInContract]
    public interface IPlugin : IContract
    {
        string SayHello(string name);
    }

}

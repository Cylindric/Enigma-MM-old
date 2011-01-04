namespace EnigmaMM.Interfaces
{
    public interface IPlugin
    {
        string Name { get; }

        void Initialise(IServer server);
    }

}

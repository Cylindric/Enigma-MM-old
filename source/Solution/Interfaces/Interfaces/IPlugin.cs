namespace EnigmaMM.Interfaces
{
    public interface IPlugin
    {
        string Name { get; }
        string Tag { get; }

        void Initialise(IServer server);
    }

}

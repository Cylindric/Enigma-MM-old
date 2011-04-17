namespace EnigmaMM.Interfaces
{
    /// <summary>
    /// Provides a simple interface for passing Commands around.
    /// </summary>
    public interface ICommandPlugin: IPlugin
    {
        bool ParseCommand(IUser user, string command);
    }
}

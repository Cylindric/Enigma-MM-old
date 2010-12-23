namespace EnigmaMM.Interfaces
{
    public class Interface
    {
        /// <summary>
        /// Valid status-states for the server manager's Minecraft instance.
        /// </summary>
        public enum Status
        {
            Starting,
            Running,
            Busy,
            PendingRestart,
            PendingStop,
            Stopping,
            Stopped,
            Failed
        }

    }
}

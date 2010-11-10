using System;

namespace EnigmaMM
{
    /// <summary>
    /// Utility object for passing data between events.
    /// </summary>
    public class CommandEventArgs : EventArgs
    {
        private string Command { get; set;}

        /// <summary>
        /// Simply creates a new CommandEventArgs and sets the Command string to that supplied.
        /// </summary>
        /// <param name="s">The message</param>
        public CommandEventArgs(string s)
        {
            Command = s;
        }
    }
}

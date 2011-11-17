using EnigmaMM.Engine.Commands;

namespace EnigmaMM.Engine
{
    /// <summary>
    /// The CommandParser is a simple tool for converting user input into Server Manager or
    /// Minecraft commands.
    /// </summary>
    /// <remarks>Any unrecognised commands are passed up to Minecraft to process directly.</remarks>
    public class CommandParser
    {
        private EMMServer mMinecraft;
        
        /// <summary>
        /// Creates a new <c>CommandParser</c> and connects it to the specified <see cref="IServer"/>.
        /// </summary>
        /// <param name="minecraft"></param>
        public CommandParser(EMMServer minecraft)
        {
            mMinecraft = minecraft;
        }

        /// <summary>
        /// Handle a command from the CLI.
        /// Commands for the server manager are prefixed with the command-character.
        /// </summary>
        /// <param name="serverMessage">The command to parse.</param>
        public bool ParseCommand(EMMServerMessage serverMessage)
        {
            Command command;
            bool executed = true;

            string[] args;

            if (serverMessage.Data.ContainsKey("command"))
            {
                args = serverMessage.Data["command"].Split(' ');
            }
            else
            {
                args = serverMessage.Message.Split(' ');
            }
            switch (args[0])
            {
                case ("start"):
                    mMinecraft.StartServer();
                    break;

                case ("restart"):
                    mMinecraft.RestartServer(true);
                    break;

                case ("restart now"):
                    mMinecraft.StopServer(false);
                    mMinecraft.StartServer();
                    break;

                case ("stop"):
                    mMinecraft.StopServer(true);
                    break;

                case ("stop now"):
                    mMinecraft.StopServer(false);
                    break;

                case ("abort"):
                    mMinecraft.AbortPendingOperations();
                    break;

                case ("maps"):
                    command = new MapsCommand();
                    command.Execute(serverMessage);
                    break;

                case ("backup"):
                    command = new BackupCommand();
                    command.Execute(serverMessage);
                    break;

                case ("get"):
                    command = new GetCommand();
                    command.Execute(serverMessage);
                    break;

                default:
                    executed = false;
                    break;
            }
            return executed;
        }

    }

}

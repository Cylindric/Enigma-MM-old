using System;

namespace EnigmaMM
{
    /// <summary>
    /// The CommandParser is a simple tool for converting user input into Server Manager or
    /// Minecraft commands.
    /// </summary>
    /// <remarks>Any unrecognised commands are passed up to Minecraft to process directly.</remarks>
    public class CommandParser
    {
        private MCServer mMinecraft;

        public CommandParser(MCServer minecraft)
        {
            mMinecraft = minecraft;
        }


        /// <summary>
        /// Handle a command from the CLI.
        /// Commands for the server manager are prefixed with the command-character.
        /// </summary>
        /// <param name="Command">The command to parse.</param>
        public void ParseCommand(String Command)
        {
            string[] args = Command.Split(' ');
            switch (args[0])
            {
                case ("quit"):
                    ParseCommand("stop");
                    mMinecraft.StopCommsServer();
                    break;

                case ("start"):
                    mMinecraft.StartServer();
                    break;

                case ("restart"):
                    ParseCommand("stop");
                    mMinecraft.StartServer();
                    break;

                case ("restart-graceful"):
                    mMinecraft.GracefulRestart();
                    break;

                case ("stop"):
                    mMinecraft.StopServer();
                    break;

                case ("stop-graceful"):
                    mMinecraft.GracefulStop();
                    break;

                case ("abort-graceful"):
                    mMinecraft.AbortPendingRestart();
                    mMinecraft.AbortPendingStop();
                    break;

                case ("maps"):
                    mMinecraft.GenerateMaps(args);
                    break;

                case ("backup"):
                    mMinecraft.Backup();
                    break;

                default:
                    mMinecraft.SendCommand(Command);
                    break;
            }
        }
    }
}

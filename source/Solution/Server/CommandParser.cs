using System;

namespace EnigmaMM
{
    class CommandParser
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
        /// <remarks>
        /// Quit: shutdown the Minecraft server and then exit the Server Manager.
        /// Start: start the Minecraft server.
        /// Stop: stop the Minecraft server.
        /// Restart: Restart the Minecraft server.
        /// Graceful: Restart the Minecraft server as soon as no users are online.
        /// </remarks>
        /// <param name="Command">The command to parse.</param>
        public void ParseCommand(String Command)
        {
            switch (Command)
            {
                case ("quit"):
                    ServerProgram.mKeepRunning = false;
                    ParseCommand("stop");
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

                default:
                    mMinecraft.SendCommand(Command);
                    break;
            }
        }
    }
}

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

                case ("graceful"):
                    mMinecraft.GracefulRestart();
                    break;

                case ("stop"):
                    mMinecraft.Shutdown();
                    break;

                default:
                    mMinecraft.SendCommand(Command);
                    break;
            }
        }
    }
}

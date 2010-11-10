using System;

namespace EnigmaMM
{
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
        /// <remarks>
        /// quit: shutdown the Minecraft server and then exit the Server Manager.
        /// start: start the Minecraft server.
        /// stop: stop the Minecraft server.
        /// stop-graceful: Stop the Minecraft server as soon as no users are online.
        /// restart: Restart the Minecraft server.
        /// restart-graceful: Restart the Minecraft server as soon as no users are online.
        /// abort-graceful: Abort a pending graceful stop or restart.
        /// generate-maps: Regenerate maps.
        /// </remarks>
        /// <param name="Command">The command to parse.</param>
        public void ParseCommand(String Command)
        {
            switch (Command)
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

                case ("maps-all"):
                    mMinecraft.GenerateMaps();
                    break;

                case ("maps-av"):
                    mMinecraft.GenerateMapAV();
                    break;

                case ("maps-avextra"):
                    mMinecraft.GenerateMapAVExtra();
                    break;

                case ("maps-overviewer"):
                    mMinecraft.GenerateMapOverviewer();
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

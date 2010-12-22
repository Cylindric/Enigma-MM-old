using System;
using System.Collections;

namespace EnigmaMM.Interfaces
{
    /// <summary>
    /// Provides an interface for passing around Server instances.
    /// </summary>
    public interface IServer : IDisposable
    {
        /// <summary>
        /// Raised when the server has stopped.
        /// </summary>
        event EventHandler<ServerMessageEventArgs> ServerStopped;

        /// <summary>
        /// Raised when the server has stopped.
        /// </summary>
        event EventHandler<ServerMessageEventArgs> ServerStarted;

        /// <summary>
        /// Raised when the server sends a message.  Data contains the message.
        /// </summary>
        event EventHandler<ServerMessageEventArgs> ServerMessage;

        /// <summary>
        /// Raised when the server experiences an error.  Data contains the error.
        /// </summary>
        event EventHandler<ServerMessageEventArgs> ServerError;

        /// <summary>
        /// Raised when the server status changes.  Data contains the new status.
        /// </summary>
        event EventHandler<ServerMessageEventArgs> StatusChanged;

        /// <summary>
        /// Gets the Minecraft server properties.
        /// </summary>
        MCServerProperties ServerProperties { get; }

        /// <summary>
        /// Gets the current server status.
        /// </summary>
        EnigmaMM.EMMServer.Status CurrentStatus { get; }

        /// <summary>
        /// Returns the last status message of the server.
        /// </summary>
        string LastStatusMessage { get; }

        /// <summary>
        /// Returns a list of the current online users.
        /// </summary>
        ArrayList Users { get; }

        /// <summary>
        /// Starts the Minecraft server.
        /// </summary>
        void StartServer();

        /// <summary>
        /// Stops the Minecraft server
        /// </summary>
        void StopServer(bool graceful);

        /// <summary>
        /// Shuts down the running Server.
        /// </summary>
        void StopServer(bool graceful, int timeout, bool force);

        /// <summary>
        /// Restarts the Minecraft server.
        /// </summary>
        void RestartServer(bool graceful);

        /// <summary>
        /// Aborts any pending graceful commands.
        /// </summary>
        void AbortPendingOperations();

        /// <summary>
        /// Sends a message to all online users.
        /// </summary>
        void Broadcast(string message);

        /// <summary>
        /// Initiate a server backup.
        /// </summary>
        void Backup();

        /// <summary>
        /// Refreshes the online user list.
        /// </summary>
        bool RefreshOnlineUserList();

        /// <summary>
        /// Executes the specified command.
        /// </summary>
        void Execute(string command);

        /// <summary>
        /// Generates maps using the specified arguments.
        /// </summary>
        void GenerateMaps(string[] args);
        
        /// <summary>
        /// Loads user data from the world save.
        /// </summary>
        void LoadSavedUserInfo();

        /// <summary>
        /// Raise a server message.
        /// </summary>
        /// <param name="message"></param>
        void RaiseServerMessage(string message);

        /// <summary>
        /// Turns off the server auto-save.
        /// </summary>
        void BlockAutoSave();

        /// <summary>
        /// Turns on the server auto-save.
        /// </summary>
        void UnblockAutoSave();
    }
}

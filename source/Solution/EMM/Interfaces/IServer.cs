using System;
using System.Collections;

namespace EnigmaMM.Interfaces
{
    public interface IServer : IDisposable
    {
        event EventHandler<ServerMessageEventArgs> ServerStopped;
        event EventHandler<ServerMessageEventArgs> ServerStarted;
        event EventHandler<ServerMessageEventArgs> ServerMessage;
        event EventHandler<ServerMessageEventArgs> ServerError;
        event EventHandler<ServerMessageEventArgs> StatusChanged;

        MCServerProperties ServerProperties { get; }
        MCServerWarps ServerWarps { get; }
        EnigmaMM.EMMServer.Status CurrentStatus { get; }
        string LastStatusMessage { get; }
        ArrayList Users { get; }

        void StartServer();
        void StopServer(bool graceful);
        void StopServer(bool graceful, int timeout, bool force);
        void RestartServer(bool graceful);
        void AbortPendingOperations();
        void Broadcast(string message);
        void Backup();
        bool RefreshOnlineUserList();
        void Execute(string command);
        void GenerateMaps(string[] args);
        void LoadSavedUserInfo();
        void RaiseServerMessage(string message);
    }
}

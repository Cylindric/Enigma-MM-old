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

        #region Properties

        MCServerProperties ServerProperties
        {
            get;
        }

        MCServerWarps ServerWarps
        {
            get;
        }

        EnigmaMM.EMMServer.Status CurrentStatus
        {
            get;
        }

        string LastStatusMessage
        {
            get;
        }

        ArrayList Users
        {
            get;
        }

        bool Listening
        {
            get;
        }

        #endregion

        void StartServer();
        void StopServer();
        void StopServer(int timeout, bool force);
        void RestartServer();
        void GracefulStop();
        void AbortPendingStop();
        void GracefulRestart();
        void Broadcast(string message);
        void AbortPendingRestart();
        void Backup();
        bool RefreshOnlineUserList();
        void SendCommand(string Command);
        void GenerateMaps(string[] args);
        void LoadSavedUserInfo();
        void RaiseServerMessage(string message);
        void StopCommsServer();
    }
}

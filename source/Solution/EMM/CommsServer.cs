using System;
using System.Net;
using System.Net.Sockets;

namespace EnigmaMM
{
    public class CommsServer : CommsManager
    {
        private bool mListening = false;

        public bool Listening
        {
            get { return mListening; }
        }

        /// <summary>
        /// Constructor for Server.
        /// </summary>
        public CommsServer()
        {
            ServerIP = Settings.ServerListenIp;
            ServerPort = Settings.ServerListenPort;
            Username = Settings.ServerUsername;
            Password = Settings.ServerPassword;
        }

        /// <summary>
        /// Starts the Server Manager listening on the configured address and port.
        /// </summary>
        public void StartListener()
        {
            VerifySecurity();
            IPAddress address = GetRemoteAddress();
            IPEndPoint endpoint = GetRemoteEndpoint(address);

            mSocketListener = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.IP);
            mSocketListener.Bind(endpoint);
            mSocketListener.Listen(4);
            mSocketListener.BeginAccept(new AsyncCallback(HandleClientConnect), null);
            mListening = true;
        }
        

        private void HandleClientConnect(IAsyncResult asyn)
        {
            try
            {
                OnRemoteConnection("Client connected");
                Socket WorkerSocket = mSocketListener.EndAccept(asyn);
                SocketList.Add(WorkerSocket);
                SendData(WorkerSocket, "Connected to server");
                WaitForData(WorkerSocket, Connections);
                mSocketListener.BeginAccept(new AsyncCallback(HandleClientConnect), null);
            }
            catch (ObjectDisposedException e)
            {
                System.Diagnostics.Debugger.Log(0, "1", "HandleClientConnect ObjectDisposedException: " + e.Message);
            }
        }

    }
}
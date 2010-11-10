using System;
using System.Net;
using System.Net.Sockets;

namespace EnigmaMM
{
    /// <summary>
    /// The CommsServer class handles all communication to and from the server.
    /// It will listen on the configured port for client connections, and subsequently
    /// send Minecraft output to the clients, and accept commands from the clients.
    /// </summary>
    public class CommsServer : CommsManager
    {
        private bool mListening = false;

        /// <summary>
        /// Returns wether or not the server is currently listening for connections.
        /// </summary>
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


        /// <summary>
        /// Stops the server from listening for new connections.
        /// </summary>
        public void StopListener()
        {
            if (mListening)
            {
                DisconnectAll();
                if (mSocketListener.Connected)
                {
                    mSocketListener.Disconnect(false);
                }
                mSocketListener.Close();                
            }
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
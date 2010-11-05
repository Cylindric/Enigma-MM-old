using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections;

namespace EnigmaMM
{
    public class Server : CommsManager
    {
        private bool mListening = false;

        public bool Listening
        {
            get { return mListening; }
        }

        public Server()
        {
            this.ServerIP = Settings.ServerListenIp;
            this.ServerPort = Settings.ServerListenPort;
            this.Username = Settings.ServerUsername;
            this.Password = Settings.ServerPassword;
        }

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
                mSocketList.Add(WorkerSocket);
                SendData(WorkerSocket, "Connected to server");
                WaitForData(WorkerSocket, mConnectionCount);
                mSocketListener.BeginAccept(new AsyncCallback(HandleClientConnect), null);
            }
            catch (ObjectDisposedException e)
            {
                System.Diagnostics.Debugger.Log(0, "1", "HandleClientConnect ObjectDisposedException: " + e.Message);
            }
        }

    }
}
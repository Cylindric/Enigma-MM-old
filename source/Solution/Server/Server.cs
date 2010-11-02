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
        private string mSuppliedUsername = null;
        private string mSuppliedPassword = null;
        private bool mAuthenticated = false;

        public override event ServerMessageEventHandler MessageReceived;

        public bool Listening
        {
            get { return mListening; }
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


        /// <summary>
        /// Extend the base command-received method to capture the first two messages as the username
        /// and password, and check that these match the configured username and password before
        /// allowing the command to be passed up to the server.
        /// </summary>
        /// <param name="Message"></param>
        protected override void OnMessageReceived(String Message)
        {
            if (!mAuthenticated)
            {
                if (mSuppliedUsername == null)
                {
                    mSuppliedUsername = Message;
                }
                else if (mSuppliedPassword == null)
                {
                    mSuppliedPassword = Message;
                }
                else if (CompareHashes(mSuppliedUsername, CreateHash(mUsername)) && CompareHashes(mSuppliedPassword, CreateHash(mPassword)))
                {
                    mAuthenticated = true;
                }
            }

            if (mAuthenticated)
            {
                base.OnMessageReceived(Message);
            }
        }

    }
}
using System;
using System.Net.Sockets;
using System.Text;
using System.Collections;
using System.Net;

namespace EnigmaMM
{
    public class CommsManager
    {
        protected String mServerAddress = "any";
        protected int mServerPort = 8221;
        protected Socket mSocketListener;
        protected string mUsername = "";
        protected string mPassword = "";
        protected String mData;
        protected int mConnectionCount = 0;
        protected ArrayList mSocketList = new ArrayList();

        private AsyncCallback pfnWorkerCallBack;
        private const string TERMINATOR = "\n";

        public delegate void ServerMessageEventHandler(string Message);
        public event ServerMessageEventHandler MessageReceived;
        public event ServerMessageEventHandler RemoteConnection;
        public event ServerMessageEventHandler RemoteDisconnection;

        public int Connections
        {
            get { return mConnectionCount; }
        }

        public String ServerIP
        {
            get { return mServerAddress; }
            set { mServerAddress = value; }
        }

        public int ServerPort
        {
            get { return mServerPort; }
            set { mServerPort = value; }
        }

        public string Username
        {
            set { mUsername = value; }
        }

        public string Password
        {
            set { mPassword = value; }
        }

        /// <summary>
        /// Sends a message to the specified socket
        /// </summary>
        /// <param name="socket">The socket to send the data to</param>
        /// <param name="message">The data to send</param>
        public void SendData(Socket socket, string message)
        {
            // Add a terminator so the other end knows the data is ended
            message += TERMINATOR;

            // Convert the data and send it
            byte[] DataToSend = System.Text.Encoding.UTF8.GetBytes(message);
            if ((socket != null) && (socket.Connected == true))
            {
                socket.Send(DataToSend);
            }
        }

        /// <summary>
        /// Sends a message to all connected sockets
        /// </summary>
        /// <param name="message">The data to send</param>
        public void SendData(string message)
        {
            foreach (Socket socket in mSocketList)
            {
                SendData(socket, message);
            }
        }

        protected void WaitForData(Socket soc, int ClientNumber)
        {
            if (pfnWorkerCallBack == null)
            {
                pfnWorkerCallBack = new AsyncCallback(HandleDataReceived);
            }
            SocketPacket theSocPkt = new SocketPacket(soc, ClientNumber);

            try
            {
                soc.BeginReceive(theSocPkt.DataBuffer, 0, theSocPkt.DataBuffer.Length, SocketFlags.None, pfnWorkerCallBack, theSocPkt);
            }
            catch (ObjectDisposedException e)
            {
                System.Diagnostics.Debugger.Log(0, "1", "OnDataReceived ObjectDisposedException: " + e.Message);
            }
            catch (SocketException)
            {
                OnRemoteDisconnection("Client disconnected");
            }
            catch (Exception e)
            {
                System.Diagnostics.Debugger.Log(0, "1", "OnDataReceived uh-oh? :( " + e.Message);
            }
        }


        private void HandleDataReceived(IAsyncResult asyn)
        {
            try
            {
                SocketPacket theSocketId = (SocketPacket)asyn.AsyncState;

                // Get the number of chars in the buffer
                int iRx = theSocketId.ThisSocket.EndReceive(asyn);

                char[] chars = new char[iRx + 1];

                // Decode the received data, making sure to only get iRx
                // characters (buffer is filled with \0)
                System.String szData = Encoding.UTF8.GetString(theSocketId.DataBuffer, 0, iRx);

                // If the buffer contains any new-line characters, then we need
                // to parse out each of the sent commands
                while (szData.Contains(TERMINATOR))
                {
                    mData += szData.Substring(0, szData.IndexOf(TERMINATOR));
                    szData = szData.Substring(szData.IndexOf(TERMINATOR) + TERMINATOR.Length);

                    OnCommandReceived(mData);

                    mData = "";
                }
                mData += szData;

                // Wait for more commands
                WaitForData(theSocketId.ThisSocket, theSocketId.ClientNumber);
            }
            catch (ObjectDisposedException e)
            {
                System.Diagnostics.Debugger.Log(0, "1", "HandleDataReceived ObjectDisposedException: " + e.Message);
            }
            catch (SocketException)
            {
                OnRemoteDisconnection("Client disconnected");
            }
            catch (Exception e)
            {
                System.Diagnostics.Debugger.Log(0, "1", "HandleDataReceived uh-oh? :( " + e.Message);
            }
        }

        public void DisconnectAll()
        {
            foreach (Socket socket in mSocketList)
            {
                if ((socket != null) && (socket.Connected == true))
                {
                    socket.Close();
                }
            }
        }

        protected virtual void OnCommandReceived(String Message)
        {
            if (MessageReceived != null)
            {
                MessageReceived(Message);
            }
        }


        protected virtual void OnRemoteConnection(string Message)
        {
            mConnectionCount++;
            if (RemoteConnection != null)
            {
                RemoteConnection(Message);
            }
        }



        protected virtual void OnRemoteDisconnection(string Message)
        {
            mConnectionCount--;
            if (RemoteDisconnection != null)
            {
                RemoteDisconnection(Message);
            }
        }


        protected void VerifySecurity() {
            if ((mUsername == "") || (mUsername == "changeme"))
            {
                throw new InvalidOperationException("No username specified.");
            }
            if ((mPassword == "") || (mPassword == "changeme"))
            {
                throw new InvalidOperationException("No password specified");
            }
        }


        protected IPAddress GetRemoteAddress()
        {
            IPAddress remoteIPAddress;

            // Special case for servers, as they will often listen on "any"
            if (mServerAddress == "any")
            {
                remoteIPAddress = IPAddress.Any;
            }

            // Attempt a straight parse of an IP Address
            else if (IPAddress.TryParse(mServerAddress, out remoteIPAddress) == false)
            {
                // parse of IP failed, is it a hostname?
                IPAddress[] remoteAddresses = Dns.GetHostAddresses(mServerAddress);
                foreach (IPAddress ip in remoteAddresses)
                {
                    // currently only support ip4 addresses
                    if (ip.AddressFamily == AddressFamily.InterNetwork)
                    {
                        remoteIPAddress = ip;
                        break;
                    }

                }

                if (remoteIPAddress == null)
                {
                    throw new Exception("Cannot resolve server address: " + mServerAddress);
                }
            }

            return remoteIPAddress;
        }


        protected IPEndPoint GetRemoteEndpoint(IPAddress address)
        {
            return new IPEndPoint(address, mServerPort);
        }

    }
}

using System;
using System.Net.Sockets;
using System.Text;
using System.Collections;
using System.Net;
using System.Security.Cryptography;

namespace EnigmaMM
{
    public class CommsManager
    {
        public enum Status
        {
            Starting,
            Running,
            Stopping,
            Stopped,
            Finished
        }

        public Status mStatus = Status.Stopped;
        private String mServerAddress = "any";
        private int mServerPort = 8221;
        protected Socket mSocketListener;
        private string mUsername = "";
        private string mPassword = "";
        private String mData;
        private int mConnectionCount = 0;
        private ArrayList mSocketList = new ArrayList();

        private AsyncCallback pfnWorkerCallBack;
        private const string TERMINATOR = "\n";

        public delegate void ServerMessageEventHandler(string Message);
        public virtual event ServerMessageEventHandler MessageReceived;
        public event ServerMessageEventHandler RemoteConnection;
        public event ServerMessageEventHandler RemoteDisconnection;

        /// <summary>
        /// Gets the current status of this CommsManager.
        /// </summary>
        public Status ComStatus
        {
            get { return mStatus; }
        }

        /// <summary>
        /// Gets the number of current active connections.
        /// </summary>
        public int Connections
        {
            get { return mConnectionCount; }
        }

        /// <summary>
        /// Gets the current list of used sockets.
        /// </summary>
        /// <remarks>Note that the SocketList can contain null objects from closed sockets.</remarks>
        protected ArrayList SocketList
        {
            get { return mSocketList; }
        }

        /// <summary>
        /// Gets or sets the ip address to use for making or accepting connections.
        /// </summary>
        public String ServerIP
        {
            get { return mServerAddress; }
            set { mServerAddress = value; }
        }

        /// <summary>
        /// Gets or sets the port to use for making or accepting connections.
        /// </summary>
        public int ServerPort
        {
            get { return mServerPort; }
            set { mServerPort = value; }
        }

        /// <summary>
        /// Gets or sets the username to use to authenticate connections.
        /// </summary>
        public string Username
        {
            get { return mUsername; }
            set { mUsername = value; }
        }

        /// <summary>
        /// Gets or sets the password to use to authenticate connections.
        /// </summary>
        public string Password
        {
            get { return mPassword; }
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
            byte[] DataToSend = Encoding.UTF8.GetBytes(message);
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
                SocketPacket socketPacket = (SocketPacket)asyn.AsyncState;

                // Get the number of chars in the buffer
                int iRx = socketPacket.ThisSocket.EndReceive(asyn);

                char[] chars = new char[iRx + 1];

                // Decode the received data, making sure to only get iRx
                // characters (buffer is filled with \0)
                System.String szData = Encoding.UTF8.GetString(socketPacket.DataBuffer, 0, iRx);

                // If the buffer contains any new-line characters, then we need
                // to parse out each of the sent commands
                while (szData.Contains(TERMINATOR))
                {
                    mData += szData.Substring(0, szData.IndexOf(TERMINATOR));
                    szData = szData.Substring(szData.IndexOf(TERMINATOR) + TERMINATOR.Length);

                    if (!socketPacket.Authenticated)
                    {
                        if (socketPacket.UserHash == null)
                        {
                            socketPacket.UserHash = mData;
                        }
                        else if (socketPacket.PassHash == null)
                        {
                            socketPacket.PassHash = mData;
                        }
                        else 
                        {
                            if (CompareHashes(socketPacket.UserHash, CreateHash(mUsername)) && CompareHashes(socketPacket.PassHash, CreateHash(mPassword)))
                            {
                                socketPacket.Authenticated = true;
                            }
                            else
                            {
                                socketPacket.UserHash = null;
                                socketPacket.PassHash = null;
                                socketPacket.Authenticated = false;
                            }
                        }
                    }

                    if (socketPacket.Authenticated)
                    {
                        OnMessageReceived(mData);
                    }

                    mData = "";
                }
                mData += szData;

                // Wait for more commands
                WaitForData(socketPacket.ThisSocket, socketPacket.ClientNumber);
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

        protected virtual void OnMessageReceived(String Message)
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

        /// <summary>
        /// Checks that current settings meet minimum security requirements.
        /// </summary>
        /// <exception cref="UnauthorizedAccessException">Username and/or password not supplied, or invalid.</exception>
        protected void VerifySecurity() {
            if ((mUsername == "") || (mUsername == "changeme"))
            {
                throw new UnauthorizedAccessException(string.Format("Invalid or no username specified: \"{0}\"", mUsername));
            }
            if ((mPassword == "") || (mPassword == "changeme"))
            {
                throw new UnauthorizedAccessException("Invalid or no password specified.");
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

        /// <summary>
        /// Creates a simple MD5 hash of the supplied string.
        /// </summary>
        /// <param name="message">The string to hash</param>
        /// <returns>The hash of the supplied string</returns>
        protected string CreateHash(string message)
        {
            byte[] src;
            byte[] hash;
            src = Encoding.UTF8.GetBytes(message);
            hash = new MD5CryptoServiceProvider().ComputeHash(src);

            StringBuilder output = new StringBuilder(hash.Length);
            for (int i = 0; i < hash.Length; i++)
            {
                output.Append(hash[i].ToString("X2"));
            }
            return output.ToString();
        }


        /// <summary>
        /// Compares the two supplied hashes and returns true if they are the same, else returns false.
        /// </summary>
        /// <param name="h1">The first hash</param>
        /// <param name="h2">The second hash</param>
        /// <returns>True if h1 equals h2; else False</returns>
        protected bool CompareHashes(string h1, string h2)
        {
            bool areEqual = false;
            if (h1.Length == h2.Length)
            {
                int i = 0;
                while ((i < h1.Length) && (h1[i] == h2[i]))
                {
                    i += 1;
                }
                if (i == h1.Length)
                {
                    areEqual = true;
                }
            }
            return areEqual;
        }
    }
}

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections;

namespace EnigmaMM
{
    public class Server
    {
        private String mServerIP = "any";
        private int mServerPort = 8221;
        private Socket mSocketListener;
        private AsyncCallback pfnWorkerCallBack;
        private bool mListening = false;
        private string mUsername = "";
        private string mPassword = "";
        private String mData;

        private ArrayList mSocketList = new ArrayList();
        private int mClientCount = 0;

        public delegate void ServerMessageEventHandler(string Message);
        public event ServerMessageEventHandler CommandReceived;
        public event ServerMessageEventHandler ClientConnected;
        public event ServerMessageEventHandler ClientDisconnected;

        public String ServerIP
        {
            get { return mServerIP; }
            set { mServerIP = value; }
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

        public bool Listening
        {
            get { return mListening; }
        }

        public int ConnectedClients
        {
            get { return mClientCount; }
        }

        public void StartListener()
        {

            if ((mUsername == "") || (mUsername == "changeme"))
            {
                throw new InvalidOperationException("No username specified in settings file.");
            }
            if ((mPassword == "") || (mPassword == "changeme"))
            {
                throw new InvalidOperationException("No password specified in settings file");
            }

            IPEndPoint LocalIp;
            if (mServerIP == "any")
            {
                LocalIp = new IPEndPoint(IPAddress.Any, mServerPort);
            }
            else
            {
                LocalIp = new IPEndPoint(IPAddress.Parse(mServerIP), mServerPort);
            }

            mSocketListener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            mSocketListener.Bind(LocalIp);
            mSocketListener.Listen(4);
            mSocketListener.BeginAccept(new AsyncCallback(HandleClientConnect), null);
            mListening = true;
        }

        private void HandleClientConnect(IAsyncResult asyn)
        {
            try
            {
                OnClientConnected("Client connected");
                Socket WorkerSocket = mSocketListener.EndAccept(asyn);
                mSocketList.Add(WorkerSocket);
                SendData(WorkerSocket, "Connected to server");
                WaitForData(WorkerSocket, mClientCount);
                mSocketListener.BeginAccept(new AsyncCallback(HandleClientConnect), null);
            }
            catch (ObjectDisposedException e)
            {
                System.Diagnostics.Debugger.Log(0, "1", "HandleClientConnect ObjectDisposedException: " + e.Message);
            }
        }

        public void SendData(string Data)
        {
            foreach (Socket socket in mSocketList)
            {
                SendData(socket, Data);
            }
        }

        public void SendData(Socket mSoc, string Data)
        {
            // Add a terminator so the other end knows the data is ended
            Data += "\n";

            // Convert the data and send it
            byte[] DataToSend = System.Text.Encoding.UTF8.GetBytes(Data);
            if ((mSoc != null) && (mSoc.Connected == true))
            {
                mSoc.Send(DataToSend);
            }
        }

        private void WaitForData(Socket soc, int ClientCount)
        {
            if (pfnWorkerCallBack == null)
            {
                pfnWorkerCallBack = new AsyncCallback(HandleDataReceived);
            }
            CSocketPacket theSocPkt = new CSocketPacket(soc, ClientCount);

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
                OnClientDisconnected("Client disconnected");
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
                CSocketPacket theSocketId = (CSocketPacket)asyn.AsyncState;

                // Get the number of chars in the buffer
                int iRx = theSocketId.ThisSocket.EndReceive(asyn);

                char[] chars = new char[iRx + 1];

                // Decode the received data, making sure to only get iRx
                // characters (buffer is filled with \0)
                System.String szData = Encoding.UTF8.GetString(theSocketId.DataBuffer, 0, iRx);

                // If the buffer contains any new-line characters, then we need
                // to parse out each of the sent commands
                while (szData.Contains("\n"))
                {
                    mData += szData.Substring(0, szData.IndexOf("\n"));
                    szData = szData.Substring(szData.IndexOf("\n") + 1);

                    Console.WriteLine("HandleDataReceived(" + theSocketId.ClientNumber +"): COMMAND: [" + mData + "]");
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
                OnClientDisconnected("Client disconnected");
            }
            catch (Exception e)
            {
                System.Diagnostics.Debugger.Log(0, "1", "HandleDataReceived uh-oh? :( " + e.Message);
            }
        }


        protected virtual void OnCommandReceived(String Message)
        {
            if (CommandReceived != null)
            {
                CommandReceived(Message);
            }
        }


        protected virtual void OnClientConnected(string Message)
        {
            mClientCount++;
            if (ClientConnected != null)
            {
                ClientConnected(Message);
            }
        }



        protected virtual void OnClientDisconnected(string Message)
        {
            mClientCount--;
            if (ClientDisconnected != null)
            {
                ClientDisconnected(Message);
            }
        }

    }
}
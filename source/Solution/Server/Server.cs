using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace EnigmaMM
{
    public class Server
    {
        private String mServerIP = "any";
        private int mServerPort = 8221;
        private Socket mSocketListener;
        private Socket mSocketWorker;
        private AsyncCallback pfnWorkerCallBack;
        private string mText;
        private bool mListening = false;

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
        
        public bool Listening
        {
            get { return mListening; }
        }
        
        public void StartListener()
        {
            mSocketListener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            IPEndPoint LocalIp;
            if (mServerIP == "any")
            {
                LocalIp = new IPEndPoint(IPAddress.Any, mServerPort);
            } else
            {
                LocalIp = new IPEndPoint(IPAddress.Parse(mServerIP), mServerPort);
            }
            mSocketListener.Bind(LocalIp);
            mSocketListener.Listen(4);
            mSocketListener.BeginAccept(new AsyncCallback(OnClientConnect), null);
            mListening = true;
        }

        private void OnClientConnect(IAsyncResult asyn) 
        {
            try
            {
                mSocketWorker = mSocketListener.EndAccept(asyn);
                WaitForData(mSocketWorker);
            }
            catch (ObjectDisposedException)
            {
                System.Diagnostics.Debugger.Log(0, "1", "OnClientConnect: Socket closed");
            }
        }

        private void WaitForData(Socket soc)
        {
            if (pfnWorkerCallBack == null)
            {
                pfnWorkerCallBack = new AsyncCallback(OnDataReceived);
            }
            CSocketPacket theSocPkt = new CSocketPacket();
            theSocPkt.thisSocket = soc;

            soc.BeginReceive(theSocPkt.dataBuffer, 0, theSocPkt.dataBuffer.Length, SocketFlags.None, pfnWorkerCallBack, theSocPkt);
        }

        private void OnDataReceived(IAsyncResult asyn)
        {
            try
            {
                CSocketPacket theSocketId = (CSocketPacket)asyn.AsyncState;

                // Get the number of chars in the buffer
                int iRx = theSocketId.thisSocket.EndReceive(asyn);

                char[] chars = new char[iRx + 1];
                
                // Decode the received data, making sure to only get iRx 
                // characters (buffer is filled with \0)
                System.String szData = Encoding.UTF8.GetString(theSocketId.dataBuffer, 0, iRx);

                // If the buffer contains any new-line characters, then we need
                // to parse out each of the sent commands
                while (szData.Contains("\n"))
                {
                    mText += szData.Substring(0, szData.IndexOf("\n"));
                    szData = szData.Substring(szData.IndexOf("\n") + 1);
                    
                    Console.WriteLine("OnDataReceived: COMMAND: [" + mText + "]");
                    theSocketId.thisSocket.Send(System.Text.Encoding.UTF8.GetBytes("Executing command [" + mText + "]"));

                    mText = "";
                }
                mText = mText + szData;
                
                // Wait for more commands
                WaitForData(mSocketWorker);
            }
            catch (ObjectDisposedException e)
            {
                System.Diagnostics.Debugger.Log(0, "1", "OnDataReceived ObjectDisposedException: " + e.Message);
            }
            catch (SocketException e)
            {
                System.Diagnostics.Debugger.Log(0, "1", "OnDataReceived SocketException :( " + e.Message);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debugger.Log(0, "1", "OnDataReceived uh-oh? :( " + e.Message);
            }
        }
    }
}

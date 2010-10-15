using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace EnigmaMM
{
    public class Server
    {
        private Socket mSocketListener;
        private Socket mSocketWorker;
        private AsyncCallback pfnWorkerCallBack;
        private string mText;
        private bool mListening = false;

        public bool Listening
        {
            get { return mListening; }
        }
        
        public void StartListener()
        {
            mSocketListener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            IPEndPoint LocalIp = new IPEndPoint(IPAddress.Any, 8221);
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
                int iRx = 0;
                iRx = theSocketId.thisSocket.EndReceive(asyn);
                char[] chars = new char[iRx + 1];
                //System.Text.Decoder d = System.Text.Encoding.UTF8.GetDecoder();
                System.String szData = Encoding.UTF8.GetString(theSocketId.dataBuffer);
                //Console.WriteLine("OnDataReceived: \"" + szData + "\"");
                while (szData.Contains("\n"))
                {
                    mText += szData.Substring(0, szData.IndexOf("\n"));
                    szData = szData.Substring(szData.IndexOf("\n") + 1);
                    Console.WriteLine("OnDataReceived: COMMAND: \"" + mText + "\"\n");
                    mText = "";
                }
                mText = mText + szData;
                
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

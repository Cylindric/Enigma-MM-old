using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;

namespace EnigmaMM
{
    public class Client
    {
        private byte[] mDataBuffer = new byte[10];
        private IAsyncResult mAsynResult;
        private String mData;
        private String mServerIP = "127.0.0.1";
        private int mServerPort = 8221;

        public AsyncCallback pfnCallBack;
        public Socket mSocClient;

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

        public void StartClient()
        {
            mSocClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress remoteIPAddress = IPAddress.Parse(mServerIP);
            IPEndPoint remoteEndpoint = new IPEndPoint(remoteIPAddress, mServerPort);
            mSocClient.Connect(remoteEndpoint);
            WaitForData();
        }

        public void SendData(string Data)
        {
            byte[] DataToSend = System.Text.Encoding.UTF8.GetBytes(Data);
            mSocClient.Send(DataToSend);
        }

        public void StopClient()
        {
            if (mSocClient != null)
            {
                mSocClient.Close();
                mSocClient = null; 
            }
        }

        public void WaitForData()
        {
            if (pfnCallBack == null)
            {
                pfnCallBack = new AsyncCallback(OnDataReceived);
            }
            CSocketPacket theSocPkt = new CSocketPacket();
            theSocPkt.thisSocket = mSocClient;
            mAsynResult = mSocClient.BeginReceive(theSocPkt.dataBuffer, 0, theSocPkt.dataBuffer.Length, SocketFlags.None, pfnCallBack, theSocPkt);
        }

        public void OnDataReceived(IAsyncResult asyn)
        {
            try
            {
                CSocketPacket theSocketId = (CSocketPacket)asyn.AsyncState;
                int iRx = 0;
                iRx = theSocketId.thisSocket.EndReceive(asyn);
                char[] chars = new char[iRx + 1];
                System.Text.Decoder d = System.Text.Encoding.UTF8.GetDecoder();
                int charLen = d.GetChars(theSocketId.dataBuffer, 0, iRx, chars, 0);
                System.String Data = new System.String(chars);
                mData += Data;
                WaitForData();
            }
            catch (ObjectDisposedException)
            {
                System.Diagnostics.Debugger.Log(0, "1", "OnDataReceived: Socket closed");
            }
        }
    }
}

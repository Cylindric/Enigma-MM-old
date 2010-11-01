﻿using System;
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

        public delegate void ClientMessageEventHandler(string Message);
        public event ClientMessageEventHandler MessageReceived;

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
            IPAddress remoteIPAddress;
            if (IPAddress.TryParse(mServerIP, out remoteIPAddress) == false)
            {
                // parse of IP failed, is it a hostname?
                IPAddress[] remoteAddresses = Dns.GetHostAddresses(mServerIP);
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
                    throw new Exception("Cannot resolve server address: " + mServerIP);
                }
            }
            IPEndPoint remoteEndpoint = new IPEndPoint(remoteIPAddress, mServerPort);
 
            mSocClient = new Socket(remoteIPAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            mSocClient.Connect(remoteEndpoint);
            if (mSocClient.Connected)
            {
                WaitForData();
            }
        }


        public void SendData(string Data)
        {
            byte[] DataToSend = System.Text.Encoding.UTF8.GetBytes(Data);
            if (mSocClient != null)
            {
                mSocClient.Send(DataToSend);
            }
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
            CSocketPacket theSocPkt = new CSocketPacket(mSocClient);
            mAsynResult = mSocClient.BeginReceive(theSocPkt.DataBuffer, 0, theSocPkt.DataBuffer.Length, SocketFlags.None, pfnCallBack, theSocPkt);
        }


        public void OnDataReceived(IAsyncResult asyn)
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
                string mText = "";
                while (szData.Contains("\n"))
                {
                    mText += szData.Substring(0, szData.IndexOf("\n"));
                    szData = szData.Substring(szData.IndexOf("\n") + 1);
                    mText = "";
                }
                mText = mText + szData;

                // Wait for more commands
                WaitForData();
            }
            catch (ObjectDisposedException)
            {
                System.Diagnostics.Debugger.Log(0, "1", "OnDataReceived: Socket closed");
            }
        }


        protected virtual void OnMessageReceived(String Message)
        {
            if (MessageReceived != null)
            {
                SendData(Message);
                MessageReceived(Message);
            }
        }

    }
}
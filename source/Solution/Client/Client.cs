﻿using System;
using System.Net;
using System.Net.Sockets;

namespace EnigmaMM
{
    public class Client : CommsManager
    {

        public void StartClient()
        {
            VerifySecurity();
            IPAddress address = GetRemoteAddress();
            IPEndPoint endpoint = GetRemoteEndpoint(address);

            Socket socket = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(endpoint);
            mSocketList.Add(socket);
            if (socket.Connected)
            {
                WaitForData(socket, 0);
            }
        }


    }
}
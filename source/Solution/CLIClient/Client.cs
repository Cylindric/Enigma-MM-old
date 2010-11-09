﻿using System;
using System.Net;
using System.Net.Sockets;

namespace EnigmaMM
{
    public class Client : CommsManager
    {

        public Client()
        {
            this.ServerIP = Settings.ClientConnectIp;
            this.ServerPort = Settings.ClientConnectPort;
            this.Username = Settings.ServerUsername;
            this.Password = Settings.ServerPassword;
        }


        public void StartClient()
        {
            VerifySecurity();
            IPAddress address = GetRemoteAddress();
            IPEndPoint endpoint = GetRemoteEndpoint(address);

            Socket socket = new Socket(address.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(endpoint);

            if (socket.Connected)
            {
                mSocketList.Add(socket);

                // first two commands must be the username and password
                SendData(CreateHash(mUsername));
                SendData(CreateHash(mPassword));

                WaitForData(socket, 0);
            }
        }


    }
}
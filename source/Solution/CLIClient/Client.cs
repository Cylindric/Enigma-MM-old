﻿using System;
using System.Net;
using System.Net.Sockets;

namespace EnigmaMM
{
    public class Client : CommsManager
    {

        public Client()
        {
            ServerIP = Settings.ClientConnectIp;
            ServerPort = Settings.ClientConnectPort;
            Username = Settings.ServerUsername;
            Password = Settings.ServerPassword;
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
                SocketList.Add(socket);

                // first two commands must be the username and password
                SendData(CreateHash(Username));
                SendData(CreateHash(Password));

                WaitForData(socket, 0);
            }
        }


    }
}
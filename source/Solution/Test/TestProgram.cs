using System;
using System.Collections.Generic;
using System.Text;
using EnigmaMM;

namespace EnigmaMM
{
    class TestProgram
    {
        static Server mServer;
        static Client mClient;

        static void Main(string[] args)
        {
            TestConnectDisconnect();
            TestSendingCommands();
            
        }


        private static void CreateObjects()
        {
            mServer = new Server();
            mClient = new Client();
            mServer.ServerIP = "any";
            mServer.ServerPort = 8221;
            mClient.ServerIP = "127.0.0.1";
            mClient.ServerPort = 8221;
        }


        private static void TestConnectDisconnect()
        {
            CreateObjects();
            mServer.StartListener();
            
            mClient.StartClient();
            mClient.StopClient();
            
            mClient.StartClient();
            mClient.StopClient();

            mClient.StartClient();
            mClient.StopClient();
        }


        private static void TestSendingCommands()
        {
            CreateObjects();

            mServer.StartListener();
            mClient.StartClient();

            for (int i = 0; i < 10; i++)
            {
                mClient.SendData("This is a really long test message sure to exceed the length of receiving buffer " + i + ".\n");
                mClient.SendData("but this one won't\n");
                mClient.SendData("And this bit is exactly correct!\n");
                mClient.SendData("Short\n");
                mClient.SendData("commands\n");
                mClient.SendData("one\n");
                mClient.SendData("after\n");
                mClient.SendData("another\n");
                System.Threading.Thread.Sleep(5000);
            }
            mClient.StopClient();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using EnigmaMM;

namespace EnigmaMM
{
    class Program
    {
        static void Main(string[] args)
        {
            Server mServer = new Server();
            mServer.StartListener();

            System.Threading.Thread.Sleep(500);

            Client mClient = new Client();
            mClient.ServerIP = "127.0.0.1";
            mClient.ServerPort = 8221;
            mClient.StartClient();
            // Loop and send test commands
            int i;
            for (i = 0; i < 100; i++)
            {
                mClient.SendData("This is a really long test message sure to exceed the length of the receiving buffer " + i + ".\n");
                mClient.SendData("but this one won't " + i + ".\n");
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

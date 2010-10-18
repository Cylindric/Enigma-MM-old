namespace EnigmaMM
{
    class ServerProgram
    {
        static void Main(string[] args)
        {
            Client C = new Client();
            C.StartClient();

            // Loop and send test commands
            int i;
            for (i = 0; i < 10; i++)
            {
                C.SendData("This is a really long test message sure to exceed the length of the receiving buffer " + i);
                C.SendData("and a little bit extra " + i);
                //C.SendData("xxxxxx");
                System.Threading.Thread.Sleep(1000);
            }
            C.StopClient();
        }
    }
}

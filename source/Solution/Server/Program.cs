using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EnigmaMM
{
    class Program
    {
        static void Main(string[] args)
        {
            Server L = new Server();
            L.StartListener();

            while (L.Listening) {
                System.Threading.Thread.Sleep(100);
            }
                
        }
    }
}

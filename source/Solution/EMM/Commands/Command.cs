using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EnigmaMM.Commands
{
    abstract class Command: IDisposable
    {
        protected EMMServer mServer;

        public Command()
        {
            mServer = Factory.GetServer();
        }

        public void Dispose()
        {
            mServer = null;
        }

    }
}

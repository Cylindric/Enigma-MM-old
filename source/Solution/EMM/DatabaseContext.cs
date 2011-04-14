using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EnigmaMM
{
    class DatabaseContext: Data.EMMDataContext
    {
        public DatabaseContext(string connection): base(connection)
        {
        }
    }
}
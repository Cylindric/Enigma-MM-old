using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EnigmaMM
{
    class ComandParser
    {
        public String ParseCommand(String Command)
        {
            String ReturnValue = "";

            switch (Command)
            {
                case "stop":
                    ReturnValue = "Server stopped";
                    break;

                case "list":
                    ReturnValue = "Online Users: Dave, Dee, Dozy";
                    break;
            }

            return ReturnValue;
        }
    }
}

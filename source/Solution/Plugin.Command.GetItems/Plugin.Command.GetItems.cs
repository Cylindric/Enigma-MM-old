using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Interfaces.BaseClasses;

namespace EnigmaMM.Plugin.Implementation
{
    public class GetItemsCommand: CommandPlugin
    {
        public GetItemsCommand()
        {
            base.Name = "GetItems";
            base.Tag = "get-items";
        }

        public override bool ParseCommand(string command)
        {
            bool parsed = false;
            if (command.StartsWith("get "))
            {
                Server.RaiseServerMessage("{0}: {1}", Name, command);
                parsed = true;
            }
            return parsed;
        }
    }
}

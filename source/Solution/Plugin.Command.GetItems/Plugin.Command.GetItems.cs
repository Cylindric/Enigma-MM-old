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

        public override bool ParseCommand(Interfaces.IUser user, string command)
        {
            bool parsed = false;
            if (command.StartsWith("get "))
            {
                Server.RaiseServerMessage("{0}: {1}", Name, command);
                parsed = true;
            }
            return parsed;
        }

        private void GiveItem(string username, int itemId, int qty)
        {
            int qtyToGive = qty;
            int giveStep = 64;

            while (qtyToGive > 0)
            {
                int give = Math.Min(qtyToGive, giveStep);
                Server.Execute(string.Format("give {0} {1} {2}", username, itemId, give));
                qtyToGive = qtyToGive - give;
            }

        }

    }
}

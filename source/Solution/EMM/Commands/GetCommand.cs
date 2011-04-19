using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnigmaMM.Data;

namespace EnigmaMM.Commands
{
    class GetCommand: Command
    {
        private const int MAX_GIVE_STEP = 64;

        public GetCommand()
        {
            mPermissionsRequired.Add(mDB.Permissions.Single(i => i.Name == "get-item"));
            mPermissionsRequired.Add(mDB.Permissions.Single(i => i.Name == "reboot"));
        }

        public bool Execute(EMMServerMessage command)
        {
            string[] parameters = command.Message.Split(' ');
            string itemName = "";
            int quantity = 0;
            if (parameters.Count() >= 2)
            {
                itemName = parameters[1];
            }
            if (parameters.Count() >= 3)
            {
                int.TryParse(parameters[2], out quantity);
            }

            int itemID = 0;
            int.TryParse(itemName, out itemID);
            Item item;
            if (itemID != 0)
            {
                item = mDB.Items.SingleOrDefault(i => i.Block_Decimal_ID == itemID);
            }
            else
            {
                item = mDB.Items.SingleOrDefault(i => i.Code == itemName);
            }
            if (item == null)
            {
                return false;
            }

            return Execute(command.User, item, quantity);
        }

        public bool Execute(User user, Item item, int quantity)
        {
            if (CheckAccess(user) == false)
            {
                return false;
            }

            int qtyToGive = GetActualQuantity(user, item, quantity);

            if (qtyToGive == 0)
            {
                return false;
            }

            while (qtyToGive > 0)
            {
                int give = Math.Min(qtyToGive, MAX_GIVE_STEP);
                mServer.Execute(string.Format("give {0} {1} {2}", user.Username, item.Block_Decimal_ID, give));
                qtyToGive = qtyToGive - give;
            }

            return true;
        }

        private int GetActualQuantity(User user, Item item, int requestedQuantity)
        {
            int finalQuantity = 0;

            if (requestedQuantity == 0)
            {
                finalQuantity = item.Stack_Size;
            }
            finalQuantity = Math.Min(finalQuantity, item.Max);

            return finalQuantity;
        }

    }
}

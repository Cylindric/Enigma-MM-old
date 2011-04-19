using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EnigmaMM.Commands
{
    class GetItems: Command
    {
        private void GetItem(string username, int itemId, int qty)
        {
            int qtyToGive = qty;
            int giveStep = 64;

            EMMServer server = Factory.GetServer();
            while (qtyToGive > 0)
            {
                int give = Math.Min(qtyToGive, giveStep);
                server.Execute(string.Format("give {0} {1} {2}", username, itemId, give));
                qtyToGive = qtyToGive - give;
            }
        }

        private void ParseRequestCommand(string username, string item, string quantity = null)
        {
            //int finalQuantity = 0;
            //int finalItemId = 0;

            //Item foundItem = sItems.Find(i => i.Code == item);
            //if (foundItem != null)
            //{
            //    finalItemId = foundItem.Id;
            //    int.TryParse(quantity, out finalQuantity);
            //    if (quantity == null)
            //    {
            //        finalQuantity = foundItem.Quantity;
            //    }
            //    finalQuantity = Math.Min(finalQuantity, foundItem.MaxQuantity);
            //}

            //mMinecraft.GiveItem(username, finalItemId, finalQuantity);
        }

    }
}

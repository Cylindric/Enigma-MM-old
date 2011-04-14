using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EnigmaMM
{
    class ItemHistory: Data.ItemHistory
    {
        public ItemHistory(Item item, User user, int quantity)
        {
            this.Item = item;
            this.User = user;
            this.Quantity = quantity;
            this.CreateDate = DateTime.Now;
        }
    }
}

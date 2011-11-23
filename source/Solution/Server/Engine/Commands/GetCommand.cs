using System;
using System.Linq;
using EnigmaMM.Engine.Data;

namespace EnigmaMM.Engine.Commands
{
    class GetCommand: Command
    {
        private const int MAX_GIVE_STEP = 64;

        public GetCommand()
        {
            mPermissionsRequired.Add(Manager.GetContext.Permissions.Single(i => i.Name == "get-item"));
        }

        protected override void ExecuteTask(EMMServerMessage command)
        {
            string[] parameters = command.Data["command"].Split(' ');
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
            
            using (EMMDataContext mDB = Manager.GetContext)
            {
                if (itemID != 0)
                {
                    item = mDB.Items.SingleOrDefault(i => i.Block_Decimal_ID == itemID);
                }
                else
                {
                    item = mDB.Items.SingleOrDefault(i => i.Code == itemName);
                }
            }

            if (item == null)
            {
                Manager.Server.Whisper(command.User, "I don't know what item that is");
                return;
            }

            if (item.Min_Level > command.User.Rank.Level)
            {
                Manager.Server.Whisper(command.User, "You are not allowed to summon that item");
                return;
            }

            ExecuteTask(command.User, item, quantity);
        }

        private void ExecuteTask(Data.User user, Item item, int quantity)
        {
            int qtyToGive = GetActualQuantity(user, item, quantity);

            if (qtyToGive == 0)
            {
                return;
            }

            Manager.Server.Whisper(user, string.Format("Giving you {0} {1}",  qtyToGive, item.Name));

            int remainingQuantity = qtyToGive;
            while (remainingQuantity > 0)
            {
                int give = Math.Min(qtyToGive, MAX_GIVE_STEP);
                Manager.Server.Execute(string.Format("give {0} {1} {2}", user.Username, item.Block_Decimal_ID, give));
                remainingQuantity = remainingQuantity - give;
            }

            ItemHistory history = new ItemHistory();
            history.Item = item;
            history.User = user;
            history.Quantity = qtyToGive;
            history.CreateDate = DateTime.Now;
            using (EMMDataContext db = Manager.GetContext)
            {
                db.ItemHistories.InsertOnSubmit(history);
                db.SubmitChanges();
            }
        }

        private int GetActualQuantity(Data.User user, Item item, int requestedQuantity)
        {
            int finalQuantity = requestedQuantity;

            if (requestedQuantity == 0)
            {
                finalQuantity = item.Stack_Size;
            }
            finalQuantity = Math.Min(finalQuantity, item.Max);

            return finalQuantity;
        }

    }
}

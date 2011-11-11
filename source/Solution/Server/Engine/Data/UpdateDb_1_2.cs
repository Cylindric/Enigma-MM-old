using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EnigmaMM.Engine.Data
{
    static class UpdateDb_1_2
    {
        private static Data.EMMDataContext mDb = Manager.Database;

        public static void DoUpdate()
        {
            updateItem(1, 4, 64, 256, "stone", "Stone Block");
            updateItem(2, 4, 64, 256, "grass", "Grass");
            mDb.Configs.FirstOrDefault(c => c.Key == "db_version").Value = "2";
            mDb.SubmitChanges();
        }
       
        /// <summary>
        /// Update the specified item, or it doesn't exist, insert it.
        /// </summary>
        /// <param name="block_id"></param>
        /// <param name="min_rank_id"></param>
        /// <param name="stack"></param>
        /// <param name="max_stack"></param>
        /// <param name="code"></param>
        /// <param name="name"></param>
        private static void updateItem(int block_id, int min_rank_id, int stack, int max_stack, string code, string name)
        {
            Data.Item item = mDb.Items.SingleOrDefault(i => i.Block_Decimal_ID == block_id);

            if (item == null)
            {
                mDb.Items.InsertOnSubmit(new Data.Item()
                {
                    Code = code,
                    Name = name,
                    Block_Decimal_ID = block_id,
                    Min_Rank_ID = min_rank_id,
                    Stack_Size = stack,
                    Max = max_stack
                });
            }
            else
            {
                item.Block_Decimal_ID = block_id;
                item.Code = code;
                item.Max = max_stack;
                item.Min_Rank_ID = min_rank_id;
                item.Name = name;
                item.Stack_Size = stack;
            }
            mDb.SubmitChanges();
        }

    }
}

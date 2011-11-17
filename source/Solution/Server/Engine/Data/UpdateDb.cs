using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EnigmaMM.Engine.Data
{
    class UpdateDb
    {
        protected static Data.EMMDataContext mDb = Manager.Database;

        protected static void UpdateConfig(string key, string value)
        {
            Data.Config record = mDb.Configs.SingleOrDefault(r => r.Key == key);
            if (record == null)
            {
                mDb.Configs.InsertOnSubmit(new Data.Config() { Key = key, Value = value });
            }
            else
            {
                record.Value = value;
            }
        }

        protected static void InsertMessage(string name, string expression, string match)
        {
            mDb.MessageTypes.InsertOnSubmit(new Data.MessageType() { Name = name, Expression = expression, MatchType = match });
        }

        protected static void InsertPermission(int rank, string name)
        {
            mDb.Permissions.InsertOnSubmit(new Data.Permission() { Rank_ID = rank, Name = name });
        }

        protected static void InsertRank(string name)
        {
            mDb.Ranks.InsertOnSubmit(new Data.Rank() { Name = name });
        }

        protected static void InsertUser(string name, Data.Rank rank)
        {
            mDb.Users.InsertOnSubmit(new Data.User() { Username = name, Rank = rank });
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
        protected static void updateItem(int block_id, int min_rank_id, int stack, int max_stack, string code, string name)
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

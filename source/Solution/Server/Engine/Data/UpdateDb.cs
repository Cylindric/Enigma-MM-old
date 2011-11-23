using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data.SqlServerCe;

namespace EnigmaMM.Engine.Data
{
    public abstract class UpdateDb
    {
        protected Data.EMMDataContext mDb = Manager.GetContext;
        protected List<string> mCommandQueue = new List<string>();
        protected string mConnectionString;
        protected string mDataFile;

        protected UpdateDb()
        {
            mDataFile = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase.Substring(8)), "data.sdf");
            mConnectionString = string.Format("Data Source = \"{0}\"", mDataFile);
        }

        public abstract void DoUpdate();

        protected void UpdateConfig(string key, string value)
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

        protected void InsertMessage(string name, string expression, string match)
        {
            mDb.MessageTypes.InsertOnSubmit(new Data.MessageType() { Name = name, Expression = expression, MatchType = match });
        }

        protected void InsertItem(int blockDecimalId, string code, string name, int stackSize, int max, int minLevel)
        {
            mDb.Items.InsertOnSubmit(new Data.Item() 
            { 
                Block_Decimal_ID = blockDecimalId,
                Max = max,
                Code = code,
                Name = name,
                Min_Level = minLevel,
                Stack_Size = stackSize
            });
        }

        protected void InsertPermission(int minLevel, string name)
        {
            mDb.Permissions.InsertOnSubmit(new Data.Permission() { Min_Level = minLevel, Name = name });
        }

        protected void InsertRank(int level, string name)
        {
            mDb.Ranks.InsertOnSubmit(new Data.Rank() { Level = level, Name = name });
        }

        protected void InsertUser(string name, Data.Rank rank)
        {
            mDb.Users.InsertOnSubmit(new Data.User() { Username = name, Rank = rank });
        }

        /// <summary>
        /// Update the specified item, or it doesn't exist, insert it.
        /// </summary>
        /// <param name="block_id"></param>
        /// <param name="min_level"></param>
        /// <param name="stack"></param>
        /// <param name="max_stack"></param>
        /// <param name="code"></param>
        /// <param name="name"></param>
        protected void updateItem(int block_id, int min_level, int stack, int max_stack, string code, string name)
        {
            Data.Item item = mDb.Items.SingleOrDefault(i => i.Block_Decimal_ID == block_id);

            if (item == null)
            {
                mDb.Items.InsertOnSubmit(new Data.Item()
                {
                    Code = code,
                    Name = name,
                    Block_Decimal_ID = block_id,
                    Min_Level = min_level,
                    Stack_Size = stack,
                    Max = max_stack
                });
            }
            else
            {
                item.Block_Decimal_ID = block_id;
                item.Code = code;
                item.Max = max_stack;
                item.Min_Level = min_level;
                item.Name = name;
                item.Stack_Size = stack;
            }
            mDb.SubmitChanges();
        }

        protected void ExecuteCommands()
        {
            using (SqlCeConnection con = new SqlCeConnection(mConnectionString))
            {
                con.Open();
                foreach (string sql in mCommandQueue)
                {
                    using (SqlCeCommand cmd = new SqlCeCommand(sql, con))
                    {
                        cmd.CommandText = sql;
                        cmd.ExecuteNonQuery();
                    }
                }
            }

        }

    }
}

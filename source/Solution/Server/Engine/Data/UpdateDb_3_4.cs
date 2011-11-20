using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EnigmaMM.Engine.Data
{
    class UpdateDb_3_4 : UpdateDb
    {
        public override void DoUpdate()
        {
            mCommandQueue.Clear();

            string[] tables = { "Tracking", "Users" };
            string[] cols = { "LocX", "LocY", "LocZ" };

            foreach (string table in tables)
            {
                foreach (string col in cols)
                {
                    mCommandQueue.Add(string.Format("ALTER TABLE {0} ADD COLUMN {1}tmp float", table, col));
                    mCommandQueue.Add(string.Format("UPDATE {0} SET {1}tmp={1}", table, col));
                    mCommandQueue.Add(string.Format("ALTER TABLE {0} DROP COLUMN {1}", table, col));
                    mCommandQueue.Add(string.Format("ALTER TABLE {0} ADD COLUMN {1} int NOT NULL DEFAULT 0", table, col));
                    mCommandQueue.Add(string.Format("UPDATE {0} SET {1}=FLOOR({1}tmp)", table, col));
                    mCommandQueue.Add(string.Format("ALTER TABLE {0} DROP COLUMN {1}tmp", table, col));
                }
            }

            ExecuteCommands();
            mDb.Configs.First(c => c.Key == "db_version").Value = "4";
        }
    }
}

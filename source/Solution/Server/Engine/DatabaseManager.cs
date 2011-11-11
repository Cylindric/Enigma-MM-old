using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EnigmaMM.Engine
{
    class DatabaseManager
    {
        Data.EMMDataContext mDb;
        int currentVersion = 2;

        public void CheckDatabaseState()
        {
            mDb = Manager.Database;
            UpdateDatabase();
            CheckConfigSettings();
            mDb.SubmitChanges();
        }

        private void CheckConfigSettings()
        {
            this.CheckConfigSetting("map_small_width", "250");
            this.CheckConfigSetting("c10t_exe", "./c10t/c10t.exe");
            this.CheckConfigSetting("overviewer_exe", "./overviewer/gmap.exe");
        }

        private void CheckConfigSetting(string key, string value)
        {
            Data.Config foundValue = mDb.Configs.FirstOrDefault(c => c.Key == key);
            if (foundValue == null)
            {
                Data.Config config = new Data.Config();
                config.Key = key;
                config.Value = value;
                mDb.Configs.InsertOnSubmit(config);
            }
        }

        private void UpdateDatabase()
        {
            int currentDbVersion = GetCurrentDbVersion();

            if (currentDbVersion <= 1)
            {
                // Upgrade from 1 to 2
                // No schema changes, just some new data to support Beta 1.9 Pre-Release 5
                Data.UpdateDb_1_2.DoUpdate();
            }
        }

        private int GetCurrentDbVersion()
        {
            return int.Parse(mDb.Configs.First(c => c.Key == "db_version").Value);
        }

    }
}

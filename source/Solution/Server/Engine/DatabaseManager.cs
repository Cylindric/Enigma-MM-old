using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EnigmaMM.Engine
{
    class DatabaseManager
    {
        Data.EMMDataContext mDb;

        public void CheckDatabaseState()
        {
            mDb = Manager.Database;
            double currentVersion = 0;
            double.TryParse(mDb.Configs.First(c => c.Key == "db_version").Value, out currentVersion);

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

    }
}

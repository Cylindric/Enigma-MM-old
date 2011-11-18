using System.IO;
using System.Linq;
using EnigmaMM.Engine.Data;

namespace EnigmaMM.Engine
{
    class DatabaseManager
    {
        Data.EMMDataContext mDb;
        public const int CURRENT_VERSION = 3;
        string datafile = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase.Substring(8)), "data.sdf");

        public void CheckDatabaseState()
        {
            mDb = Manager.Database;
            if (!System.IO.File.Exists(datafile))
            {
                UpdateDb creator = new CreateDb();
                creator.DoUpdate();

                UpdateDb inserter = new InsertData();
                inserter.DoUpdate();

                mDb.Configs.First(c => c.Key == "db_version").Value = CURRENT_VERSION.ToString();
            }
            UpdateDatabase();
            mDb.SubmitChanges();
        }

        private void UpdateDatabase()
        {
            if (GetCurrentDbVersion() < CURRENT_VERSION)
            {
                if (GetCurrentDbVersion() < 3)
                {
                    new UpdateDb_2_3().DoUpdate();
                }
            }
        }

        private int GetCurrentDbVersion()
        {
            return int.Parse(mDb.Configs.First(c => c.Key == "db_version").Value);
        }

    }
}

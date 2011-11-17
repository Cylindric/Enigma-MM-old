using System.IO;
using System.Linq;
using EnigmaMM.Engine.Data;

namespace EnigmaMM.Engine
{
    class DatabaseManager
    {
        Data.EMMDataContext mDb;
        public const int CURRENT_VERSION = 2;
        string datafile = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase.Substring(8)), "data.sdf");

        public DatabaseManager()
        {
            if (!System.IO.File.Exists(datafile))
            {
                CreateDb.DoCreate(datafile);
                InsertData.DoInsert();
                mDb.Configs.First(c => c.Key == "db_version").Value = CURRENT_VERSION.ToString();
            }
        }

        public void CheckDatabaseState()
        {
            mDb = Manager.Database;
            UpdateDatabase();
            mDb.SubmitChanges();
        }

        private void UpdateDatabase()
        {
        }

        private int GetCurrentDbVersion()
        {
            return int.Parse(mDb.Configs.First(c => c.Key == "db_version").Value);
        }

    }
}

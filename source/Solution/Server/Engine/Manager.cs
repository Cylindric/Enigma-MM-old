using EnigmaMM.Engine.Data;

namespace EnigmaMM.Engine
{
    public class Manager
    {
        private static volatile EMMServer sEmm;
        private static object sEmmSync = new object();

        private static volatile DatabaseManager sDbm;
        private static object sDatabaseSync = new object();

        public static EMMServer Server
        {
            get
            {
                if (sEmm == null)
                {
                    lock (sEmmSync)
                    {
                        if (sEmm == null)
                        {
                            sEmm = new EMMServer();
                        }
                    }
                }
                return sEmm;
            }
        }

        /// <summary>
        /// Returns a new Database Context for the primary database, and performs 
        /// database checks the first time it's used.
        /// </summary>
        public static EMMDataContext Database
        {
            get
            {
                if (sDbm == null)
                {
                    lock (sDatabaseSync)
                    {
                        if (sDbm == null)
                        {
                            sDbm = new DatabaseManager();
                            sDbm.CheckDatabaseState();
                        }
                    }
                }
                return new EMMDataContext("data.sdf");;
            }
        }
    }

}

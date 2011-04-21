using EnigmaMM.Engine.Data;

namespace EnigmaMM.Engine
{
    public class Manager
    {
        private static volatile EMMServer sEmm;
        private static object sEmmSync = new object();
        private static volatile EMMDataContext sDatabase;
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

        public static EMMDataContext Database
        {
            get
            {
                if (sDatabase == null)
                {
                    lock (sDatabaseSync)
                    {
                        if (sDatabase == null)
                        {
                            sDatabase = new EMMDataContext("EMM.sdf");
                        }
                    }
                }
                return sDatabase;
            }
        }
    }

}

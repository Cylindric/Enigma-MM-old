namespace EnigmaMM
{
    public class Factory
    {
        private static volatile EMMServer sEmm;
        private static object sEmmSync = new object();

        public static EMMServer GetServer()
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
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EnigmaMM.Commands
{
    abstract class Command: IDisposable
    {
        protected EMMServer mServer;
        protected List<Data.Permission> mPermissionsRequired = new List<Data.Permission>();
        protected EnigmaMM.DatabaseContext mDB;

        public Command()
        {
            mServer = Factory.GetServer();
            mDB = EMMServer.Database;
        }

        public void Dispose()
        {
            mServer = null;
        }

        protected bool CheckAccess(Data.User user)
        {
            if (mPermissionsRequired.Count == 0)
            {
                return true;
            }

            var userPermissions = from p in mDB.Permissions
                                  where p.Rank.Rank_ID <= user.Rank.Rank_ID
                                  && mPermissionsRequired.Contains(p)
                                  select p;

            if (userPermissions.Count() == mPermissionsRequired.Count)
            {
                return true;
            }
            else
            {
                mServer.RaiseServerMessage("Access to command denied");
                return false;
            }
        }
    }
}

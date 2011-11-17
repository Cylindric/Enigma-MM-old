using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using EnigmaMM.Engine.Data;

namespace EnigmaMM.Engine.Commands
{
    abstract class Command: IDisposable
    {
        protected List<Data.Permission> mPermissionsRequired = new List<Data.Permission>();

        public Command()
        {
        }

        public void Dispose()
        {
        }

        public void Execute(EMMServerMessage serverMessage)
        {
            if (CheckAccess(serverMessage.User))
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(ExecuteTask), serverMessage);
            }
        }

        private void ExecuteTask(object param)
        {
            ExecuteTask((EMMServerMessage)param);
        }

        /// <summary>
        /// Method that will be called to perform the command
        /// </summary>
        /// <param name="serverMessage">User's full command</param>
        abstract protected void ExecuteTask(EMMServerMessage serverMessage);

        protected bool CheckAccess(Data.User user)
        {
            EMMDataContext mDB = Manager.Database;
            if (mPermissionsRequired.Count == 0)
            {
                return true;
            }

            int userPermissions = (from p in mDB.Permissions
                                  where p.Min_Level <= user.Rank.Level
                                  && mPermissionsRequired.Contains(p)
                                  select p).Count();

            if (userPermissions == mPermissionsRequired.Count)
            {
                return true;
            }
            else
            {
                Manager.Server.RaiseServerMessage("Access to command denied");
                return false;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using EnigmaMM.Engine.Data;

namespace EnigmaMM.Engine.Gui
{
    public class ObservableUsers : ObservableCollection<Data.User>
    {
        private EMMDataContext dataDC;
        private bool ignoreEvents;


        public ObservableUsers()
        {
            this.ignoreEvents = true;
            this.dataDC = new EMMDataContext("data.sdf");

            var userList = from user in dataDC.Users
                           orderby user.Username
                           select user;

            foreach (User user in userList)
            {
                this.Add(user);
            }

            ignoreEvents = false;


        }

        protected override void OnCollectionChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (!ignoreEvents)
            {
                switch (e.Action)
                {
                    case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                        foreach (User user in e.NewItems)
                        {
                            dataDC.Users.InsertOnSubmit(user);
                        }
                        break;

                    case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                        foreach (User user in e.NewItems)
                        {
                            dataDC.Users.DeleteOnSubmit(user);
                        }
                        break;
                    case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                        break;
                }
            }
            base.OnCollectionChanged(e);
        }

        public void Save()
        {
            if (this.dataDC != null)
            {
                this.dataDC.SubmitChanges();
            }
        }
    
    }
}

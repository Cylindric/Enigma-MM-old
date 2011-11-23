using System.Windows;
using EnigmaMM.Engine;
using EnigmaMM.Engine.Data;

    namespace EnigmaMM
    {
        public partial class UsersForm : Window
        {
            private EMMDataContext mDb = Manager.GetContext;

            public UsersForm()
            {
                InitializeComponent();
                dataGrid1.DataContext = mDb.Users;              
            }
        }
    }

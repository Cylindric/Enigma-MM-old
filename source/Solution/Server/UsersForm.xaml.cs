using System.Windows;

    namespace EnigmaMM
    {
        public partial class UsersForm : Window
        {
            public UsersForm()
            {
                InitializeComponent();
                dataGrid1.DataContext = EnigmaMM.Engine.Manager.Database.Users;              
            }
        }
    }

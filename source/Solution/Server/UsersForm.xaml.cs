using System.Windows;
using System.Windows.Data;
using EnigmaMM.Engine.Gui;

    namespace EnigmaMM
    {
        public partial class UsersForm : Window
        {
            private ObservableUsers users;

            public UsersForm()
            {
                InitializeComponent();
            }

            private void Window_Loaded(object sender, RoutedEventArgs e)
            {
                users = new ObservableUsers();
                usersDataGrid.ItemsSource = users;
            }

            private void SaveChanges_Click(object sender, RoutedEventArgs e)
            {
                users.Save();
            }

        }
    }

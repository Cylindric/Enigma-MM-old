using System;
using System.Timers;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Collections;
using EnigmaMM.Interfaces;

namespace EnigmaMM
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private EMMServer mMinecraft;
        private CommandParser mParser;
        private const int MAX_LOG_ENTRIES = 100;

        private InvokeOC<LogListItem> mLogItems;
        private object mLogItemLock;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Initialise()
        {
            // Setup the log viewer
            mLogItemLock = new object();
            mLogItems = new InvokeOC<LogListItem>(uxLogListView.Dispatcher);
            uxLogListView.ItemsSource = mLogItems;

            // Setup the server manager
            mMinecraft = new EMMServer();
            mMinecraft.ServerMessage += HandleServerMessage;
            mMinecraft.StatusChanged += HandleServerMessage;
            mMinecraft.StartCommsServer();
            mParser = new CommandParser(mMinecraft);
        }

        private delegate void UpdateServerMetricsDelegate();
        private void UpdateServerMetrics()
        {
            if (this.Dispatcher.CheckAccess())
            {
                SetStartButtonState(false);
                SetStopButtonState(false);
                SetRestartButtonState(false);

                uxRestartButton.IsEnabled = false;
                uxStatusBarStatus.Text = mMinecraft.CurrentStatus.ToString();


                switch (mMinecraft.CurrentStatus)
                {
                    case EMMServer.Status.Starting:
                        uxStatusBarStatusIcon.Source = new BitmapImage(new Uri("/Resources/status-starting.png", UriKind.Relative));
                        break;

                    case EMMServer.Status.Stopping:
                        uxStatusBarStatusIcon.Source = new BitmapImage(new Uri("/Resources/status-stopping.png", UriKind.Relative));
                        break;

                    case EMMServer.Status.Running:
                        uxStatusBarStatusIcon.Source = new BitmapImage(new Uri("/Resources/status-running.png", UriKind.Relative));
                        SetStopButtonState(true);
                        SetRestartButtonState(true);
                        break;

                    case EMMServer.Status.PendingRestart:
                        uxStatusBarStatusIcon.Source = new BitmapImage(new Uri("/Resources/status-pendingrestart.png", UriKind.Relative));
                        SetStopButtonState(true);
                        break;

                    case EMMServer.Status.PendingStop:
                        uxStatusBarStatusIcon.Source = new BitmapImage(new Uri("/Resources/status-pendingstop.png", UriKind.Relative));
                        SetStopButtonState(true);
                        SetRestartButtonState(true);
                        break;

                    case EMMServer.Status.Stopped:
                        uxStatusBarStatusIcon.Source = new BitmapImage(new Uri("/Resources/status-stopped.png", UriKind.Relative));
                        SetStartButtonState(true);
                        break;

                    case EMMServer.Status.Failed:
                        uxStatusBarStatusIcon.Source = new BitmapImage(new Uri("/Resources/status-failed.png", UriKind.Relative));
                        SetStartButtonState(true);
                       break;
                }

                uxStatusBarUsers.Text = string.Format("Online users: {0}", mMinecraft.Users.Count);

                // Update the user-list
                SynchUserList();

                // Scroll the log
                if (uxLogListView.Items.Count > 0)
                {
                    uxLogListView.SelectedItem = uxLogListView.Items.GetItemAt(uxLogListView.Items.Count - 1);
                    uxLogListView.ScrollIntoView(uxLogListView.SelectedItem);
                }
            }
            else
            {
                this.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new UpdateServerMetricsDelegate(UpdateServerMetrics));
            }
        }

        private void SetStartButtonState(bool enabled)
        {
            uxStartButton.IsEnabled = enabled;
            if (enabled)
            {
                uxStartButtonImage.Source = new BitmapImage(new Uri("/Resources/toolbar-start-active.png", UriKind.Relative));
            }
            else
            {
                uxStartButtonImage.Source = new BitmapImage(new Uri("/Resources/toolbar-start-inactive.png", UriKind.Relative));
            }
        }

        private void SetRestartButtonState(bool enabled)
        {
            uxRestartButton.IsEnabled = enabled;
            if (enabled)
            {
                uxRestartButtonImage.Source = new BitmapImage(new Uri("/Resources/toolbar-restart-active.png", UriKind.Relative));
            }
            else
            {
                uxRestartButtonImage.Source = new BitmapImage(new Uri("/Resources/toolbar-restart-inactive.png", UriKind.Relative));
            }
        }

        private void SetStopButtonState(bool enabled)
        {
            uxStopButton.IsEnabled = enabled;
            if (enabled)
            {
                uxStopButtonImage.Source = new BitmapImage(new Uri("/Resources/toolbar-stop-active.png", UriKind.Relative));
            }
            else
            {
                uxStopButtonImage.Source = new BitmapImage(new Uri("/Resources/toolbar-stop-inactive.png", UriKind.Relative));
            }
        }

        private void SynchUserList()
        {
            // Remove old people
            ArrayList oldItems = new ArrayList();
            for (int localID = 0; localID < uxUserListView.Items.Count; localID++)
            {
                if (!mMinecraft.Users.Contains(uxUserListView.Items[localID]))
                {
                    oldItems.Add(uxUserListView.Items[localID]);
                }
            }
            for (int oldID = 0; oldID < oldItems.Count; oldID++)
            {
                uxUserListView.Items.Remove(oldItems[oldID]);
            }

            // Add missing people
            for (int serverID = 0; serverID < mMinecraft.Users.Count; serverID++)
            {
                if (!uxUserListView.Items.Contains(mMinecraft.Users[serverID]))
                {
                    uxUserListView.Items.Add(mMinecraft.Users[serverID]);
                }
            }
        }

        private void SendServerCommand(string command)
        {
            AddMessageToLog(command);
            mParser.ParseCommand(command);
        }

        private void AddMessageToLog(string message)
        {
            mLogItems.Add(new LogListItem(message));
            if (mLogItems.Count > MAX_LOG_ENTRIES)
            {
                mLogItems.RemoveAt(0);
            }
        }

        private void HandleServerMessage(object sender, ServerMessageEventArgs e)
        {
            if (e.Message.Length > 0)
            {
                AddMessageToLog(e.Message);
            }
            UpdateServerMetrics();
        }

        private void uxStartButton_Click(object sender, RoutedEventArgs e)
        {
            mMinecraft.StartServer();
        }

        private void uxStopButton_Click(object sender, RoutedEventArgs e)
        {
            if (uxGracefulCheckbox.IsChecked.Value)
            {
                mMinecraft.GracefulStop();
            }
            else
            {
                mMinecraft.StopServer();
            }
        }

        private void uxRestartButton_Click(object sender, RoutedEventArgs e)
        {
            if (uxGracefulCheckbox.IsChecked.Value)
            {
                mMinecraft.GracefulRestart();
            }
            else
            {
                mMinecraft.RestartServer();
            }
        }

        private void uxCommandInput_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SendServerCommand(uxCommandInput.Text);
                uxCommandInput.Text = "";
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Initialise();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            mMinecraft.StopServer(60000, true);
        }

        private void uxLogListView_SourceUpdated(object sender, System.Windows.Data.DataTransferEventArgs e)
        {
            uxLogListView.SelectedIndex = uxLogListView.Items.Count - 1;
        }

    }

}

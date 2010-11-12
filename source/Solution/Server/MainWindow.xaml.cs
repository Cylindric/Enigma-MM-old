using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using EnigmaMM;
using System.Collections;
using System.Collections.ObjectModel;
using System.Threading;

namespace EnigmaMM
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MCServer mMinecraft;
        private CommandParser mParser;
        private const int MAX_LOG_ENTRIES = 10;

        private InvokeOC<LogListItem> mLogItems;
        private object mLogItemLock;

        public MainWindow()
        {
            InitializeComponent();
            mLogItemLock = new object();
            mLogItems = new InvokeOC<LogListItem>(uxLogListView.Dispatcher);

            mMinecraft = new MCServer();
            mMinecraft.ServerMessage += HandleServerMessage;
            mParser = new CommandParser(mMinecraft);
            uxLogListView.ItemsSource = mLogItems;
        }

        private void UpdateServerMetrics()
        {
        }

        private void SendServerCommand(string command)
        {
            AddMessageToLog(command);
            mParser.ParseCommand(command);
        }

        private void AddMessageToLog(string message)
        {
            lock (mLogItemLock)
            {
                mLogItems.Add(new LogListItem(message));
                if (mLogItems.Count > MAX_LOG_ENTRIES)
                {
                    mLogItems.RemoveAt(0);
                }
            }
        }

        private delegate void HandleServerMessageDelegate(string message);

        public void HandleServerMessage(string message)
        {
            AddMessageToLog(message);
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

    }
}

using System;
using System.Windows.Forms;
using EnigmaMM;
using System.Drawing;

namespace EnigmaMM
{
    public partial class ServerForm : Form
    {
        private int mMaxLogItems = 100;
        private MCServer mMCServer;
        private CommandParser mParser;

        delegate void HandleServerMessageDelegate(string message);

        public ServerForm()
        {
            InitializeComponent();
        }

        public ServerForm(MCServer server)
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            mMCServer = server;
            mMCServer.ServerMessage += HandleServerMessage;
            mMCServer.StatusChanged += HandleServerStatus;
            mParser = new CommandParser(mMCServer);
            UpdateServerMetrics();
        }

        private void SendServerCommand(string command)
        {
            AddMessageToLog(MessageType.UserCommand, command);
            mParser.ParseCommand(command);
        }


        private void UpdateServerMetrics()
        {
            UpdateServerStatus();
            uxStatusUsersOnlineLabel.Text = string.Format("Users: {0}", mMCServer.OnlineUserCount);

        }

        private void UpdateServerStatus()
        {
            uxStatusServerStatusLabel.Text = mMCServer.CurrentStatus.ToString();
            uxStartButton.Enabled = false;
            uxStopButton.Enabled = false;
            uxRestartButton.Enabled = false;
            switch (mMCServer.CurrentStatus)
            {
                case MCServer.Status.Starting:
                    break;

                case MCServer.Status.Running:
                    uxStopButton.Enabled = true;
                    uxRestartButton.Enabled = true;
                    break;

                case MCServer.Status.PendingRestart:
                    uxStopButton.Enabled = true;
                    break;

                case MCServer.Status.PendingStop:
                    uxStopButton.Enabled = true;
                    uxRestartButton.Enabled = true;
                    break;

                case MCServer.Status.Stopped:
                    uxStartButton.Enabled = true;
                    break;
            }
        }


        private enum MessageType
        {
            ServerMessage,
            UserCommand,
        }


        private void AddMessageToLog(MessageType type, string message)
        {
            uxLogListview.SuspendLayout();
            ListViewItem item = new ListViewItem();
            item.Text = message;
            if (type == MessageType.UserCommand)
            {
                item.ForeColor = Color.FromKnownColor(KnownColor.Red);
            }
            uxLogListview.Items.Add(item);
            if (uxLogListview.Items.Count > mMaxLogItems)
            {
                uxLogListview.Items.RemoveAt(uxLogListview.Items.Count);
            }
            item.Selected = true;
            item.EnsureVisible();
            item.Selected = false;
            uxLogListview.ResumeLayout();
        }


        public void HandleServerMessage(string message)
        {
            if (this.InvokeRequired)
            {
                HandleServerMessageDelegate handler = new HandleServerMessageDelegate(HandleServerMessage);
                this.Invoke(handler, message);
            }
            else
            {
                AddMessageToLog(MessageType.ServerMessage, message);
                UpdateServerMetrics();
            }
        }


        private void HandleServerStatus(string message)
        {
            if (this.InvokeRequired)
            {
                HandleServerMessageDelegate handler = new HandleServerMessageDelegate(HandleServerStatus);
                this.Invoke(handler, message);
            }
            else
            {
                UpdateServerMetrics();
            }
        }


        private void uxStartButton_Click(object sender, EventArgs e)
        {
            mMCServer.StartServer();
        }

        private void uxStopButton_Click(object sender, EventArgs e)
        {
            if (uxGracefulCheck.Checked)
            {
                mMCServer.GracefulStop();
            }
            else
            {
                mMCServer.StopServer();
            }
        }

        private void uxRestartButton_Click(object sender, EventArgs e)
        {
            if (uxGracefulCheck.Checked)
            {
                mMCServer.GracefulRestart();
            }
            else
            {
                mMCServer.RestartServer();
            }
        }

        private void uxCommandInput_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SendServerCommand(uxCommandInput.Text);
                uxCommandInput.Text = "";
            }
        }


    }
}

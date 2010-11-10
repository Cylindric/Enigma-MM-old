using System;
using System.Windows.Forms;
using EnigmaMM;
using System.Drawing;

namespace EnigmaMM
{
    public partial class ServerForm : Form
    {
        private int mMaxLogItems = 100;
        private CommsServer mServer;
        private MCServer mMinecraft;
        private CommandParser mParser;

        delegate void HandleServerMessageDelegate(string message);

        public ServerForm()
        {
            InitializeComponent();
        }

        public ServerForm(CommsServer server)
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            mServer = server;
            mMinecraft = server.Minecraft;
            mMinecraft.ServerMessage += HandleServerMessage;
            mMinecraft.StatusChanged += HandleServerStatus;
            mParser = new CommandParser(mMinecraft);
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
            uxStatusUsersOnlineLabel.Text = string.Format("Users: {0}", mMinecraft.OnlineUserCount);

        }

        private void UpdateServerStatus()
        {
            uxStatusServerStatusLabel.Text = mMinecraft.CurrentStatus.ToString();
            uxStartButton.Enabled = false;
            uxStopButton.Enabled = false;
            uxRestartButton.Enabled = false;
            switch (mMinecraft.CurrentStatus)
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
            mMinecraft.StartServer();
        }

        private void uxStopButton_Click(object sender, EventArgs e)
        {
            if (uxGracefulCheck.Checked)
            {
                mMinecraft.GracefulStop();
            }
            else
            {
                mMinecraft.StopServer();
            }
        }

        private void uxRestartButton_Click(object sender, EventArgs e)
        {
            if (uxGracefulCheck.Checked)
            {
                mMinecraft.GracefulRestart();
            }
            else
            {
                mMinecraft.RestartServer();
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

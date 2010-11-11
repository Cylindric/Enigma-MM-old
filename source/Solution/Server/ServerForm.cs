using System;
using System.Windows.Forms;
using EnigmaMM;
using System.Drawing;
using System.Collections;
using System.Threading;

namespace EnigmaMM
{
    public partial class ServerForm : Form
    {
        private int mMaxLogItems = 100;
        private MCServer mMinecraft;
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

            mMinecraft = server;
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

        private void UpdateOnlineUsers(ArrayList onlineUsers)
        {
            ArrayList changeusers = new ArrayList();
            bool resort = false;

            // remove old users
            foreach (ListViewItem olduser in uxUserListview.Items)
            {
                if (!onlineUsers.Contains(olduser.Text))
                {
                    changeusers.Add(olduser);
                }
            }
            foreach(ListViewItem olduser in changeusers)
            {
                uxUserListview.Items.Remove(olduser);
            }
            changeusers.Clear();


            // add new users
            for (int i = 0; i < onlineUsers.Count; i++)
            {
                string mcuser = onlineUsers[i].ToString();
                if (!uxUserListview.Items.ContainsKey(mcuser))
                {
                    changeusers.Add(mcuser);
                }
            }

            foreach(string newuser in changeusers)
            {
                ListViewItem item = new ListViewItem();
                item.Text = newuser;
                item.Name = newuser;
                uxUserListview.Items.Add(item);
                resort = true;
            }
            changeusers.Clear();

            if (resort)
            {
                uxUserListview.Sort();
            }
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

        private void ServerForm_Load(object sender, EventArgs e)
        {
            if(mMinecraft.Listening)
            {
                AddMessageToLog(MessageType.ServerMessage, "Server is listening for remote commands");
            }
        }

        private void ServerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (mMinecraft.CurrentStatus != MCServer.Status.Stopped)
            {
                // Form is closing, but server is still running!
                // Attempt to stop the server, if after 30 seconds it has not stopped,
                // force it.
                mMinecraft.StopServer(30000, true);

            }
        }


    }
}

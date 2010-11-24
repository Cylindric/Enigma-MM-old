using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.Threading;

namespace MinecraftSimulator
{
    class Simulator
    {
        public bool Running { get; set; }
        public bool Fast { get; set; }
        public bool Turbo { get; set; }
        public bool Lagging { get; set; }
        public bool LowMem { get; set; }
        public List<string> Players { get; set; }

        public delegate void ServerMessageEventHandler(string Message);
        public event ServerMessageEventHandler RaiseCommandReceivedEvent;

        private System.Timers.Timer mIdleTimer;
        private Random mRand;
        private Messages mMsg;
        private DateTime mLastAutoSave;

        private const double RATE_LAG = 0.001;
        private const double RATE_PLAYER_JOINS = 0.05;
        private const double RATE_PLAYER_QUITS = 0.05;
        private const double RATE_PLAYER_DC = 0.0002;
        private TimeSpan AUTO_SAVE_DELAY = new TimeSpan(0, 30, 0);

        public Simulator()
        {
            mIdleTimer = new System.Timers.Timer();
            mRand = new Random();
            mMsg = new Messages(this);
            Fast = true;
            Turbo = false;
            Running = false;
            Lagging = true;
            LowMem = true;
        }

        public void Start()
        {
            Players = new List<string>();
            Running = true;
            mMsg.StartupMessages();
            mMsg.PrepareSpawnArea();

            mLastAutoSave = DateTime.Now;

            mIdleTimer.Elapsed += new ElapsedEventHandler(PerformIdleActions);
            if (Turbo)
            {
                mIdleTimer.Interval = 50;
            }
            else
            {
                mIdleTimer.Interval = 300;
            }
            mIdleTimer.Start();

            while (Running)
            {
                Thread.Sleep(100);
            }
        }

        public void SendCommand(string message)
        {
            SendMessage(message);
            switch (message)
            {
                case "!useradd":
                    UserLogIn();
                    break;

                case "!userdel":
                    UserLogOut();
                    break;

                case "list":
                    mMsg.ConnectedPlayers();
                    break;

                case "save-all":
                    mMsg.Save();
                    break;

                case "save-off":
                    mMsg.SaveOff();
                    break;

                case "save-on":
                    mMsg.SaveOn();
                    break;

                case "stop":
                    mIdleTimer.Enabled = false;
                    mMsg.Stop();
                    Running = false;
                    break;

                default:
                    mMsg.UnknownCommand();
                    break;
            }
        }

        private void PerformIdleActions(object source, ElapsedEventArgs e)
        {
            // always auto-save if the delay has expired
            if (mLastAutoSave.Add(AUTO_SAVE_DELAY) < DateTime.Now)
            {
                mMsg.Save();
                mLastAutoSave = DateTime.Now;
            }

            // if enabled, print a lagging message
            if (Trigger(RATE_LAG) && (Lagging))
            {
                mMsg.Lagging();
            }

            // user logged in
            if (Trigger(RATE_PLAYER_JOINS))
            {
                UserLogIn();
            }

            // user logged out
            if (Trigger(RATE_PLAYER_QUITS))
            {
                UserLogOut();
            }

            // user client closed (crashed or closed without disconnecting)
            if (Trigger(RATE_PLAYER_DC))
            {
                UserDisconnected();
            }
        }

        private void UserLogOut()
        {
            if (Players.Count > 0)
            {
                string username = Players[mRand.Next(0, Players.Count - 1)];
                Players.Remove(username);
                mMsg.PlayerQuit(username);
            }
        }

        private void UserDisconnected()
        {
            if (Players.Count > 0)
            {
                string username = Players[mRand.Next(0, Players.Count - 1)];
                Players.Remove(username);
                mMsg.PlayerDisconnected(username);
            }
        }

        private void UserLogIn()
        {
            string username = string.Format("SomeUser{0}", mRand.Next(111111, 999999));
            Players.Add(username);
            mMsg.PlayerJoined(username);
        }

        private bool Trigger(double odds)
        {
            if (Turbo)
            {
                return mRand.NextDouble() <= (odds * 10);
            }
            else
            {
                return mRand.NextDouble() <= odds;
            }
        }

        public void SendMessage(string message)
        {
            if (RaiseCommandReceivedEvent != null)
            {
                RaiseCommandReceivedEvent(message);
            }
        }

    }
}

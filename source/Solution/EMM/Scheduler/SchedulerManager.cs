using System;
using System.Collections.Generic;
using System.IO;
using System.Timers;
using System.Xml;
using EnigmaMM.Interfaces;

namespace EnigmaMM.Scheduler
{
    public class SchedulerManager
    {
        private const int TIMER_INTERVAL = 60000;
        private IServer mServer;
        private string mFile;
        private List<ScheduleTask> mTasks;
        private Timer mTimer;

        public SchedulerManager()
        {
            mServer = null;
            mFile = "";
            mTasks = new List<ScheduleTask>();
            mTimer = new Timer();
        }

        public SchedulerManager(string file)
            : this()
        {
            mFile = file;
        }

        public SchedulerManager(IServer server) 
            : this()
        {
            mServer = server;
            mFile = Path.Combine(Settings.ServerManagerRoot, "scheduler.xml");
        }

        public ScheduleTask NextTask
        {
            get
            {
                ScheduleTask next = null;
                foreach (ScheduleTask task in mTasks)
                {
                    if ((next == null) || (task.NextRun < next.NextRun))
                    {
                        next = task;
                    }
                }
                return next;
            }
        }

        public void Start()
        {
            mTimer.Interval = TIMER_INTERVAL;
            mTimer.Start();
        }

        public void Stop()
        {
            mTimer.Stop();
        }

        public void LoadSchedule()
        {
            mTasks = new List<ScheduleTask>();
            if ( (mFile.Length == 0) || (!File.Exists(mFile)) )
            {
                return;
            }
            XmlDocument xml = new XmlDocument();
            xml.Load(mFile);
            XmlNodeList nodeList = xml.DocumentElement.SelectNodes("/schedule/task");
            foreach (XmlNode taskNode in nodeList)
            {
                ScheduleTask task = new ScheduleTask();
                task.Name = taskNode.SelectSingleNode("name").InnerText;
                task.Command = taskNode.SelectSingleNode("command").InnerText;
                task.RunDays = taskNode.SelectSingleNode("days").InnerText;
                task.RunHours = taskNode.SelectSingleNode("hours").InnerText;
                task.RunMinutes = taskNode.SelectSingleNode("minutes").InnerText;
                AddTask(task);
            }
        }

        public void AddTask(ScheduleTask task)
        {
            task.CalculateNextRunTime();
            mTasks.Add(task);
        }

        private void onTimerEvent(object source, ElapsedEventArgs e)
        {
            // Run all "missed" tasks and increment their run-times
            foreach (ScheduleTask task in mTasks)
            {
                if(task.NextRun < e.SignalTime)
                {
                    if (mServer != null)
                    {
                        mServer.SendCommand(task.Command);
                    }
                    task.CalculateNextRunTime(e.SignalTime);
                }
            }

            // Determine if the next-run time is sooner than the next scheduled run-time, and if it is shorten the delay.
            DateTime nextTimerEvent = e.SignalTime.AddMilliseconds(mTimer.Interval);
            if (NextTask.NextRun < nextTimerEvent)
            {
                mTimer.Interval = (nextTimerEvent - NextTask.NextRun).TotalMilliseconds;
            }
            else
            {
                mTimer.Interval = TIMER_INTERVAL;
            }

        }
    }
}


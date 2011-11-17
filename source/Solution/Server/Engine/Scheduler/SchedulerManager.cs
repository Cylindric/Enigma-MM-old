using System;
using System.Collections.Generic;
using System.IO;
using System.Timers;
using System.Xml;
using EnigmaMM.Engine;

namespace EnigmaMM.Scheduler
{
    /// <summary>
    /// Provides functionality for managing scheduled tasks that can execute at
    /// pre-defined repeating intervals.
    /// </summary>
    public class SchedulerManager
    {
        private const int TIMER_INTERVAL = 5000;
        private EMMServer mServer;
        private List<ScheduleTask> mTasks;
        private Timer mTimer;

        /// <summary>
        /// Gets or sets the <seealso cref="EMMServer"/> to use for executing the
        /// scheduled commands.
        /// </summary>
        public EMMServer Server
        {
            get { return mServer; }
            set { mServer = value; }
        }

        /// <summary>
        /// Gets the <seealso cref="IScheduleTask"/> that is scheduled to run next.
        /// </summary>
        /// <remarks>If no tasks are scheduled, returns <c>null</c>.</remarks>
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

        /// <summary>
        /// Creates a new ScheduleManager with default values, linked to the 
        /// specified <see cref="EMMServer"/>.
        /// </summary>
        /// <param name="server">The EMMServer to use for executing commands.</param>
        public SchedulerManager(EMMServer server)
        {
            mServer = server;
            mTasks = new List<ScheduleTask>();
            mTimer = new Timer();
            mTimer.Elapsed += onTimerEvent;
        }

        /// <summary>
        /// Starts the scheduler.
        /// </summary>
        public void Start()
        {
            mTimer.Interval = TIMER_INTERVAL;
            processTimer(DateTime.Now);
            mTimer.Start();
        }

        /// <summary>
        /// Stops the scheduler.
        /// </summary>
        public void Stop()
        {
            mTimer.Stop();
        }

        /// <summary>
        /// Parses the file specified by <c>file</c> for tasks, and adds them
        /// to the execution queue.
        /// </summary>
        /// <param name="file">Full path to the schedule file.</param>
        public void LoadSchedule(string file)
        {
            mTasks = new List<ScheduleTask>();
            if ( (file.Length == 0) || (!File.Exists(file)) )
            {
                return;
            }
            XmlDocument xml = new XmlDocument();
            xml.Load(file);
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

        /// <summary>
        /// Adds the specified <see cref="IScheduleTask"/> to the task list.
        /// </summary>
        /// <param name="task">The task to add.</param>
        public void AddTask(ScheduleTask task)
        {
            task.CalculateNextRunTime();
            mTasks.Add(task);
        }


        private void processTimer(DateTime signalTime)
        {
            if (mTasks.Count == 0)
            {
                return;
            }

            // Run all "missed" tasks and increment their run-times
            foreach (ScheduleTask task in mTasks)
            {
                if (task.NextRun <= signalTime)
                {
                    ExecuteTask(task);
                }

                // Determine if the next-run time is sooner than the next scheduled run-time, and if it is shorten the delay.
                DateTime nextTimerEvent = signalTime.AddMilliseconds(mTimer.Interval);
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


        private void onTimerEvent(object source, ElapsedEventArgs e)
        {
            processTimer(e.SignalTime);
        }

        private void ExecuteTask(ScheduleTask task)
        {
            task.CalculateNextRunTime();
            if (mServer != null)
            {
                mServer.RaiseServerMessage(string.Format("Running scheduled task '{0}'.  Next run will be {1}.", task.Name, task.NextRun));
                mServer.Execute(task.Command);
            }
        }

    }
}


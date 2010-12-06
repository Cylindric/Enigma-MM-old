using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;

namespace EnigmaMM.Scheduler
{
    class Scheduler
    {
        private MCServer mServer;
        private string mFile;
        private List<ScheduleTask> mTasks;

        public Scheduler(MCServer server)
        {
            mServer = server;
            mTasks = new List<ScheduleTask>();
        }

        public void LoadSchedule()
        {
            mTasks = new List<ScheduleTask>();
            mFile = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location), "scheduler.xml");
            if (!File.Exists(mFile))
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
                task.CalculateNextRunTime();
            }
        }
    }
}

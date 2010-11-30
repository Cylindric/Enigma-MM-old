using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EnigmaMM.Scheduler
{
    public class ScheduleTask
    {
        public string Name { get; set; }
        public string Command { get; set; }
        public DateTime NextRun { get; private set; }

        public string RunDays { get; set; }
        public string RunHours { get; set; }
        public string RunMinutes { get; set; }

        public ScheduleTask()
        {
        }

        public ScheduleTask(string days, string hours, string minutes)
        {
            RunDays = days;
            RunHours = hours;
            RunMinutes = minutes;
        }

        public ScheduleTask(string days, int hours, int minutes)
        {
            RunDays = days;
            RunHours = hours.ToString();
            RunMinutes = minutes.ToString();
        }

        public ScheduleTask(string days, string hours, int minutes)
        {
            RunDays = days;
            RunHours = hours;
            RunMinutes = minutes.ToString();
        }

        public void CalculateNextRunTime()
        {
            CalculateNextRunTime(DateTime.Now);
        }

        public void CalculateNextRunTime(DateTime start)
        {
            int m = 0;
            int h = 0;
            DayOfWeek d = DayOfWeek.Monday;

            int.TryParse(RunMinutes, out m);
            int.TryParse(RunHours, out h);
            if (Enum.IsDefined(typeof(DayOfWeek), RunDays))
            {
                d = (DayOfWeek)Enum.Parse(typeof(DayOfWeek), RunDays);
            }

            DateTime next = start;
            next = next.AddSeconds(0 - next.Second);
            next = next.AddMinutes(1);

            bool done = false;
            while (!done)
            {
                if (!Match(next.DayOfWeek, RunDays))
                {
                    next = next.AddDays(1);
                    next = next.AddHours(0 - next.Hour); // set hour to zero
                    next = next.AddMinutes(0 - next.Minute); // set minutes to zero
                    continue;
                }

                if (!Match(next.Hour, RunHours))
                {
                    next = next.AddHours(1);
                    next = next.AddMinutes(0 - next.Minute); // set minutes to zero
                    continue;
                }

                if (!Match(next.Minute, RunMinutes))
                {
                    next = next.AddMinutes(1);
                    continue;
                }

                done = true;
            }
            this.NextRun = next;
        }

        private bool Match(DayOfWeek d, string match)
        {
            // The special string "*" always matches
            if (match == "*")
            {
                return true;
            }

            // Iterate through each part of supplied value, for example "Monday", or "Monday,Wednesday,Friday"
            string[] parts = match.Split(',');
            for (int i = 0; i < parts.Length; i++)
            {
                // If this part is numeric, perform a simple comparison
                if (Enum.IsDefined(typeof(DayOfWeek), parts[i]))
                {
                    if ((DayOfWeek)Enum.Parse(typeof(DayOfWeek), parts[i]) == d)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private bool Match(int n, string match)
        {
            int ParsedInt;

            // The special string "*" always matches
            if (match == "*")
            {
                return true;
            }

            // Iterate through each part of supplied value, for example "1", or "0,15,30,45"
            string[] parts = match.Split(',');
            for (int i = 0; i < parts.Length; i++)
            {
                // If this part is numeric, perform a simple comparison
                if (int.TryParse(parts[i], out ParsedInt))
                {
                    if (int.Parse(parts[i].Trim()) == n)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }

}

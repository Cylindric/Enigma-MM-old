using System;
using System.Linq;
using EnigmaMM.Interfaces;

namespace EnigmaMM.Scheduler
{
    /// <summary>
    /// The ScheduleTask class provides the functionality for specifying scheduled tasks
    /// with recurring times they should execute.
    /// </summary>
    public class ScheduleTask : IScheduleTask
    {
        public const string AT_STARTUP = "@startup";
        public const string AT_NEVER = "@never";

        private string mRunDays;
        private string mRunHours;
        private string mRunMinutes;

        /// <summary>
        /// The set of valid parameter types.
        /// </summary>
        private enum ParameterType
        {
            Days,
            Hours,
            Minutes
        }

        #region Public Getters and Setters

        /// <summary>
        /// Gets or sets the name of this task, for logging and management.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the command to send to the server when the schedule is met.
        /// </summary>
        public string Command { get; set; }

        /// <summary>
        /// Gets the next scheduled run time.
        /// </summary>
        public DateTime NextRun { get; private set; }

        /// <summary>
        /// Gets the time this task is next set to run, formatted in natural language.
        /// </summary>
        public string NextRunString { get { return FormatDateAsString(NextRun); } }

        /// <summary>
        /// Gets or sets the Days schedule.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Invalid parameter</exception>
        public string RunDays
        {
            get { return mRunDays; }
            set
            {
                if (!IsValidParameter(ParameterType.Days, value))
                {
                    throw new ArgumentOutOfRangeException(string.Format("Days must be * or a valid set of DayOfWeek but was {0}", value));
                }
                mRunDays = value;
            }
        }

        /// <summary>
        /// Gets or sets the Hours schedule.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Invalid parameter</exception>
        public string RunHours
        {
            get { return mRunHours; }
            set
            {
                if (!IsValidParameter(ParameterType.Hours, value))
                {
                    throw new ArgumentOutOfRangeException(string.Format("Hours must be * or a set of numbers between 0 and 23 but was {0}", value));
                }
                mRunHours = value;
            }
        }

        /// <summary>
        /// Gets or sets the Minutes schedule.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Invalid parameter</exception>
        public string RunMinutes
        {
            get { return mRunMinutes; }
            set {
                if (!IsValidParameter(ParameterType.Minutes, value))
                {
                    throw new ArgumentOutOfRangeException(string.Format("Minutes must be * or a set of numbers between 0 and 59 but was {0}", value));
                }
                mRunMinutes = value; 
            }
        }

        #endregion

        /// <summary>
        /// Initialise a new ScheduleTask with an empty schedule.
        /// </summary>
        public ScheduleTask()
        {
            mRunDays = "";
            mRunHours = "";
            mRunMinutes = "";
            NextRun = DateTime.MaxValue;
        }

        /// <summary>
        /// Initialise a new ScheduleTask with the specified schedule.
        /// </summary>
        /// <param name="days">The days schedule</param>
        /// <param name="hours">The hours schedule</param>
        /// <param name="minutes">The minutes schedule</param>
        public ScheduleTask(string days, string hours, string minutes)
            : this()
        {
            RunDays = days;
            RunHours = hours;
            RunMinutes = minutes;
            CalculateNextRunTime();
        }

        /// <summary>
        /// Initialise a new ScheduleTask with the specified schedule.
        /// </summary>
        /// <param name="days">The days schedule</param>
        /// <param name="hours">The hours schedule</param>
        /// <param name="minutes">The minutes schedule</param>
        public ScheduleTask(string days, int hours, int minutes)
            : this(days, hours.ToString(), minutes.ToString())
        { }

        /// <summary>
        /// Initialise a new ScheduleTask with the specified schedule.
        /// </summary>
        /// <param name="days">The days schedule</param>
        /// <param name="hours">The hours schedule</param>
        /// <param name="minutes">The minutes schedule</param>
        public ScheduleTask(string days, string hours, int minutes)
            : this(days, hours, minutes.ToString())
        { }

        /// <summary>
        /// Calculates the next time this task is due to run, after the current time.
        /// </summary>
        /// <remarks>The calculated run time will be at least one minute in the future.</remarks>
        /// <returns>The calculated next run time.</returns>
        public DateTime CalculateNextRunTime()
        {
            return CalculateNextRunTime(DateTime.Now);
        }

        /// <summary>
        /// Calculates the next time this task is due to run, after the specified time.
        /// </summary>
        /// <remarks>
        /// The calculated run time will be at least one minute later than start.
        /// This method is not optimised.  The loops can be made to finish much
        /// sooner, at the expense of complexity.  As this only runs once each 
        /// time the task is run, it's probably not important.
        /// </remarks>
        /// <param name="start">The start time to use from which to find the next run time.</param>
        /// <returns>The calculated next run time.</returns>
        public DateTime CalculateNextRunTime(DateTime start)
        {
            int m = 0;
            int h = 0;
            DayOfWeek d = DayOfWeek.Monday;

            int.TryParse(RunMinutes, out m);
            int.TryParse(RunHours, out h);
            if (IsDoW(RunDays))
            {
                d = (DayOfWeek)Enum.Parse(typeof(DayOfWeek), RunDays);
            }

            DateTime next = start;
            next = next.AddSeconds(-next.Second); // zero out the unwanted components
            next = next.AddMilliseconds(-next.Millisecond); // zero out the unwanted components
            next = next.AddMinutes(1);

            bool done = false;

            if (RunDays == AT_STARTUP)
            {
                RunDays = AT_NEVER;
                done = true;
            }
            else if (RunDays == AT_NEVER)
            {
                next = DateTime.MaxValue;
                done = true;
            }

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
            return this.NextRun;
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

        private bool IsValidParameter(ParameterType type, string param)
        {
            bool result = true;
            
            // A '*' in any position is the same as 'always'
            if (param.Contains('*'))
            {
                return true;
            }

            // Check each part in a multi-part parameter.  All parts must pass.
            if (param.Contains(','))
            {
                string[] parts = param.Split(',');
                foreach (string part in parts)
                {
                    result = (result && IsValidParameter(type, part));
                }
            }
            else if ((type == ParameterType.Days) && (param.Equals(AT_STARTUP)))
            {
                return true;
            }
            else if ((type == ParameterType.Days) && (param.Equals(AT_NEVER)))
            {
                return true;
            }
            else
            {
                // Check this param
                int intpart;
                switch (type)
                {
                    case ParameterType.Days:
                        result = result && IsDoW(param);
                        break;

                    case ParameterType.Hours:
                        if (int.TryParse(param, out intpart))
                        {
                            result = result && (intpart >= 0 && intpart < 24);
                        }
                        else
                        {
                            result = false;
                        }
                        break;

                    case ParameterType.Minutes:
                        if (int.TryParse(param, out intpart))
                        {
                            result = result && (intpart >= 0 && intpart < 60);
                        }
                        else
                        {
                            result = false;
                        }
                        break;
                }
            }
            return result;
        }

        private bool IsDoW(string s)
        {
            return Enum.IsDefined(typeof(DayOfWeek), s);
        }

        // TODO: fancy string formatting for relative times
        private string FormatDateAsString(DateTime d)
        {
            int TOLLERANCE_HOUR = 5;
            string formatString = "{0}";

            if (d == DateTime.MaxValue)
            {
                return "never";
            }
            return d.ToString();
        }

    }

}

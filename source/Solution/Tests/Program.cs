using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace EnigmaMM.Tests
{
    class Program
    {
        static void Main(string[] args)
        {
            Scheduler.ScheduleTaskTests t = new Scheduler.ScheduleTaskTests();
            t.TestAllDays();
            t.TestAlternateDays();
            t.TestEveryHour();
            t.TestNextDay();
            t.TestNextRun();
        }
    }
}

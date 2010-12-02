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
            ServerTests serverTests = new ServerTests();
            serverTests.FixtureSetup();
            serverTests.TestInitialise();
            serverTests.TestServerRecognisesNewUser();
            serverTests.FixtureTeardown();

            Scheduler.ScheduleTaskTests taskTests = new Scheduler.ScheduleTaskTests();
            taskTests.TestAllDays();
            taskTests.TestAlternateDays();
            taskTests.TestEveryHour();
            taskTests.TestNextDay();
            taskTests.TestNextRun();
        }
    }
}

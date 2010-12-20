using System;
using Moq;
using NUnit.Framework;

namespace EnigmaMM.Scheduler
{
    [TestFixture]
    [Category("Utilities")]
    public class ScheduleTaskTests
    {
        /// <summary>
        /// The mondayMorning variable is used as a convenient placeholder to facilitate
        /// consistent tests.
        /// It is set to early monday morning, on 4th January 2010 at 4:30am
        /// </summary>
        DateTime mondayMorning = new DateTime(2010, 1, 4, 4, 30, 0);

        [Test]
        public void TestUnwantedComponentsAreZero(
            [Range(0, 23, 7)] int h,
            [Range(0, 59, 23)] int m
            )
        {
            ScheduleTask task = new ScheduleTask("*", h, m);
            Assert.That(task.NextRun.Second, Is.EqualTo(0));
            Assert.That(task.NextRun.Millisecond, Is.EqualTo(0));
        }

        [Test]
        public void TestLegalSingleDaysAccepted(
            [Values("Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday")] string day
            )
        {
            ScheduleTask task = new ScheduleTask(day, 0, 0);
            Assert.That(task.RunDays, Is.EqualTo(day));
        }

        [Test]
        public void TestLegalMultipleDaysAccepted(
            [Values("Sunday", "Tuesday,Wednesday", "Thursday,Friday,Saturday", "Wednesday,Monday,Thursday,Friday")] string days
            )
        {
            ScheduleTask task = new ScheduleTask(days, 0, 0);
            Assert.That(task.RunDays, Is.EqualTo(days));
        }

        [Test]
        public void TestLegalSpecialDaysAccepted()
        {
            ScheduleTask task = new ScheduleTask(ScheduleTask.AT_STARTUP, 0, 0);
            Assert.That(task.RunDays, Is.EqualTo(ScheduleTask.AT_NEVER));

            task = new ScheduleTask(ScheduleTask.AT_NEVER, 0, 0);
            Assert.That(task.RunDays, Is.EqualTo(ScheduleTask.AT_NEVER));
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void TestIllegalValuesAreRejected()
        {
            ScheduleTask task;
            task = new ScheduleTask("a", 0, 0); // invalid day
            task = new ScheduleTask("*", -1, 0); // too low hour
            task = new ScheduleTask("*", 0, -1); // too low minute
            task = new ScheduleTask("*", 24, 0); // too high hour
            task = new ScheduleTask("*", 0, 60); // too high minute
            task = new ScheduleTask("", "*", "*"); //empty day
            task = new ScheduleTask("*", "", "*"); //empty hour
            task = new ScheduleTask("*", "*", ""); //empty minute
        }

        [Test]
        public void TestNewTaskSetsNextRun(
            [Values(0, 13, 23)] int h,
            [Values(0, 37, 59)] int m
            )
        {
            ScheduleTask task = new ScheduleTask("Monday", h, m);
            Assert.That(task.NextRun.DayOfWeek, Is.EqualTo(DayOfWeek.Monday));
            Assert.That(task.NextRun.Hour, Is.EqualTo(h));
            Assert.That(task.NextRun.Minute, Is.EqualTo(m));
            Assert.That(task.NextRun.Second, Is.EqualTo(0));
        }

        [Test]
        public void TestNextRun()
        {
            ScheduleTask task = new ScheduleTask("Monday", 5, 30);
 
            task.CalculateNextRunTime(mondayMorning);
            Assert.That(task.NextRun.DayOfWeek, Is.EqualTo(DayOfWeek.Monday));
            Assert.That(task.NextRun.Hour, Is.EqualTo(5));
            Assert.That(task.NextRun.Minute, Is.EqualTo(30));
        }

        [Test]
        public void TestNextDay()
        {
            ScheduleTask task = new ScheduleTask("Tuesday", 5, 30);

            task.CalculateNextRunTime(mondayMorning);
            Assert.That(task.NextRun.DayOfWeek, Is.EqualTo(DayOfWeek.Tuesday));
            Assert.That(task.NextRun.Hour, Is.EqualTo(5));
            Assert.That(task.NextRun.Minute, Is.EqualTo(30));
        }

        [Test]
        public void TestAlternateDays()
        {
            ScheduleTask task = new ScheduleTask("Tuesday,Thursday,Saturday", 5, 30);

            task.CalculateNextRunTime(mondayMorning);
            Assert.That(task.NextRun.DayOfWeek, Is.EqualTo(DayOfWeek.Tuesday));
            Assert.That(task.NextRun.Hour, Is.EqualTo(5));
            Assert.That(task.NextRun.Minute, Is.EqualTo(30));

            task.CalculateNextRunTime(task.NextRun);
            Assert.That(task.NextRun.DayOfWeek, Is.EqualTo(DayOfWeek.Thursday));
            Assert.That(task.NextRun.Hour, Is.EqualTo(5));
            Assert.That(task.NextRun.Minute, Is.EqualTo(30));

            task.CalculateNextRunTime(task.NextRun);
            Assert.That(task.NextRun.DayOfWeek, Is.EqualTo(DayOfWeek.Saturday));
            Assert.That(task.NextRun.Hour, Is.EqualTo(5));
            Assert.That(task.NextRun.Minute, Is.EqualTo(30));
        }

        /// <summary>
        /// Test that if Days is specified as *, then the task will run every day.
        /// </summary>
        [Test]
        public void TestAllDays()
        {
            ScheduleTask task = new ScheduleTask("*", 5, 30);

            task.CalculateNextRunTime(mondayMorning);
            Assert.That(task.NextRun.DayOfWeek, Is.EqualTo(DayOfWeek.Monday));
            Assert.That(task.NextRun.Hour, Is.EqualTo(5));
            Assert.That(task.NextRun.Minute, Is.EqualTo(30));

            task.CalculateNextRunTime(task.NextRun);
            Assert.That(task.NextRun.DayOfWeek, Is.EqualTo(DayOfWeek.Tuesday));
            Assert.That(task.NextRun.Hour, Is.EqualTo(5));
            Assert.That(task.NextRun.Minute, Is.EqualTo(30));

            task.CalculateNextRunTime(task.NextRun);
            Assert.That(task.NextRun.DayOfWeek, Is.EqualTo(DayOfWeek.Wednesday));
            Assert.That(task.NextRun.Hour, Is.EqualTo(5));
            Assert.That(task.NextRun.Minute, Is.EqualTo(30));

            task.CalculateNextRunTime(task.NextRun);
            Assert.That(task.NextRun.DayOfWeek, Is.EqualTo(DayOfWeek.Thursday));
            Assert.That(task.NextRun.Hour, Is.EqualTo(5));
            Assert.That(task.NextRun.Minute, Is.EqualTo(30));

            task.CalculateNextRunTime(task.NextRun);
            Assert.That(task.NextRun.DayOfWeek, Is.EqualTo(DayOfWeek.Friday));
            Assert.That(task.NextRun.Hour, Is.EqualTo(5));
            Assert.That(task.NextRun.Minute, Is.EqualTo(30));

            task.CalculateNextRunTime(task.NextRun);
            Assert.That(task.NextRun.DayOfWeek, Is.EqualTo(DayOfWeek.Saturday));
            Assert.That(task.NextRun.Hour, Is.EqualTo(5));
            Assert.That(task.NextRun.Minute, Is.EqualTo(30));

            task.CalculateNextRunTime(task.NextRun);
            Assert.That(task.NextRun.DayOfWeek, Is.EqualTo(DayOfWeek.Sunday));
            Assert.That(task.NextRun.Hour, Is.EqualTo(5));
            Assert.That(task.NextRun.Minute, Is.EqualTo(30));

            task.CalculateNextRunTime(task.NextRun);
            Assert.That(task.NextRun.DayOfWeek, Is.EqualTo(DayOfWeek.Monday));
            Assert.That(task.NextRun.Hour, Is.EqualTo(5));
            Assert.That(task.NextRun.Minute, Is.EqualTo(30));
        }

        /// <summary>
        /// Test that if Days and Hours are specified as *, then the task will run hourly.
        /// </summary>
        [Test]
        public void TestEveryHour()
        {
            ScheduleTask task = new ScheduleTask("*", "*", 0);

            task.CalculateNextRunTime(mondayMorning);
            for (int i = 5; i < 24; i++)
            {
                Assert.That(task.NextRun.DayOfWeek, Is.EqualTo(DayOfWeek.Monday));
                Assert.That(task.NextRun.Hour, Is.EqualTo(i));
                Assert.That(task.NextRun.Minute, Is.EqualTo(0));
                task.CalculateNextRunTime(task.NextRun);
            }
            for (int i = 0; i < 24; i++)
            {
                Assert.That(task.NextRun.DayOfWeek, Is.EqualTo(DayOfWeek.Tuesday));
                Assert.That(task.NextRun.Hour, Is.EqualTo(i));
                Assert.That(task.NextRun.Minute, Is.EqualTo(0));
                task.CalculateNextRunTime(task.NextRun);
            }
        }

        [Test]
        public void TestStartupNextRunIsSoon()
        {
            ScheduleTask task = new ScheduleTask(ScheduleTask.AT_STARTUP, 0, 0);
            Assert.That(task.NextRun, Is.AtMost(DateTime.Now.AddMinutes(1)));
        }

    }
}

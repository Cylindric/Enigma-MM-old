using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using EnigmaMM.Scheduler;

namespace EnigmaMM.Scheduler
{
    [TestFixture]
    public class ScheduleTaskTests
    {
        /// <summary>
        /// The mondayMorning variable is used as a convenient placeholder to facilitate
        /// consistent tests.
        /// It is set to early monday morning, on 4th January 2010 at 4:30am
        /// </summary>
        DateTime mondayMorning = new DateTime(2010, 1, 4, 4, 30, 0);

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
    }
}

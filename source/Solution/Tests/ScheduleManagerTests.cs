using System;
using EnigmaMM.Interfaces;
using Moq;
using NUnit.Framework;

namespace EnigmaMM.Scheduler
{
    [TestFixture]
    [Category("Utilities")]
    public class ScheduleManagerTests
    {
        /// <summary>
        /// The mondayMorning variable is used as a convenient placeholder to facilitate
        /// consistent tests.
        /// It is set to early monday morning, on 4th January 2010 at 4:30am
        /// </summary>
        private DateTime mondayMorning = new DateTime(2010, 1, 4, 4, 30, 0);

        private Mock<IServer> mockServer;

        [TestFixtureSetUp]
        public void TestInit()
        {
            mockServer = new Mock<IServer>();
        }

        /// <summary>
        /// Trivial test just to make sure the object instantiates successfully.
        /// </summary>
        [Test]
        public void TestSchedulerCreation()
        {
            SchedulerManager scheduler = new SchedulerManager(mockServer.Object);
        }

        [Test]
        public void TestCanAddTask()
        {
            SchedulerManager scheduler = new SchedulerManager(mockServer.Object);
            ScheduleTask task = new ScheduleTask("*", 5, 0);
            scheduler.AddTask(task);
            Console.WriteLine(string.Format("nextTask() is at {0}", scheduler.NextTask.NextRun));
            Assert.That(scheduler.NextTask, Is.EqualTo(task));
        }

        [Test]
        public void TestTasksLoadFromFile()
        {
            SchedulerManager scheduler = new SchedulerManager(mockServer.Object);
        }

        /// <summary>
        /// Tests if an earlier task is added after a later task, 
        /// that nextTask() correctly returns the earlier one.
        /// </summary>
        [Test]
        public void TestNextTaskSequencing()
        {
            SchedulerManager scheduler = new SchedulerManager(mockServer.Object);
            ScheduleTask taskA = new ScheduleTask("*", 6, 0);
            ScheduleTask taskB = new ScheduleTask("*", 5, 0);
            scheduler.AddTask(taskA);
            scheduler.AddTask(taskB);
            Assert.That(scheduler.NextTask, Is.Not.EqualTo(taskA));
            Assert.That(scheduler.NextTask, Is.EqualTo(taskB));
        }

        [Test]
        public void TestTaskExecutes()
        {
            SchedulerManager scheduler = new SchedulerManager(mockServer.Object);
            //mockServer.Verify(s => s.StartServer(), Times.Exactly(1));
        }
    }
}

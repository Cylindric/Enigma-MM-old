using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.IO;
using System.Threading;
using EnigmaMM.Interfaces;

namespace EnigmaMM
{
    [TestFixture]
    [Category("Server")]
    public class ServerTests
    {
        private EMMServer mPersistentServer;
        private const int SLEEP_STEP = 100;

        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            // Reflection within the server will yield the incorrect directory for settings, so we need to set it here.
            string testRoot = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase.Substring(8));
            string settingsFile = Path.Combine(testRoot, "settings.conf");

            using (TextWriter tw = new StreamWriter(settingsFile))
            {
                tw.WriteLine(string.Format("MinecraftRoot={0}", testRoot));
                tw.WriteLine(@"ServerJar=.\MinecraftSimulator.exe");
                tw.Close();
            }
            Console.WriteLine("Settings File is: " + settingsFile);

            mPersistentServer = new EMMServer(settingsFile);
            Assert.That(Settings.Filename, Is.EqualTo(settingsFile));

            mPersistentServer.StartServer();
            int maxWait = 3000;
            while ((mPersistentServer.CurrentStatus != EMMServer.Status.Running) && (maxWait > 0))
            {
                Thread.Sleep(SLEEP_STEP);
                Console.WriteLine("Waiting for running... " + maxWait.ToString());
                maxWait -= SLEEP_STEP;
            }
            Assert.That(mPersistentServer.CurrentStatus, Is.EqualTo(EMMServer.Status.Running), "Expected server to be Running but it wasn't. {0}", mPersistentServer.LastStatusMessage);
        }

        [TestFixtureTearDown]
        public void FixtureTeardown()
        {
            mPersistentServer.StopServer(0, true);
            mPersistentServer = null;
        }

        [Test]
        public void TestServerRecognisesNewUser()
        {
            int startingUsers = mPersistentServer.Users.Count;

            Assert.That(mPersistentServer.CurrentStatus, Is.EqualTo(EMMServer.Status.Running));

            mPersistentServer.SendCommand("!useradd");
            WaitForUserCount(startingUsers + 1);
            Assert.That(mPersistentServer.Users.Count, Is.EqualTo(startingUsers + 1));

            mPersistentServer.SendCommand("!useradd");
            WaitForUserCount(startingUsers + 2);
            Assert.That(mPersistentServer.Users.Count, Is.EqualTo(startingUsers + 2));
        }

        [Test]
        public void TestServerRecognisesRemoveUser()
        {
            int startingUsers = mPersistentServer.Users.Count;

            Assert.That(mPersistentServer.CurrentStatus, Is.EqualTo(EMMServer.Status.Running));

            mPersistentServer.SendCommand("!useradd");
            mPersistentServer.SendCommand("!useradd");
            WaitForUserCount(startingUsers + 2);
            Assert.That(mPersistentServer.Users.Count, Is.EqualTo(startingUsers + 2), "Failed to add two users");

            mPersistentServer.SendCommand("!userdel");
            WaitForUserCount(startingUsers + 1);
            Assert.That(mPersistentServer.Users.Count, Is.EqualTo(startingUsers + 1));

            mPersistentServer.SendCommand("!userdel");
            WaitForUserCount(startingUsers);
            Assert.That(mPersistentServer.Users.Count, Is.EqualTo(startingUsers));
        }

        private void WaitForUserCount(int targetCount)
        {
            int maxWait = 1000;
            while ((mPersistentServer.Users.Count != targetCount) && (maxWait > 0))
            {
                Thread.Sleep(SLEEP_STEP);
                Console.WriteLine("Waiting for user " + maxWait.ToString());
                maxWait -= SLEEP_STEP;
            }
            Assert.That(mPersistentServer.Users.Count, Is.EqualTo(targetCount));
        }
    }
}

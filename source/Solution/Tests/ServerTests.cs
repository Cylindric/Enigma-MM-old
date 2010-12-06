using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.IO;
using System.Threading;

namespace EnigmaMM
{
    [TestFixture]
    public class ServerTests
    {
        private MCServer mPersistentServer;

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

            mPersistentServer = new MCServer(settingsFile);
            Assert.That(Settings.Filename, Is.EqualTo(settingsFile));

            mPersistentServer.ServerMessage += HandleServerMessage;

            mPersistentServer.StartServer();
            int maxWait = 10000;
            while ((mPersistentServer.CurrentStatus != MCServer.Status.Running) && (maxWait > 0))
            {
                Thread.Sleep(500);
                Console.WriteLine("Waiting for running... " + maxWait.ToString());
                maxWait -= 500;
            }
            Assert.That(mPersistentServer.CurrentStatus, Is.EqualTo(MCServer.Status.Running), "Expected server to be Running but it wasn't. {0}", mPersistentServer.LastStatusMessage);
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
            Assert.That(mPersistentServer.CurrentStatus, Is.EqualTo(MCServer.Status.Running));
            Assert.That(mPersistentServer.Users.Count, Is.EqualTo(0), "User count should be zero at start of test, was {0}", mPersistentServer.Users.Count);

            mPersistentServer.SendCommand("!useradd");
            int maxWait = 1000;
            while ((mPersistentServer.Users.Count != 1) && (maxWait > 0))
            {
                Thread.Sleep(100);
                Console.WriteLine("Waiting for user... " + maxWait.ToString());
                maxWait -= 100;
            }
            Assert.That(mPersistentServer.Users.Count, Is.EqualTo(1));
        }


        private void HandleServerMessage(string message)
        {
            Console.Error.WriteLine(message);
        }

    }
}

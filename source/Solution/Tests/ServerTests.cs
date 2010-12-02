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
            string settingsPath = "";
            Assert.That(Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase.Substring(8)), "settings.conf"), Is.EqualTo("jam"), Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase.Substring(8)), "settings.conf"));
            Settings.Initialise(Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase.Substring(8)), "settings.conf"));

            Assert.That(Settings.Loaded, Is.False);
            mPersistentServer = new MCServer();
            Assert.That(Settings.Loaded, Is.True);

            mPersistentServer.StartServer();
            Thread.Sleep(1000);
            Assert.That(mPersistentServer.CurrentStatus, Is.EqualTo(MCServer.Status.Running), "Expected server to be Running but it wasn't. {0}", mPersistentServer.LastStatusMessage);
        }

        [TestFixtureTearDown]
        public void FixtureTeardown()
        {
            mPersistentServer.StopServer(1, true);
            mPersistentServer = null;
        }

        [Test]
        public void TestInitialise()
        {
            using (MCServer server = new MCServer())
            {
                server.StartServer();
            }
        }

        [Test]
        public void TestServerRecognisesNewUser()
        {
            Assert.That(mPersistentServer.Users.Count, Is.EqualTo(0));

            mPersistentServer.SendCommand("!useradd");
            Thread.Sleep(100);
            Assert.That(mPersistentServer.Users.Count, Is.EqualTo(1));
        }
    }
}

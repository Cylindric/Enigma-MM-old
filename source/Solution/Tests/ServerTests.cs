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
            mPersistentServer = new MCServer();
            mPersistentServer.StartServer();
            Thread.Sleep(1000);
            Assert.That(mPersistentServer.CurrentStatus, Is.EqualTo(MCServer.Status.Running));
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

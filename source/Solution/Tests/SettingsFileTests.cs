using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.IO;
using System.Threading;
using EnigmaMM.Engine;

namespace EnigmaMM
{
    [TestFixture]
    [Category("Server")]
    class SettingsFileTests
    {
        private string mSettingsFilename;
        private EMMServer mPersistentServer;
        private SettingsFile mSettings;

        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            mPersistentServer = new EMMServer();

            mSettingsFilename = Path.Combine(Path.GetTempPath(), Path.GetTempFileName());
            using (TextWriter tw = new StreamWriter(mSettingsFilename))
            {
                tw.WriteLine(@"SimpleValue=Banana");
                tw.WriteLine(@"TestSimpleValue=Strawberry");
                tw.WriteLine(string.Format(@"AbsolutePath={0}", mSettingsFilename));
                tw.WriteLine(string.Format(@"RelativePath=.\{0}", Path.GetFileName(mSettingsFilename)));
                tw.Close();
            }

            mSettings = new SettingsFile(mPersistentServer, mSettingsFilename, '=');
            mSettings.LookForNewSettings();
        }

        [TestFixtureTearDown]
        public void FixtureTeardown()
        {
            File.Delete(mSettingsFilename);
        }

        [Test]
        public void TestSimpleValueIsReturned()
        {
            string output = mSettings.GetString("TestSimpleValue");
            Assert.That(output, Is.EqualTo("Strawberry"));
        }

        [Test]
        public void TestDefaultValueIsReturnedForMissing()
        {
            string output = mSettings.GetString("NonExistentKey", "ApplePie");
            Assert.That(output, Is.EqualTo("ApplePie"));
        }

        [Test]
        public void TestDefaultValueIsNotReturnedForPresent()
        {
            string output = mSettings.GetString("SimpleValue", "ApplePie");
            Assert.That(output, Is.EqualTo("Banana"));
        }

        [Test]
        public void TestAbsolutePathValueRoundtrips()
        {
            string output = mSettings.GetRootedPath("", "AbsolutePath");
            Assert.That(output, Is.EqualTo(mSettingsFilename));
            Assert.That(File.Exists(output), Is.True);
        }

        [Test]
        public void TestRelativePathValueRoundtrips()
        {
            string output = mSettings.GetRootedPath(Path.GetDirectoryName(mSettingsFilename), "RelativePath");
            Assert.That(output, Is.EqualTo(mSettingsFilename));
            Assert.That(File.Exists(output), Is.True);
        }

    }
}

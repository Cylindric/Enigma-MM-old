using EnigmaMM.Interfaces;
using EnigmaMM.Plugin.Implementation;
using Moq;
using NUnit.Framework;
using System.IO;

namespace EnigmaMM
{
    [TestFixture]
    [Category("Plugins")]
    class AlphaVespucciPlugin
    {
        private Mock<IServer> mockServer;
        private Mock<ISettings> mockSettings;
        private string mTestRoot;
        private string mOutputPath;
        private string mCachePath;
        private string mWorldPath;

        [TestFixtureSetUp]
        public void TestInit()
        {
            mockServer = new Mock<IServer>();
            mockSettings = new Mock<ISettings>();

            mTestRoot = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase.Substring(8));

            mOutputPath = Path.Combine(Path.Combine(mTestRoot, "Maps"), "av");
            mCachePath = Path.Combine(Path.Combine(mTestRoot, "Cache"), "av");
            mWorldPath = Path.Combine(Path.Combine(mTestRoot, "Minecraft"), "world");

            ResetSettings();

            mockServer.SetupGet(o => o.MinecraftSettings.WorldPath).Returns(mWorldPath);
            mockServer.SetupGet(o => o.Settings.ServerManagerRoot).Returns(mTestRoot);
            mockServer.Setup(o => o.GetSettings(It.IsAny<string>())).Returns(mockSettings.Object);
        }

        [Test]
        public void TestPluginInitialises()
        {
            AlphaVespucci av = new AlphaVespucci();
        }

        [Test]
        public void TestDefaultOutputPathExecutes()
        {
            ResetSettings();
            AlphaVespucci av = new AlphaVespucci();
            av.Initialise(mockServer.Object);
            av.Render();
        }

        [Test]
        public void TestNonDefaultOutputDoesNotCreateDefaultOutputPath()
        {
            ResetSettings();
            mockSettings.Setup(o => o.GetRootedPath(It.IsAny<string>(),
               "OutputPath", It.IsAny<string>()))
               .Returns(Path.Combine(Path.Combine(mTestRoot, "Maps"), "avnondefault"));

            AlphaVespucci av = new AlphaVespucci();
            av.Initialise(mockServer.Object);
            av.Render();

            if (Directory.Exists(Path.Combine(Path.Combine(mTestRoot, "Maps"), "avnondefault")))
            {
                Directory.Delete(Path.Combine(Path.Combine(mTestRoot, "Maps"), "avnondefault"), true);
            }
        }

        private void ResetSettings()
        {
            if (Directory.Exists(mOutputPath))
            {
                Directory.Delete(mOutputPath, true);
            }
            mockSettings.Setup(o => o.GetRootedPath(It.IsAny<string>(),
               "OutputPath", It.IsAny<string>()))
               .Returns(mOutputPath);

            if (Directory.Exists(mCachePath))
            {
                Directory.Delete(mCachePath, true);
            }
            mockSettings.Setup(o => o.GetRootedPath(It.IsAny<string>(),
                "CachePath", It.IsAny<string>()))
                .Returns(mCachePath);

            mockSettings.Setup(o => o.GetRootedPath(It.IsAny<string>(),
                "ExePath", It.IsAny<string>()))
                .Returns(Path.Combine(mTestRoot, "Test.NullCommand.exe"));
        }

    }
}

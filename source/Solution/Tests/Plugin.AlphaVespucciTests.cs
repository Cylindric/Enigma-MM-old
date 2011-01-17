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
        Mock<ISettings> mockSettings;

        [TestFixtureSetUp]
        public void TestInit()
        {
            mockServer = new Mock<IServer>();
            mockSettings = new Mock<ISettings>();

            string testRoot = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().CodeBase.Substring(8));

            string outputpath = Path.Combine(testRoot, "Maps");
            string cachepath = Path.Combine(Path.Combine(testRoot, "Cache"), "av");
            string worldpath = Path.Combine(Path.Combine(testRoot, "Minecraft"), "world");

            mockSettings.Setup(o => o.GetRootedPath(It.IsAny<string>(), 
                "OutputPath", It.IsAny<string>()))
                .Returns(outputpath);
            
            mockSettings.Setup(o => o.GetRootedPath(It.IsAny<string>(), 
                "CachePath", It.IsAny<string>()))
                .Returns(cachepath);

            mockServer.SetupGet(o => o.MinecraftSettings.WorldPath).Returns(worldpath);
            mockServer.SetupGet(o => o.Settings.ServerManagerRoot).Returns(testRoot);
            mockServer.Setup(o => o.GetSettings(It.IsAny<string>())).Returns(mockSettings.Object);
        }

        [Test]
        public void TestPluginInitialises()
        {
            AlphaVespucci av = new AlphaVespucci();
        }

        [Test]
        public void TestOutput()
        {
            //OutputPath = PluginSettings.GetRootedPath(Server.Settings.ServerManagerRoot, "OutputPath", @".\Maps\" + this.Tag);
            //CachePath = PluginSettings.GetRootedPath(Server.Settings.ServerManagerRoot, "CachePath", @".\Cache\" + this.Tag);

            AlphaVespucci av = new AlphaVespucci();
            av.Initialise(mockServer.Object);
        }
    }
}

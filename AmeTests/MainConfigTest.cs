using AmeWindowsService.Config;
using NUnit.Framework;

namespace AmeTests
{
    [TestFixture]
    public class MainConfigTest
    {
        [Test]
        public void LoadConfig()
        {
            SettingsProvider config = new SettingsProvider();
            Assert.IsNotNull(config.Root);
        }
    }
}

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
            MainConfig config = new MainConfig();
            Assert.IsNotNull(config.Root);
        }
    }
}

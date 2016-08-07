using VIC.ObjectConfig;
using Xunit;

namespace UT.VIC.ObjectConfig
{
    public class ConfigSourceTest
    {
        [Fact]
        public void TestConfigSource()
        {
            var c = new ConfigSource("test", () => "test2");
            Assert.Equal("test", c.Key);
            Assert.Equal("test2", c.GetValue());
        }
    }
}
using Microsoft.Extensions.FileProviders;
using System.IO;
using VIC.ObjectConfig;
using Xunit;

namespace UT.VIC.ObjectConfig
{
    public class PhysicalFileConfigStoreTest
    {
        [Fact]
        public void TestPhysicalFileConfigStore()
        {
            var store = new PhysicalFileConfigStore(Directory.GetCurrentDirectory());
            Assert.NotNull(store.FileProvider);
            Assert.IsType<PhysicalFileProvider>(store.FileProvider);
            Assert.Null(store.GetConfigSource("test"));
            Assert.True(store.Update(new ConfigSource("test", () => "test1")));
            var c = store.GetConfigSource("test");
            Assert.NotNull(c);
            Assert.Equal("test1", c.GetValue());
            Assert.True(store.Update(new ConfigSource("test", () => c)));
            var c2 = store.GetConfigSource("test");
            Assert.NotNull(c2);
            Assert.Same(c, c2.GetValue());
            var c3 = store.Get<ConfigSource>("test");
            Assert.NotNull(c3);
            Assert.Same(c, c3);
        }
    }
}
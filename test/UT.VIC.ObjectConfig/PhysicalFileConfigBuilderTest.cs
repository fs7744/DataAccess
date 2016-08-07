using Microsoft.Extensions.FileProviders;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VIC.ObjectConfig;
using VIC.ObjectConfig.Abstraction;
using VIC.ObjectConfig.Json;
using VIC.ObjectConfig.Xml;
using Xunit;

namespace UT.VIC.ObjectConfig
{
    public class TestPhysicalFileConfigBuilder : PhysicalFileConfigBuilder
    {
        public IConfigBuilder ChangeStore(IConfigStore store)
        {
            _Store = store;
            return this;
        }
    }

    public class PhysicalFileConfigBuilderTest
    {
        [Fact]
        public void TestPhysicalFileConfigBuilder()
        {
            var builder = new TestPhysicalFileConfigBuilder();
            builder.SetBasePath(Directory.GetCurrentDirectory());
            var store = new TestPhysicalFileConfigStore(Directory.GetCurrentDirectory());
            store.Data = new Dictionary<string, List<IFileInfo>>()
            {
                { "s1", new List<IFileInfo>() { new TestXmlFile<Student>(new Student() { Age = 1, Name = "1" }), new TestXmlFile<Student>(new Student() { Age = 4, Name = "4" }) } },
                { "s2", new List<IFileInfo>() { new TestXmlFile<Student>(new Student() { Age = 2, Name = "2" }), new TestXmlFile<Student>(new Student() { Age = 2, Name = "2" }) } },
                { "s3", new List<IFileInfo>() { new TestXmlFile<Student>(new Student() { Age = 3, Name = "3" }), new TestXmlFile<Student>(new Student() { Age = 3, Name = "3" }) } },
                { "js1", new List<IFileInfo>() { new TestJsonFile<Student>(new Student() { Age = 11, Name = "1" }), new TestJsonFile<Student>(new Student() { Age = 14, Name = "4" }) } },
                { "js2", new List<IFileInfo>() { new TestJsonFile<Student>(new Student() { Age = 12, Name = "2" }), new TestJsonFile<Student>(new Student() { Age = 12, Name = "2" }) } },
                { "js3", new List<IFileInfo>() { new TestJsonFile<Student>(new Student() { Age = 13, Name = "3" }), new TestJsonFile<Student>(new Student() { Age = 13, Name = "3" }) } }
            };
            var c = builder.ChangeStore(store)
                .Add(new JsonConfigFileProvider<Student>("js", true, ss =>
                {
                    return ss.Aggregate(async (l, r) =>
                    {
                        var x = await l;
                        var y = await r;
                        x.Age += y.Age;
                        x.Name += y.Name;
                        return x;
                    });
                }, "js1", "js2", "js3"))
                .Add(new XmlConfigFileProvider<Student>("k", true, ss =>
                {
                    return ss.Aggregate(async (l, r) =>
                    {
                        var x = await l;
                        var y = await r;
                        x.Age += y.Age;
                        x.Name += y.Name;
                        return x;
                    });
                }, "s1", "s2", "s3"))
                .Build();

            var s = c.Get<Student>("k");
            Assert.NotNull(s);
            Assert.Equal(6, s.Age);
            Assert.Equal("123", s.Name);
            var js = store.Get<Student>("js");
            Assert.NotNull(s);
            Assert.Equal(36, js.Age);
            Assert.Equal("123", js.Name);
            store.DoChange();
            s = c.Get<Student>("k");
            Assert.NotNull(s);
            Assert.Equal(9, s.Age);
            Assert.Equal("423", s.Name);
            js = c.Get<Student>("js");
            Assert.NotNull(s);
            Assert.Equal(39, js.Age);
            Assert.Equal("423", js.Name);
        }
    }
}
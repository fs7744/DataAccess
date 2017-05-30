using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.FileProviders;
using VIC.ObjectConfig.Json;
using VIC.ObjectConfig.Xml;
using VIC.ObjectConfig.Merge;
using Xunit;
using System.Linq;

namespace UT.VIC.ObjectConfig
{

    public class MergeConfigBuilderTest
    {
        [Fact]
        public void TestMergeConfigBuilderBuilder()
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
            var a = new MergeConfigBuilder(rs => rs.Select(i => i as Student).ToList())
                .Add(c)
                .Add(c)
                .Build();

            var dd = a.GetConfigSource("k").GetValue();
            Assert.IsType(typeof(List<Student>), dd);
            var s = a.GetConfigSource("k").GetValue() as List<Student>;
            Assert.NotNull(s);
            var d = s.ToArray();
            Assert.Equal(2, d.Length);
            Assert.Equal(6, d[0].Age);
            Assert.Equal("123", d[0].Name);
            Assert.Equal(6, d[1].Age);
            Assert.Equal("123", d[1].Name);
            d = null;
            var js = a.GetConfigSource("js").GetValue() as List<Student>;
            Assert.NotNull(js);
            d = js.ToArray();
            Assert.Equal(2, d.Length);
            Assert.Equal(36, d[0].Age);
            Assert.Equal("123", d[0].Name);
            Assert.Equal(36, d[1].Age);
            Assert.Equal("123", d[1].Name);
        }
    }
}
using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VIC.ObjectConfig;
using VIC.ObjectConfig.Json;
using Xunit;

namespace UT.VIC.ObjectConfig
{
    public class TestJsonFile<T> : IFileInfo
    {
        private T _Data;

        public TestJsonFile(T data)
        {
            _Data = data;
        }

        public bool Exists { get { return true; } }

        public bool IsDirectory { get { return false; } }

        public DateTimeOffset LastModified { get { return DateTimeOffset.Now; } }

        public long Length
        {
            get
            {
                return 1L;
            }
        }

        public string Name { get; private set; }

        public string PhysicalPath
        {
            get
            {
                return Name;
            }
        }

        public Stream CreateReadStream()
        {
            var stream = new MemoryStream();
            TextWriter sw = new StreamWriter(stream);
            sw.Write(JsonConvert.SerializeObject(_Data));
            sw.Flush();
            stream.Position = 0L;
            return stream;
        }
    }

    public class JsonConfigFileProviderTest
    {
        [Fact]
        public void TestXmlConfigFileProvider()
        {
            var xml = new JsonConfigFileProvider<Student>("k", true, ss =>
            {
                return ss.Aggregate(async (l, r) =>
                {
                    var x = await l;
                    var y = await r;
                    x.Age += y.Age;
                    x.Name += y.Name;
                    return x;
                });
            }, "s1", "s2", "s3");
            var store = new TestPhysicalFileConfigStore(Directory.GetCurrentDirectory());
            store.Data = new Dictionary<string, List<IFileInfo>>()
            {
                { "s1", new List<IFileInfo>() { new TestJsonFile<Student>(new Student() { Age = 11, Name = "1" }), new TestJsonFile<Student>(new Student() { Age = 14, Name = "4" }) } },
                { "s2", new List<IFileInfo>() { new TestJsonFile<Student>(new Student() { Age = 12, Name = "2" }), new TestJsonFile<Student>(new Student() { Age = 12, Name = "2" }) } },
                { "s3", new List<IFileInfo>() { new TestJsonFile<Student>(new Student() { Age = 13, Name = "3" }), new TestJsonFile<Student>(new Student() { Age = 13, Name = "3" }) } }
            };
            xml.SetConfig(store);
            var s = store.Get<Student>("k");
            Assert.NotNull(s);
            Assert.Equal(36, s.Age);
            Assert.Equal("123", s.Name);
            store.DoChange();
            s = store.Get<Student>("k");
            Assert.NotNull(s);
            Assert.Equal(39, s.Age);
            Assert.Equal("423", s.Name);
        }
    }
}
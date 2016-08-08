using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using VIC.ObjectConfig;
using VIC.ObjectConfig.Xml;
using Xunit;

namespace UT.VIC.ObjectConfig
{
    public class TestXmlFile<T> : IFileInfo
    {
        private T _Data;

        public TestXmlFile(T data)
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
            var serializer = new XmlSerializer(typeof(T));
            serializer.Serialize(stream, _Data);
            stream.Position = 0L;
            return stream;
        }
    }

    public class TestPhysicalFileConfigStore : PhysicalFileConfigStore
    {
        private Dictionary<string, List<IFileInfo>> _Data;

        public Dictionary<string, List<IFileInfo>> Data
        {
            get { return _Data; }
            set
            {
                _Data = value;
                (FileProvider as TestPhysicalFileProvider)._Data = value;
            }
        }

        public class TestChangeToken : IChangeToken
        {
            public List<Action<object>> Callback { get; private set; } = new List<Action<object>>();

            public bool ActiveChangeCallbacks { get; private set; }

            public bool HasChanged { get; private set; }

            public IDisposable RegisterChangeCallback(Action<object> callback, object state)
            {
                Callback.Add(callback);
                return null;
            }
        }

        public class TestPhysicalFileProvider : IFileProvider
        {
            public Dictionary<string, List<IFileInfo>> _Data;

            private int _Index = 0;

            public List<TestChangeToken> Tokens { get; private set; } = new List<TestChangeToken>();

            public IDirectoryContents GetDirectoryContents(string subpath)
            {
                throw new NotImplementedException();
            }

            public IFileInfo GetFileInfo(string subpath)
            {
                return _Data[subpath][_Index];
            }

            public IChangeToken Watch(string filter)
            {
                var result = new TestChangeToken();
                Tokens.Add(result);
                return result;
            }

            public void DoChange()
            {
                _Index++;
                Tokens.SelectMany(i => i.Callback).ToList().ForEach(i => i(null));
            }
        }

        public TestPhysicalFileConfigStore(string basePath) : base(basePath)
        {
            FileProvider = new TestPhysicalFileProvider();
        }

        public void DoChange()
        {
            (FileProvider as TestPhysicalFileProvider).DoChange();
        }
    }

    public class Student
    {
        public int Age { get; set; }

        public string Name { get; set; }
    }

    public class XmlConfigFileProviderTest
    {
        [Fact]
        public void TestXmlConfigFileProvider()
        {
            var xml = new XmlConfigFileProvider<Student>("k", true, ss =>
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
                { "s1", new List<IFileInfo>() { new TestXmlFile<Student>(new Student() { Age = 1, Name = "1" }), new TestXmlFile<Student>(new Student() { Age = 4, Name = "4" }) } },
                { "s2", new List<IFileInfo>() { new TestXmlFile<Student>(new Student() { Age = 2, Name = "2" }), new TestXmlFile<Student>(new Student() { Age = 2, Name = "2" }) } },
                { "s3", new List<IFileInfo>() { new TestXmlFile<Student>(new Student() { Age = 3, Name = "3" }), new TestXmlFile<Student>(new Student() { Age = 3, Name = "3" }) } }
            };
            xml.SetConfig(store);
            var s = store.Get<Student>("k");
            Assert.NotNull(s);
            Assert.Equal(6, s.Age);
            Assert.Equal("123", s.Name);
            store.DoChange();
            s = store.Get<Student>("k");
            Assert.NotNull(s);
            Assert.Equal(9, s.Age);
            Assert.Equal("423", s.Name);
        }
    }
}
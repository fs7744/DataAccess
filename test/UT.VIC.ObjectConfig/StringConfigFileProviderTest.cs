using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VIC.ObjectConfig;
using Xunit;

namespace UT.VIC.ObjectConfig
{

    public class StringConfigFileProviderTest
    {
        [Fact]
        public void TestStringConfigFileProvider()
        {
            var xml = new StringConfigFileProvider("k", true, ss =>
            {
                return ss.Aggregate(async (l, r) =>
                {
                    var x = await l;
                    var y = await r;
                    return x + y;
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
            var s = store.Get<string>("k");
            Assert.NotNull(s);
            Assert.Equal("{\"Age\":11,\"Name\":\"1\"}{\"Age\":12,\"Name\":\"2\"}{\"Age\":13,\"Name\":\"3\"}", s);
            store.DoChange();
            s = store.Get<string>("k");
            Assert.NotNull(s);
            Assert.Equal("{\"Age\":14,\"Name\":\"4\"}{\"Age\":12,\"Name\":\"2\"}{\"Age\":13,\"Name\":\"3\"}", s);
        }
    }
}
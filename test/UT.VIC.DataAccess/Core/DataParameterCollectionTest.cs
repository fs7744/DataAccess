using System;
using System.Linq;
using VIC.DataAccess;
using VIC.DataAccess.Core;
using Xunit;

namespace UT.VIC.DataAccess.Core
{
    public class DataParameterCollectionTest
    {
        [Fact]
        public void TestDataParameterCollectionTest()
        {
            var ds = new DataParameterCollection();
            var dp = new DataParameter() { ParameterName = "s" };
            Assert.Equal(0, ds.Count);
            ds.Add(dp);
            Assert.Equal(1, ds.Count);
            Assert.Same(dp, ds["@s"]);
            dp = new DataParameter() { ParameterName = "fs" };
            ds.Add(dp);
            Assert.Equal(2, ds.Count);
            Assert.Throws<ArgumentException>(() => ds.Add(dp));
            Assert.Same(dp, ds["@fs"]);
            Assert.NotSame(dp, ds["@s"]);
            Assert.True(ds.Contains("@s"));
            Assert.True(ds.Contains("@fs"));
            Assert.False(ds.Contains("@fsd"));
            Assert.True(ds.Contains(dp));
            var list = ds.GetParams().ToList();
            Assert.Equal(2, list.Count);
            Assert.True(list.Contains(dp));
            Assert.True(ds.Remove("@fs"));
            Assert.Equal(1, ds.Count);
            Assert.False(ds.Contains(dp));
            Assert.False(ds.Remove("@fs"));
            dp = ds["@s"];
            Assert.True(ds.Contains(dp));
            ds.Clear();
            Assert.Equal(0, ds.Count);
            Assert.False(ds.Contains("@fs"));
            Assert.False(ds.Contains("@s"));
            Assert.False(ds.Contains(dp));
        }
    }
}
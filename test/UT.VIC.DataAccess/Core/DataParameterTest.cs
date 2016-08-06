using System.Data;
using VIC.DataAccess;
using Xunit;

namespace UT.VIC.DataAccess.Core
{
    public class DataParameterTest
    {
        [Fact]
        public void TestDataParameter()
        {
            var dp = new DataParameter();
            Assert.Equal(ParameterDirection.Input, dp.Direction);
            Assert.True(dp.IsNullable);
            Assert.Null(dp.ParameterName);
            dp.ParameterName = "dd";
            Assert.Equal("@dd", dp.ParameterName);
            dp.ParameterName = "@fgd";
            Assert.Equal("@fgd", dp.ParameterName);
            dp.Precision = 3;
            Assert.Equal(3, dp.Precision);
            dp.Scale = 4;
            Assert.Equal(4, dp.Scale);
            dp.Size = 6;
            Assert.Equal(6, dp.Size);
        }
    }
}
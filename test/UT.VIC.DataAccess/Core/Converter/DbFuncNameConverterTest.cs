using System;
using System.Collections.Generic;
using VIC.DataAccess.Abstraction.Converter;
using VIC.DataAccess.Core.Converter;
using Xunit;

namespace UT.VIC.DataAccess.Core.Converter
{
    public class DbFuncNameConverterTest
    {
        private IDbFuncNameConverter _Converter = new DbFuncNameConverter();

        [Theory]
        [InlineData(typeof(long), "GetInt64")]
        [InlineData(typeof(bool), "GetBoolean")]
        [InlineData(typeof(string), "GetString")]
        [InlineData(typeof(DateTime), "GetDateTime")]
        [InlineData(typeof(decimal), "GetDecimal")]
        [InlineData(typeof(double), "GetDouble")]
        [InlineData(typeof(int), "GetInt32")]
        [InlineData(typeof(float), "GetFloat")]
        [InlineData(typeof(short), "GetInt16")]
        [InlineData(typeof(byte), "GetByte")]
        [InlineData(typeof(Guid), "GetGuid")]
        [InlineData(typeof(DbFuncNameConverter), "GetValue")]
        [InlineData(typeof(long?), "GetInt64")]
        [InlineData(typeof(bool?), "GetBoolean")]
        [InlineData(typeof(DateTime?), "GetDateTime")]
        [InlineData(typeof(decimal?), "GetDecimal")]
        [InlineData(typeof(double?), "GetDouble")]
        [InlineData(typeof(int?), "GetInt32")]
        [InlineData(typeof(float?), "GetFloat")]
        [InlineData(typeof(short?), "GetInt16")]
        [InlineData(typeof(byte?), "GetByte")]
        [InlineData(typeof(Guid?), "GetGuid")]
        [InlineData(typeof(List<DbFuncNameConverter>), "GetValue")]
        public void TestDbFuncNameConverter(Type type, string funcName)
        {
            Assert.Equal(funcName, _Converter.Convert(type));
        }
    }
}
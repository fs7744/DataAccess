using System;
using System.Collections.Generic;
using System.Data;
using VIC.DataAccess.Abstraction.Converter;
using VIC.DataAccess.Core.Converter;
using Xunit;

namespace UT.VIC.DataAccess.Core.Converter
{
    public class DbTypeConverterTest
    {
        private IDbTypeConverter _Converter = new DbTypeConverter();

        [Theory]
        [InlineData(typeof(long), DbType.Int64)]
        [InlineData(typeof(bool), DbType.Boolean)]
        [InlineData(typeof(string), DbType.String)]
        [InlineData(typeof(DateTime), DbType.DateTime2)]
        [InlineData(typeof(decimal), DbType.Decimal)]
        [InlineData(typeof(double), DbType.Double)]
        [InlineData(typeof(int), DbType.Int32)]
        [InlineData(typeof(float), DbType.Single)]
        [InlineData(typeof(short), DbType.Int16)]
        [InlineData(typeof(byte), DbType.Byte)]
        [InlineData(typeof(Guid), DbType.Guid)]
        [InlineData(typeof(long?), DbType.Int64)]
        [InlineData(typeof(bool?), DbType.Boolean)]
        [InlineData(typeof(DateTime?), DbType.DateTime2)]
        [InlineData(typeof(decimal?), DbType.Decimal)]
        [InlineData(typeof(double?), DbType.Double)]
        [InlineData(typeof(int?), DbType.Int32)]
        [InlineData(typeof(float?), DbType.Single)]
        [InlineData(typeof(short?), DbType.Int16)]
        [InlineData(typeof(byte?), DbType.Byte)]
        [InlineData(typeof(Guid?), DbType.Guid)]
        [InlineData(typeof(DbTypeConverter), DbType.String)]
        [InlineData(typeof(List<DbTypeConverter>), DbType.String)]
        public void TestDbFuncNameConverter(Type type, DbType dbType)
        {
            Assert.Equal(dbType, _Converter.Convert(type));
        }
    }
}
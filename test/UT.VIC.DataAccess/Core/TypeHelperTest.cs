using System;
using System.Collections.Generic;
using VIC.DataAccess.Core;
using Xunit;

namespace UT.VIC.DataAccess.Core
{
    public class TypeHelperTest
    {
        [Theory]
        [InlineData(typeof(long), typeof(long))]
        [InlineData(typeof(bool), typeof(bool))]
        [InlineData(typeof(string), typeof(string))]
        [InlineData(typeof(DateTime), typeof(DateTime))]
        [InlineData(typeof(decimal), typeof(decimal))]
        [InlineData(typeof(double), typeof(double))]
        [InlineData(typeof(int), typeof(int))]
        [InlineData(typeof(float), typeof(float))]
        [InlineData(typeof(short), typeof(short))]
        [InlineData(typeof(byte), typeof(byte))]
        [InlineData(typeof(Guid), typeof(Guid))]
        [InlineData(typeof(long?), typeof(long))]
        [InlineData(typeof(bool?), typeof(bool))]
        [InlineData(typeof(DateTime?), typeof(DateTime))]
        [InlineData(typeof(decimal?), typeof(decimal))]
        [InlineData(typeof(double?), typeof(double))]
        [InlineData(typeof(int?), typeof(int))]
        [InlineData(typeof(float?), typeof(float))]
        [InlineData(typeof(short?), typeof(short))]
        [InlineData(typeof(byte?), typeof(byte))]
        [InlineData(typeof(Guid?), typeof(Guid))]
        [InlineData(typeof(List<int>), typeof(List<int>))]
        public void TestGetRealType(Type type, Type realType)
        {
            Assert.Equal(realType, TypeHelper.GetRealType(type));
        }
    }
}
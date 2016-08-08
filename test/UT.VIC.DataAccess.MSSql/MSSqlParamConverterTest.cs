using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using UT.VIC.DataAccess.MSSql;
using VIC.DataAccess.Abstraction.Converter;
using VIC.DataAccess.Core.Converter;
using VIC.DataAccess.MSSql.Core.Converter;
using Xunit;

namespace UT.VIC.DataAccess.Core.Converter
{
    public class MSSqlParamConverterTest
    {
        private IParamConverter _Converter = new MSSqlParamConverter(new DbTypeConverter());

        private List<Student> _Students = new List<Student>()
        {
            new Student()
            {
                Age = 1,
                Name = "Victor1"
            },
            new Student() { Age = 2, Name = "Victor2" },
            new Student()
            {
                Age = 3,
                Name = "Victor3"
            },
        };

        [Fact]
        public void TestMSSqlParamConvert()
        {
            var dc = new DbTypeConverter();
            var type = typeof(Student);
            var ps = TypeExtensions.GetProperties(type, BindingFlags.Instance | BindingFlags.Public)
                .Where(i => i.CanRead).ToList();
            foreach (var item in _Students)
            {
                var ds = _Converter.Convert(type, item);
                Assert.Equal(ps.Count, ds.Count);
                for (int i = 0; i < ps.Count; i++)
                {
                    Assert.IsType<SqlParameter>(ds[i]);
                    Assert.Equal(dc.Convert(ps[i].PropertyType), ds[i].DbType);
                    Assert.Equal(ParameterDirection.Input, ds[i].Direction);
                    Assert.True(ds[i].IsNullable);
                    Assert.Equal("@" + ps[i].Name, ds[i].ParameterName);
                    Assert.Equal(ps[i].GetMethod.Invoke(item, new object[0]), ds[i].Value);
                }
            }
        }
    }
}
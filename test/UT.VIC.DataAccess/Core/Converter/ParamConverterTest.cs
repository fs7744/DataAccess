using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using VIC.DataAccess.Abstraction.Converter;
using VIC.DataAccess.Core.Converter;
using Xunit;

namespace UT.VIC.DataAccess.Core.Converter
{
    public class ParamConverterTest
    {
        public class TestParam : DbParameter
        {
            public override DbType DbType { get; set; }

            public override ParameterDirection Direction { get; set; }

            public override bool IsNullable { get; set; }

            public override string ParameterName { get; set; }

            public override int Size { get; set; }

            public override string SourceColumn { get; set; }

            public override bool SourceColumnNullMapping { get; set; }

            public override object Value { get; set; }

            public override void ResetDbType()
            {
                throw new NotImplementedException();
            }
        }

        public class TestParamConverter : ParamConverter
        {
            public TestParamConverter(IDbTypeConverter dc) : base(dc)
            {
            }

            protected override Type GetParameterType()
            {
                return typeof(TestParam);
            }
        }

        private IParamConverter _Converter = new TestParamConverter(new DbTypeConverter());

        private List<Student> _Students = new List<Student>()
        {
            new Student()
            {
                Age = 1,
                Name = "Victor1",
                ClassNumber = 2,
                Long = 3L,
                Decimal = 4M,
                Byte = 2,
                DateTime = new DateTime(1990,2,3),
                Double = 4.4D,
                Float = 33.3F,
                Short = 77,
                Guid = Guid.Parse("E82E26DA-AED1-4FD3-BFF9-73BE66F28EED"),
                Bool = true
            },
            new Student() { Age = 2, Name = "Victor2" },
            new Student()
            {
                Age = 3,
                Name = "Victor3",
                ClassNumber = 2,
                Long2 = 3L,
                Decimal2 = 4M,
                Byte2 = 2,
                DateTime2 = new DateTime(1990,2,3),
                Double2 = 4.4D,
                Float2 = 33.3F,
                Short2 = 77,
                Guid2 = Guid.Parse("E82E26DA-AED1-4FD3-BFF9-73BE66F28EED"),
                Bool2 = false
            },
        };

        [Fact]
        public void TestParamConvert()
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
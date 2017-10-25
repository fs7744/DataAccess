using System;
using System.Collections.Generic;
using VIC.DataAccess.Abstraction.Converter;
using VIC.DataAccess.Core;
using VIC.DataAccess.Core.Converter;
using Xunit;

namespace UT.VIC.DataAccess.Core.Converter
{
    public class EntityConverterTest
    {
        private IEntityConverter _Converter = new EntityConverter();

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
        public void TestEntityConvert()
        {
            var type = typeof(Student);
            var reader = new ListDataReader<Student>(_Students);
            foreach (var item in _Students)
            {
                reader.Read();
                Student s = _Converter.GetConverter<Student>(reader)(reader);
                Assert.Equal(item.ClassNumber, s.ClassNumber);
                Assert.Equal(item.Age, s.Age);
                Assert.Equal(item.Bool, s.Bool);
                Assert.Equal(item.Byte, s.Byte);
                Assert.Equal(item.Name, s.Name);
                Assert.Equal(item.DateTime, s.DateTime);
                Assert.Equal(item.Decimal, s.Decimal);
                Assert.Equal(item.Double, s.Double);
                Assert.Equal(item.Float, s.Float);
                Assert.Equal(item.Guid, s.Guid);
                Assert.Equal(item.Long, s.Long);
                Assert.Equal(item.Short, s.Short);
                Assert.Equal(item.Bool2, s.Bool2);
                Assert.Equal(item.Byte2, s.Byte2);
                Assert.Equal(item.DateTime2, s.DateTime2);
                Assert.Equal(item.Decimal2, s.Decimal2);
                Assert.Equal(item.Double2, s.Double2);
                Assert.Equal(item.Float2, s.Float2);
                Assert.Equal(item.Guid2, s.Guid2);
                Assert.Equal(item.Long2, s.Long2);
                Assert.Equal(item.Short2, s.Short2);
                Assert.NotSame(item, s);
            }
        }
    }
}
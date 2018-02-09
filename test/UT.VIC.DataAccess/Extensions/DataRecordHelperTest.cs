using System;
using System.Collections.Generic;
using System.Text;
using UT.VIC.DataAccess.Core;
using VIC.DataAccess.Core;
using Xunit;
using VIC.DataAccess.Extensions;

namespace UT.VIC.DataAccess.Extensions
{
    public class DataRecordHelperTest
    {
        private List<Student> _Students = new List<Student>()
        {
            new Student()
            {
                Age = 1,
                Name = null,
                ClassNumber = 2,
                Long2 = 2L,
                Decimal2 = 4M,
                Byte2 = 2,
                DateTime2 = new DateTime(1990,2,3),
                Double2 = 4.4D,
                Float2 = 33.3F,
                Short2 = 77,
                Guid2 = Guid.Parse("E82E26DA-AED1-4FD3-BFF9-73BE66F28EED"),
                Bool2 = true,
                Char = '2'
            },
            new Student() { Age = 2, Name = "Victor2" },
            new Student()
            {
                Age = 3,
                Name = "Victor3",
                ClassNumber = 3,
                Long2 = 3L,
                Decimal2 = 5M,
                Byte2 = 3,
                DateTime2 = new DateTime(1991,2,3),
                Double2 = 5.5D,
                Float2 = 33.43F,
                Short2 = 76,
                Guid2 = Guid.Parse("A82E26DA-AED1-4FD3-BFF9-73BE66F28EED"),
                Bool2 = false,
                Char = '3'
            },
        };

        [Fact]
        public void TestGetStringWithIsDBNullCheck()
        {
            var reader = new ListDataReader<Student>(_Students);
            reader.Read();
            Assert.Null(reader.GetStringWithNull("Name"));
            reader.Read();
            Assert.NotNull(reader.GetStringWithNull("Name"));
            reader.Read();
            Assert.NotNull(reader.GetStringWithNull("Name"));
        }

        [Fact]
        public void TestGetBoolean()
        {
            var reader = new ListDataReader<Student>(_Students);
            reader.Read();
            var col = "Bool2";
            Assert.True(reader.GetBoolean(col));
            reader.Read();
            reader.Read();
            Assert.False(reader.GetBoolean(col));
        }

        [Fact]
        public void TestGetBooleanWithIsDBNullCheck()
        {
            var reader = new ListDataReader<Student>(_Students);
            reader.Read();
            var col = "Bool2";
            Assert.True(reader.GetBooleanWithNull(col).Value);
            reader.Read();
            Assert.False(reader.GetBooleanWithNull(col).HasValue);
            reader.Read();
            Assert.False(reader.GetBooleanWithNull(col).Value);
        }

        [Fact]
        public void TestGetGetByte()
        {
            var reader = new ListDataReader<Student>(_Students);
            reader.Read();
            var col = "Byte2";
            Assert.Equal(2, reader.GetByte(col));
            reader.Read();
            reader.Read();
            Assert.Equal(3, reader.GetByte(col));
        }

        [Fact]
        public void TestGetByteWithIsDBNullCheck()
        {
            var reader = new ListDataReader<Student>(_Students);
            reader.Read();
            var col = "Byte2";
            Assert.Equal(2, reader.GetByteWithNull(col).Value);
            reader.Read();
            Assert.False(reader.GetByteWithNull(col).HasValue);
            reader.Read();
            Assert.Equal(3, reader.GetByteWithNull(col).Value);
        }

        [Fact]
        public void TestGetChar()
        {
            var reader = new ListDataReader<Student>(_Students);
            reader.Read();
            var col = "Char";
            Assert.Equal('2', reader.GetChar(col));
            reader.Read();
            reader.Read();
            Assert.Equal('3', reader.GetChar(col));
        }

        [Fact]
        public void TestGetCharWithIsDBNullCheck()
        {
            var reader = new ListDataReader<Student>(_Students);
            reader.Read();
            var col = "Char";
            Assert.Equal('2', reader.GetCharWithNull(col).Value);
            reader.Read();
            Assert.False(reader.GetCharWithNull(col).HasValue);
            reader.Read();
            Assert.Equal('3', reader.GetCharWithNull(col).Value);
        }

        [Fact]
        public void TestGetDateTime()
        {
            var reader = new ListDataReader<Student>(_Students);
            reader.Read();
            var col = "DateTime2";
            Assert.Equal(new DateTime(1990, 2, 3), reader.GetDateTime(col));
            reader.Read();
            reader.Read();
            Assert.Equal(new DateTime(1991, 2, 3), reader.GetDateTime(col));
        }

        [Fact]
        public void TestGetDateTimeWithIsDBNullCheck()
        {
            var reader = new ListDataReader<Student>(_Students);
            reader.Read();
            var col = "DateTime2";
            Assert.Equal(new DateTime(1990, 2, 3), reader.GetDateTimeWithNull(col).Value);
            reader.Read();
            Assert.False(reader.GetDateTimeWithNull(col).HasValue);
            reader.Read();
            Assert.Equal(new DateTime(1991, 2, 3), reader.GetDateTimeWithNull(col).Value);
        }

        [Fact]
        public void TestGetDecimal()
        {
            var reader = new ListDataReader<Student>(_Students);
            reader.Read();
            var col = "Decimal2";
            Assert.Equal(4M, reader.GetDecimal(col));
            reader.Read();
            reader.Read();
            Assert.Equal(5M, reader.GetDecimal(col));
        }

        [Fact]
        public void TestGetDecimalWithIsDBNullCheck()
        {
            var reader = new ListDataReader<Student>(_Students);
            reader.Read();
            var col = "Decimal2";
            Assert.Equal(4M, reader.GetDecimalWithNull(col).Value);
            reader.Read();
            Assert.False(reader.GetDecimalWithNull(col).HasValue);
            reader.Read();
            Assert.Equal(5M, reader.GetDecimalWithNull(col).Value);
        }

        [Fact]
        public void TestGetDouble()
        {
            var reader = new ListDataReader<Student>(_Students);
            reader.Read();
            var col = "Double2";
            Assert.Equal(4.4, reader.GetDouble(col));
            reader.Read();
            reader.Read();
            Assert.Equal(5.5, reader.GetDouble(col));
        }

        [Fact]
        public void TestGetDoubleWithIsDBNullCheck()
        {
            var reader = new ListDataReader<Student>(_Students);
            reader.Read();
            var col = "Double2";
            Assert.Equal(4.4, reader.GetDoubleWithNull(col).Value);
            reader.Read();
            Assert.False(reader.GetDoubleWithNull(col).HasValue);
            reader.Read();
            Assert.Equal(5.5, reader.GetDoubleWithNull(col).Value);
        }

        [Fact]
        public void TestGetFloat()
        {
            var reader = new ListDataReader<Student>(_Students);
            reader.Read();
            var col = "Float2";
            Assert.Equal(33.3F, reader.GetFloat(col));
            reader.Read();
            reader.Read();
            Assert.Equal(33.43F, reader.GetFloat(col));
        }

        [Fact]
        public void TestGetFloatWithIsDBNullCheck()
        {
            var reader = new ListDataReader<Student>(_Students);
            reader.Read();
            var col = "Float2";
            Assert.Equal(33.3F, reader.GetFloatWithNull(col).Value);
            reader.Read();
            Assert.False(reader.GetFloatWithNull(col).HasValue);
            reader.Read();
            Assert.Equal(33.43F, reader.GetFloatWithNull(col).Value);
        }

        [Fact]
        public void TestGetGuid()
        {
            var reader = new ListDataReader<Student>(_Students);
            reader.Read();
            var col = "Guid2";
            Assert.Equal(Guid.Parse("E82E26DA-AED1-4FD3-BFF9-73BE66F28EED"), reader.GetGuid(col));
            reader.Read();
            reader.Read();
            Assert.Equal(Guid.Parse("A82E26DA-AED1-4FD3-BFF9-73BE66F28EED"), reader.GetGuid(col));
        }

        [Fact]
        public void TesGetGuidWithIsDBNullCheck()
        {
            var reader = new ListDataReader<Student>(_Students);
            reader.Read();
            var col = "Guid2";
            Assert.Equal(Guid.Parse("E82E26DA-AED1-4FD3-BFF9-73BE66F28EED"), reader.GetGuidWithNull(col).Value);
            reader.Read();
            Assert.False(reader.GetGuidWithNull(col).HasValue);
            reader.Read();
            Assert.Equal(Guid.Parse("A82E26DA-AED1-4FD3-BFF9-73BE66F28EED"), reader.GetGuidWithNull(col).Value);
        }

        [Fact]
        public void TestGetInt16()
        {
            var reader = new ListDataReader<Student>(_Students);
            reader.Read();
            var col = "Short2";
            Assert.Equal(77, reader.GetInt16(col));
            reader.Read();
            reader.Read();
            Assert.Equal(76, reader.GetInt16(col));
        }

        [Fact]
        public void TesGetInt16WithIsDBNullCheck()
        {
            var reader = new ListDataReader<Student>(_Students);
            reader.Read();
            var col = "Short2";
            Assert.Equal(77, reader.GetInt16WithNull(col).Value);
            reader.Read();
            Assert.False(reader.GetInt16WithNull(col).HasValue);
            reader.Read();
            Assert.Equal(76, reader.GetInt16WithNull(col).Value);
        }

        [Fact]
        public void TestGetInt32()
        {
            var reader = new ListDataReader<Student>(_Students);
            reader.Read();
            var col = "ClassNumber";
            Assert.Equal(2, reader.GetInt32(col));
            reader.Read();
            reader.Read();
            Assert.Equal(3, reader.GetInt32(col));
        }

        [Fact]
        public void TesGetInt32WithIsDBNullCheck()
        {
            var reader = new ListDataReader<Student>(_Students);
            reader.Read();
            var col = "ClassNumber";
            Assert.Equal(2, reader.GetInt32WithNull(col).Value);
            reader.Read();
            Assert.False(reader.GetInt32WithNull(col).HasValue);
            reader.Read();
            Assert.Equal(3, reader.GetInt32WithNull(col).Value);
        }

        [Fact]
        public void TestGetInt64()
        {
            var reader = new ListDataReader<Student>(_Students);
            reader.Read();
            var col = "Long2";
            Assert.Equal(2, reader.GetInt64(col));
            reader.Read();
            reader.Read();
            Assert.Equal(3, reader.GetInt64(col));
        }

        [Fact]
        public void TesGetInt64WithIsDBNullCheck()
        {
            var reader = new ListDataReader<Student>(_Students);
            reader.Read();
            var col = "Long2";
            Assert.Equal(2, reader.GetInt64WithNull(col).Value);
            reader.Read();
            Assert.False(reader.GetInt64WithNull(col).HasValue);
            reader.Read();
            Assert.Equal(3, reader.GetInt64WithNull(col).Value);
        }
    }
}

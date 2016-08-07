using Moq;
using System;
using System.Data.Common;
using VIC.DataAccess.Abstraction.Converter;
using VIC.DataAccess.Core.Converter;
using Xunit;

namespace UT.VIC.DataAccess.Core.Converter
{
    public class ScalarConverterTest
    {
        private IScalarConverter _Converter = new ScalarConverter();

        [Fact]
        public void TestScalarConvertInt64()
        {
            var readerMock = new Mock<DbDataReader>();
            readerMock.SetupSequence(i => i.GetInt64(0))
                .Returns(3L)
                .Returns(5L);
            readerMock.SetupSequence(i => i.IsDBNull(0))
                .Returns(false)
                .Returns(true);
            Assert.Equal(3L, _Converter.Convert<long>(readerMock.Object));
            Assert.Equal(5L, _Converter.Convert<long?>(readerMock.Object));
            Assert.Null(_Converter.Convert<long?>(readerMock.Object));
        }

        [Fact]
        public void TestScalarConvertBool()
        {
            var readerMock = new Mock<DbDataReader>();
            readerMock.SetupSequence(i => i.GetBoolean(0))
                .Returns(true)
                .Returns(false);
            readerMock.SetupSequence(i => i.IsDBNull(0))
                .Returns(false)
                .Returns(true);
            Assert.True(_Converter.Convert<bool>(readerMock.Object));
            Assert.False(_Converter.Convert<bool?>(readerMock.Object));
            Assert.Null(_Converter.Convert<bool?>(readerMock.Object));
        }

        [Fact]
        public void TestScalarConvertString()
        {
            var readerMock = new Mock<DbDataReader>();
            readerMock.SetupSequence(i => i.GetString(0))
                .Returns(null)
                .Returns("ss");
            readerMock.SetupSequence(i => i.IsDBNull(0))
                .Returns(false)
                .Returns(false);
            Assert.Null(_Converter.Convert<string>(readerMock.Object));
            Assert.Equal("ss", _Converter.Convert<string>(readerMock.Object));
        }

        [Fact]
        public void TestScalarConvertDateTime()
        {
            var readerMock = new Mock<DbDataReader>();
            readerMock.SetupSequence(i => i.GetDateTime(0))
                .Returns(new DateTime(1991,5,30))
                .Returns(new DateTime(1992, 5, 30));
            readerMock.SetupSequence(i => i.IsDBNull(0))
                .Returns(false)
                .Returns(true);
            Assert.Equal(new DateTime(1991, 5, 30), _Converter.Convert<DateTime>(readerMock.Object));
            Assert.Equal(new DateTime(1992, 5, 30), _Converter.Convert<DateTime?>(readerMock.Object));
            Assert.Null(_Converter.Convert<DateTime?>(readerMock.Object));
        }

        [Fact]
        public void TestScalarConvertDecimal()
        {
            var readerMock = new Mock<DbDataReader>();
            readerMock.SetupSequence(i => i.GetDecimal(0))
                .Returns(33M)
                .Returns(66.66M);
            readerMock.SetupSequence(i => i.IsDBNull(0))
                .Returns(false)
                .Returns(true);
            Assert.Equal(33M, _Converter.Convert<decimal>(readerMock.Object));
            Assert.Equal(66.66M, _Converter.Convert<decimal?>(readerMock.Object));
            Assert.Null(_Converter.Convert<decimal?>(readerMock.Object));
        }

        [Fact]
        public void TestScalarConvertDouble()
        {
            var readerMock = new Mock<DbDataReader>();
            readerMock.SetupSequence(i => i.GetDouble(0))
                .Returns(331D)
                .Returns(66.661D);
            readerMock.SetupSequence(i => i.IsDBNull(0))
                .Returns(false)
                .Returns(true);
            Assert.Equal(331D, _Converter.Convert<double>(readerMock.Object));
            Assert.Equal(66.661D, _Converter.Convert<double?>(readerMock.Object));
            Assert.Null(_Converter.Convert<double?>(readerMock.Object));
        }

        [Fact]
        public void TestScalarConvertInt32()
        {
            var readerMock = new Mock<DbDataReader>();
            readerMock.SetupSequence(i => i.GetInt32(0))
                .Returns(331)
                .Returns(66);
            readerMock.SetupSequence(i => i.IsDBNull(0))
                .Returns(false)
                .Returns(true);
            Assert.Equal(331, _Converter.Convert<int>(readerMock.Object));
            Assert.Equal(66, _Converter.Convert<int?>(readerMock.Object));
            Assert.Null(_Converter.Convert<int?>(readerMock.Object));
        }

        [Fact]
        public void TestScalarConvertFloat()
        {
            var readerMock = new Mock<DbDataReader>();
            readerMock.SetupSequence(i => i.GetFloat(0))
                .Returns(331.2F)
                .Returns(66.3F);
            readerMock.SetupSequence(i => i.IsDBNull(0))
                .Returns(false)
                .Returns(true);
            Assert.Equal(331.2F, _Converter.Convert<float>(readerMock.Object));
            Assert.Equal(66.3F, _Converter.Convert<float?>(readerMock.Object));
            Assert.Null(_Converter.Convert<float?>(readerMock.Object));
        }

        [Fact]
        public void TestScalarConvertInt16()
        {
            var readerMock = new Mock<DbDataReader>();
            readerMock.SetupSequence(i => i.GetInt16(0))
                .Returns(331)
                .Returns(6);
            readerMock.SetupSequence(i => i.IsDBNull(0))
                .Returns(false)
                .Returns(true);
            Assert.Equal(331, _Converter.Convert<short>(readerMock.Object));
            Assert.Equal(6, _Converter.Convert<short?>(readerMock.Object));
            Assert.Null(_Converter.Convert<short?>(readerMock.Object));
        }

        [Fact]
        public void TestScalarConvertByte()
        {
            var readerMock = new Mock<DbDataReader>();
            readerMock.SetupSequence(i => i.GetByte(0))
                .Returns(3)
                .Returns(66);
            readerMock.SetupSequence(i => i.IsDBNull(0))
                .Returns(false)
                .Returns(true);
            Assert.Equal(3, _Converter.Convert<byte>(readerMock.Object));
            Assert.Equal(66, _Converter.Convert<byte?>(readerMock.Object));
            Assert.Null(_Converter.Convert<byte?>(readerMock.Object));
        }


        [Fact]
        public void TestScalarConvertGuid()
        {
            var readerMock = new Mock<DbDataReader>();
            readerMock.SetupSequence(i => i.GetGuid(0))
                .Returns(Guid.Parse("E82E26DA-AED1-4FD3-BFF9-73BE66F28EED"))
                .Returns(Guid.Parse("E82E56DA-AED1-4FD3-BFF9-73BE66F28EED"));
            readerMock.SetupSequence(i => i.IsDBNull(0))
                .Returns(false)
                .Returns(true);
            Assert.Equal(Guid.Parse("E82E26DA-AED1-4FD3-BFF9-73BE66F28EED"), _Converter.Convert<Guid>(readerMock.Object));
            Assert.Equal(Guid.Parse("E82E56DA-AED1-4FD3-BFF9-73BE66F28EED"), _Converter.Convert<Guid?>(readerMock.Object));
            Assert.Null(_Converter.Convert<Guid?>(readerMock.Object));
        }
    }
}
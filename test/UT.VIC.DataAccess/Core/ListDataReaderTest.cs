using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using VIC.DataAccess.Core;
using Xunit;

namespace UT.VIC.DataAccess.Core
{
    public class People
    {
        public int Age { get; set; }

        public string Name { get; set; }

        public string CanNotReadName { private get; set; }

        public string FiledName;

        public DateTime DateTime { get; set; }

        public long Long { get; set; }

        public decimal Decimal { get; set; }

        public double Double { get; set; }

        public float Float { get; set; }

        public short Short { get; set; }

        public byte Byte { get; set; }

        public Guid Guid { get; set; }

        public bool Bool { get; set; }
    }

    public class Student : People
    {
        public DateTime? DateTime2 { get; set; }

        public int? ClassNumber { get; set; }

        public long? Long2 { get; set; }

        public decimal? Decimal2 { get; set; }

        public double? Double2 { get; set; }

        public float? Float2 { get; set; }

        public short? Short2 { get; set; }

        public byte? Byte2 { get; set; }

        public Guid? Guid2 { get; set; }

        public bool? Bool2 { get; set; }
    }

    public class ListDataReaderTest
    {
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

        private List<PropertyInfo> _Properties = TypeExtensions.GetProperties(typeof(Student),
                    BindingFlags.Instance |
                    BindingFlags.Public).ToList();

        [Fact]
        public void TestFieldCount()
        {
            var reader = new ListDataReader<Student>(_Students);
            Assert.Equal(_Properties.Where(i => i.CanRead).Count(), reader.FieldCount);
        }

        [Fact]
        public void TestIntIndexer()
        {
            var reader = new ListDataReader<Student>(_Students);
            var ps = _Properties.Where(i => i.CanRead).ToArray();
            var studentIndex = 0;
            while (reader.Read())
            {
                var index = 0;
                var st = _Students[studentIndex++];
                foreach (var p in ps)
                {
                    Assert.Equal(p.GetMethod.Invoke(st, new object[0]), reader[index]);
                    index++;
                }
            }
        }

        [Fact]
        public void TestGetName()
        {
            var reader = new ListDataReader<Student>(_Students);
            var ps = _Properties.Where(i => i.CanRead).ToArray();
            while (reader.Read())
            {
                var index = 0;
                foreach (var p in ps)
                {
                    Assert.Equal(p.Name, reader.GetName(index++));
                }
            }
        }

        [Fact]
        public void TestGetDataTypeName()
        {
            var reader = new ListDataReader<Student>(_Students);
            var ps = _Properties.Where(i => i.CanRead).ToArray();
            while (reader.Read())
            {
                var index = 0;
                foreach (var p in ps)
                {
                    Assert.Equal(p.PropertyType.Name, reader.GetDataTypeName(index++));
                }
            }
        }

        [Fact]
        public void TestGetOrdinal()
        {
            var reader = new ListDataReader<Student>(_Students);
            var ps = _Properties.Where(i => i.CanRead).ToArray();
            while (reader.Read())
            {
                var index = 0;
                foreach (var p in ps)
                {
                    Assert.Equal(index++, reader.GetOrdinal(p.Name));
                }
            }
        }

        [Fact]
        public async void TestNameIndexer()
        {
            var reader = new ListDataReader<Student>(_Students);
            var ps = _Properties.Where(i => i.CanRead).ToArray();
            foreach (var st in _Students)
            {
                await reader.ReadAsync();
                foreach (var p in ps)
                {
                    Assert.Equal(p.GetMethod.Invoke(st, new object[0]), reader[p.Name]);
                }
            }
        }

        [Fact]
        public void TestRead()
        {
            var reader = new ListDataReader<Student>(_Students);
            foreach (var st in _Students)
            {
                Assert.True(reader.Read());
            }
            Assert.False(reader.Read());
        }

        [Fact]
        public void TestNextResult()
        {
            var reader = new ListDataReader<Student>(_Students);
            Assert.False(reader.NextResult());
            foreach (var st in _Students)
            {
                reader.Read();
                Assert.False(reader.NextResult());
            }
            Assert.False(reader.NextResult());
        }

        [Fact]
        public async void TestIsDBNull()
        {
            var reader = new ListDataReader<Student>(_Students);
            var ps = _Properties.Where(i => i.CanRead).ToArray();
            foreach (var st in _Students)
            {
                await reader.ReadAsync();
                var index = 0;
                foreach (var p in ps)
                {
                    Assert.Equal(p.GetMethod.Invoke(st, new object[0]) == null, reader.IsDBNull(index++));
                }
            }
        }

        [Fact]
        public async void TestGetValue()
        {
            var reader = new ListDataReader<Student>(_Students);
            var ps = _Properties.Where(i => i.CanRead).ToArray();
            foreach (var st in _Students)
            {
                await reader.ReadAsync();
                var index = 0;
                foreach (var p in ps)
                {
                    Assert.Equal(p.GetMethod.Invoke(st, new object[0]), reader.GetValue(index++));
                }
            }
        }

        [Fact]
        public async void TestGetValues()
        {
            var reader = new ListDataReader<Student>(_Students);
            var ps = _Properties.Where(i => i.CanRead).ToArray();
            foreach (var st in _Students)
            {
                await reader.ReadAsync();
                var vs = new object[ps.Length];
                Assert.Equal(ps.Length, reader.GetValues(vs));
                var pds = ps.Select(p => p.GetMethod.Invoke(st, new object[0])).ToArray();
                for (int i = 0; i < vs.Length; i++)
                {
                    Assert.Equal(pds[i], vs[i]);
                }
            }
        }

        [Fact]
        public async void TestGetString()
        {
            var reader = new ListDataReader<Student>(_Students);
            var ps = _Properties.Where(i => i.CanRead).ToArray();
            foreach (var st in _Students)
            {
                await reader.ReadAsync();
                var index = 0;
                foreach (var p in ps)
                {
                    if (p.PropertyType == typeof(string))
                    {
                        Assert.Equal(p.GetMethod.Invoke(st, new object[0]), reader.GetString(index));
                    }
                    else if (p.PropertyType.GetTypeInfo().IsGenericType
                        && p.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>)
                        && p.GetMethod.Invoke(st, new object[0]) == null)
                    {
                        Assert.Null(reader.GetString(index));
                    }
                    else
                    {
                        Assert.ThrowsAny<RuntimeBinderException>(() => reader.GetString(index));
                    }
                    index++;
                }
            }
        }

        [Fact]
        public async void TestGetInt64()
        {
            var reader = new ListDataReader<Student>(_Students);
            var ps = _Properties.Where(i => i.CanRead).ToArray();
            foreach (var st in _Students)
            {
                await reader.ReadAsync();
                var index = 0;
                foreach (var p in ps)
                {
                    if (p.GetMethod.Invoke(st, new object[0]) != null
                        && (p.PropertyType == typeof(long) || p.PropertyType == typeof(long?)))
                    {
                        Assert.Equal(p.GetMethod.Invoke(st, new object[0]), reader.GetInt64(index));
                    }
                    else if (p.PropertyType.GetTypeInfo().IsGenericType
                        && p.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>)
                        && p.GetMethod.Invoke(st, new object[0]) == null)
                    {
                        Assert.ThrowsAny<RuntimeBinderException>(() => reader.GetInt64(index));
                    }
                    else if (p.PropertyType == typeof(int) || p.PropertyType == typeof(int?)
                       || p.PropertyType == typeof(short) || p.PropertyType == typeof(short?)
                       || p.PropertyType == typeof(byte) || p.PropertyType == typeof(byte?))
                    {
                        Assert.NotNull(reader.GetInt64(index));
                    }
                    else
                    {
                        Assert.ThrowsAny<RuntimeBinderException>(() => reader.GetInt64(index));
                    }
                    index++;
                }
            }
        }

        [Fact]
        public async void TestGetInt32()
        {
            var reader = new ListDataReader<Student>(_Students);
            var ps = _Properties.Where(i => i.CanRead).ToArray();
            foreach (var st in _Students)
            {
                await reader.ReadAsync();
                var index = 0;
                foreach (var p in ps)
                {
                    if (p.GetMethod.Invoke(st, new object[0]) != null
                        && (p.PropertyType == typeof(int) || p.PropertyType == typeof(int?)))
                    {
                        Assert.Equal(p.GetMethod.Invoke(st, new object[0]), reader.GetInt32(index));
                    }
                    else if (p.PropertyType.GetTypeInfo().IsGenericType
                        && p.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>)
                        && p.GetMethod.Invoke(st, new object[0]) == null)
                    {
                        Assert.ThrowsAny<RuntimeBinderException>(() => reader.GetInt32(index));
                    }
                    else if (p.PropertyType == typeof(short) || p.PropertyType == typeof(short?)
                       || p.PropertyType == typeof(byte) || p.PropertyType == typeof(byte?))
                    {
                        Assert.NotNull(reader.GetInt32(index));
                    }
                    else
                    {
                        Assert.ThrowsAny<RuntimeBinderException>(() => reader.GetInt32(index));
                    }
                    index++;
                }
            }
        }

        [Fact]
        public async void TestGetInt16()
        {
            var reader = new ListDataReader<Student>(_Students);
            var ps = _Properties.Where(i => i.CanRead).ToArray();
            foreach (var st in _Students)
            {
                await reader.ReadAsync();
                var index = 0;
                foreach (var p in ps)
                {
                    if (p.GetMethod.Invoke(st, new object[0]) != null
                        && (p.PropertyType == typeof(short) || p.PropertyType == typeof(short?)))
                    {
                        Assert.Equal(p.GetMethod.Invoke(st, new object[0]), reader.GetInt16(index));
                    }
                    else if (p.PropertyType.GetTypeInfo().IsGenericType
                        && p.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>)
                        && p.GetMethod.Invoke(st, new object[0]) == null)
                    {
                        Assert.ThrowsAny<RuntimeBinderException>(() => reader.GetInt16(index));
                    }
                    else if (p.PropertyType == typeof(byte) || p.PropertyType == typeof(byte?))
                    {
                        Assert.NotNull(reader.GetInt16(index));
                    }
                    else
                    {
                        Assert.ThrowsAny<RuntimeBinderException>(() => reader.GetInt16(index));
                    }
                    index++;
                }
            }
        }

        [Fact]
        public async void TestGetFloat()
        {
            var reader = new ListDataReader<Student>(_Students);
            var ps = _Properties.Where(i => i.CanRead).ToArray();
            foreach (var st in _Students)
            {
                await reader.ReadAsync();
                var index = 0;
                foreach (var p in ps)
                {
                    if (p.GetMethod.Invoke(st, new object[0]) != null
                        && (p.PropertyType == typeof(float) || p.PropertyType == typeof(float?)))
                    {
                        Assert.Equal(p.GetMethod.Invoke(st, new object[0]), reader.GetFloat(index));
                    }
                    else if (p.PropertyType.GetTypeInfo().IsGenericType
                        && p.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>)
                        && p.GetMethod.Invoke(st, new object[0]) == null)
                    {
                        Assert.ThrowsAny<RuntimeBinderException>(() => reader.GetFloat(index));
                    }
                    else if (p.PropertyType == typeof(short) || p.PropertyType == typeof(short?)
                        || p.PropertyType == typeof(int) || p.PropertyType == typeof(int?)
                        || p.PropertyType == typeof(byte) || p.PropertyType == typeof(byte?)
                        || p.PropertyType == typeof(long) || p.PropertyType == typeof(long?))
                    {
                        Assert.NotNull(reader.GetFloat(index));
                    }
                    else
                    {
                        Assert.ThrowsAny<RuntimeBinderException>(() => reader.GetFloat(index));
                    }
                    index++;
                }
            }
        }

        [Fact]
        public async void TestGetDouble()
        {
            var reader = new ListDataReader<Student>(_Students);
            var ps = _Properties.Where(i => i.CanRead).ToArray();
            foreach (var st in _Students)
            {
                await reader.ReadAsync();
                var index = 0;
                foreach (var p in ps)
                {
                    if (p.GetMethod.Invoke(st, new object[0]) != null
                        && (p.PropertyType == typeof(double) || p.PropertyType == typeof(double?)))
                    {
                        Assert.Equal(p.GetMethod.Invoke(st, new object[0]), reader.GetDouble(index));
                    }
                    else if (p.PropertyType.GetTypeInfo().IsGenericType
                        && p.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>)
                        && p.GetMethod.Invoke(st, new object[0]) == null)
                    {
                        Assert.ThrowsAny<RuntimeBinderException>(() => reader.GetDouble(index));
                    }
                    else if (p.PropertyType == typeof(short) || p.PropertyType == typeof(short?)
                        || p.PropertyType == typeof(int) || p.PropertyType == typeof(int?)
                        || p.PropertyType == typeof(byte) || p.PropertyType == typeof(byte?)
                        || p.PropertyType == typeof(long) || p.PropertyType == typeof(long?)
                        || p.PropertyType == typeof(float) || p.PropertyType == typeof(float?))
                    {
                        Assert.NotNull(reader.GetDouble(index));
                    }
                    else
                    {
                        Assert.ThrowsAny<RuntimeBinderException>(() => reader.GetDouble(index));
                    }
                    index++;
                }
            }
        }

        [Fact]
        public async void TestGetDecimal()
        {
            var reader = new ListDataReader<Student>(_Students);
            var ps = _Properties.Where(i => i.CanRead).ToArray();
            foreach (var st in _Students)
            {
                await reader.ReadAsync();
                var index = 0;
                foreach (var p in ps)
                {
                    if (p.GetMethod.Invoke(st, new object[0]) != null
                        && (p.PropertyType == typeof(decimal) || p.PropertyType == typeof(decimal?)))
                    {
                        Assert.Equal(p.GetMethod.Invoke(st, new object[0]), reader.GetDecimal(index));
                    }
                    else if (p.PropertyType.GetTypeInfo().IsGenericType
                        && p.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>)
                        && p.GetMethod.Invoke(st, new object[0]) == null)
                    {
                        Assert.ThrowsAny<RuntimeBinderException>(() => reader.GetDecimal(index));
                    }
                    else if (p.PropertyType == typeof(short) || p.PropertyType == typeof(short?)
                        || p.PropertyType == typeof(int) || p.PropertyType == typeof(int?)
                        || p.PropertyType == typeof(byte) || p.PropertyType == typeof(byte?)
                        || p.PropertyType == typeof(long) || p.PropertyType == typeof(long?))
                    {
                        Assert.NotNull(reader.GetDecimal(index));
                    }
                    else
                    {
                        Assert.ThrowsAny<RuntimeBinderException>(() => reader.GetDecimal(index));
                    }
                    index++;
                }
            }
        }

        [Fact]
        public async void TestGetByte()
        {
            var reader = new ListDataReader<Student>(_Students);
            var ps = _Properties.Where(i => i.CanRead).ToArray();
            foreach (var st in _Students)
            {
                await reader.ReadAsync();
                var index = 0;
                foreach (var p in ps)
                {
                    if (p.GetMethod.Invoke(st, new object[0]) != null
                        && (p.PropertyType == typeof(byte) || p.PropertyType == typeof(byte?)))
                    {
                        Assert.Equal(p.GetMethod.Invoke(st, new object[0]), reader.GetByte(index));
                    }
                    else if (p.PropertyType.GetTypeInfo().IsGenericType
                        && p.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>)
                        && p.GetMethod.Invoke(st, new object[0]) == null)
                    {
                        Assert.ThrowsAny<RuntimeBinderException>(() => reader.GetByte(index));
                    }
                    else
                    {
                        Assert.ThrowsAny<RuntimeBinderException>(() => reader.GetByte(index));
                    }
                    index++;
                }
            }
        }

        [Fact]
        public async void TestGetBoolean()
        {
            var reader = new ListDataReader<Student>(_Students);
            var ps = _Properties.Where(i => i.CanRead).ToArray();
            foreach (var st in _Students)
            {
                await reader.ReadAsync();
                var index = 0;
                foreach (var p in ps)
                {
                    if (p.PropertyType == typeof(bool) || (p.PropertyType == typeof(bool?) && p.GetMethod.Invoke(st, new object[0]) != null))
                    {
                        Assert.Equal(p.GetMethod.Invoke(st, new object[0]), reader.GetBoolean(index));
                    }
                    else if (p.PropertyType.GetTypeInfo().IsGenericType
                        && p.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>)
                        && p.GetMethod.Invoke(st, new object[0]) == null)
                    {
                        Assert.ThrowsAny<RuntimeBinderException>(() => reader.GetBoolean(index));
                    }
                    else
                    {
                        Assert.ThrowsAny<RuntimeBinderException>(() => reader.GetBoolean(index));
                    }
                    index++;
                }
            }
        }

        [Fact]
        public async void TestGetDateTime()
        {
            var reader = new ListDataReader<Student>(_Students);
            var ps = _Properties.Where(i => i.CanRead).ToArray();
            foreach (var st in _Students)
            {
                await reader.ReadAsync();
                var index = 0;
                foreach (var p in ps)
                {
                    if (p.PropertyType == typeof(DateTime) || (p.PropertyType == typeof(DateTime?) && p.GetMethod.Invoke(st, new object[0]) != null))
                    {
                        Assert.Equal(p.GetMethod.Invoke(st, new object[0]), reader.GetDateTime(index));
                    }
                    else if (p.PropertyType.GetTypeInfo().IsGenericType
                        && p.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>)
                        && p.GetMethod.Invoke(st, new object[0]) == null)
                    {
                        Assert.ThrowsAny<RuntimeBinderException>(() => reader.GetDateTime(index));
                    }
                    else
                    {
                        Assert.ThrowsAny<RuntimeBinderException>(() => reader.GetDateTime(index));
                    }
                    index++;
                }
            }
        }
    }
}
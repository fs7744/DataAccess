using Moq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using UT.VIC.DataAccess.Core.Converter;
using VIC.DataAccess;
using VIC.DataAccess.Core;
using VIC.DataAccess.Core.Converter;
using Xunit;
using System.Linq;

namespace UT.VIC.DataAccess.Core
{
    public class SetDataReader<T> : ListDataReader<T>
    {
        private IEnumerator<IEnumerable<T>> _Set;

        public SetDataReader(IEnumerator<IEnumerable<T>> data) : base(data.Current)
        {
            _Set = data;
        }

        public override bool NextResult()
        {
            var result = _Set.MoveNext();
            if (result && _Set.Current != null)
                _Data = _Set.Current.GetEnumerator();
            return result;
        }
    }

    public class DataCommandTest
    {
        private static List<Student> _Students = new List<Student>()
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

        public class TestDbParameterCollection : DbParameterCollection
        {
            List<DbParameter> PS = new List<DbParameter>();

            public override int Count
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public override object SyncRoot
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public override int Add(object value)
            {
                PS.Add(value as DbParameter);
                return 1;
            }

            public override void AddRange(Array values)
            {
                foreach (DbParameter item in values)
                {
                    PS.Add(item);
                }
            }

            public override void Clear()
            {
                PS.Clear();
            }

            public override bool Contains(string value)
            {
                throw new NotImplementedException();
            }

            public override bool Contains(object value)
            {
                throw new NotImplementedException();
            }

            public override void CopyTo(Array array, int index)
            {
                throw new NotImplementedException();
            }

            public override IEnumerator GetEnumerator()
            {
                throw new NotImplementedException();
            }

            public override int IndexOf(string parameterName)
            {
                throw new NotImplementedException();
            }

            public override int IndexOf(object value)
            {
                throw new NotImplementedException();
            }

            public override void Insert(int index, object value)
            {
                throw new NotImplementedException();
            }

            public override void Remove(object value)
            {
                throw new NotImplementedException();
            }

            public override void RemoveAt(string parameterName)
            {
                throw new NotImplementedException();
            }

            public override void RemoveAt(int index)
            {
                throw new NotImplementedException();
            }

            protected override DbParameter GetParameter(string parameterName)
            {
                return PS.FirstOrDefault(i => i.ParameterName == parameterName);
            }

            protected override DbParameter GetParameter(int index)
            {
                throw new NotImplementedException();
            }

            protected override void SetParameter(string parameterName, DbParameter value)
            {
                throw new NotImplementedException();
            }

            protected override void SetParameter(int index, DbParameter value)
            {
                throw new NotImplementedException();
            }
        }

        public class TestDbConnection : DbConnection
        {
            public TestDbCommand Command;

            public override string ConnectionString { get; set; }

            public override string Database { get { return ConnectionString; } }

            public override string DataSource { get { return ConnectionString; } }

            public override string ServerVersion { get { return ConnectionString; } }

            private ConnectionState _State;

            public override ConnectionState State
            {
                get
                {
                    return _State;
                }
            }

            public override void ChangeDatabase(string databaseName)
            {
                throw new NotImplementedException();
            }

            public override void Close()
            {
                _State = ConnectionState.Closed;
            }

            public override void Open()
            {
                _State = ConnectionState.Open;
            }

            protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
            {
                var mockTran = new Mock<DbTransaction>();
                mockTran.SetupGet(x => x.IsolationLevel).Returns(isolationLevel);
                return mockTran.Object;
            }

            protected override DbCommand CreateDbCommand()
            {
                Command = new TestDbCommand();
                return Command;
            }
        }

        public class TestDbCommand : DbCommand
        {
            public override string CommandText { get; set; }

            public override int CommandTimeout { get; set; }

            public override CommandType CommandType { get; set; }

            public override bool DesignTimeVisible { get; set; }

            public override UpdateRowSource UpdatedRowSource { get; set; }

            protected override DbConnection DbConnection { get; set; }

            protected override DbParameterCollection DbParameterCollection { get; } = new TestDbParameterCollection();

            protected override DbTransaction DbTransaction { get; set; }

            public override void Cancel()
            {
                throw new NotImplementedException();
            }

            public override int ExecuteNonQuery()
            {
                return 3;
            }

            public override object ExecuteScalar()
            {
                throw new NotImplementedException();
            }

            public override void Prepare()
            {
                throw new NotImplementedException();
            }

            protected override DbParameter CreateDbParameter()
            {
                return new TestParam();
            }

            protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
            {
                var data = new List<List<Student>>() { _Students, _Students, _Students }.GetEnumerator();
                data.MoveNext();
                return new SetDataReader<Student>(data);
            }
        }

        public class TestDataCommand : DataCommand
        {
            public TestDbConnection Connection;

            public TestDataCommand() : base(new TestParamConverter(new DbTypeConverter()), new ScalarConverter(), new EntityConverter())
            {
            }

            protected override DbConnection CreateConnection(string connectionString)
            {
                Connection = new TestDbConnection();
                return Connection;
            }
        }

        private Action<Student, Student> _Test = (item, s) =>
        {
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
        };

        [Fact]
        public void TestAddPreParam()
        {
            var command = new TestDataCommand();
            var p = new DataParameter() { ParameterName = "test" };
            command.PreParameters.Add(p);
            Assert.True(command.PreParameters.Contains(p));
            command.Text = "sql";
            Assert.Equal("sql", command.Text);
            command.ConnectionString = "sqlConnectionString";
            Assert.Equal("sqlConnectionString", command.ConnectionString);
            command.Timeout = 1003;
            Assert.Equal(1003, command.Timeout);
            Assert.Equal(CommandType.Text, command.Type);
            command.Type = CommandType.StoredProcedure;
            Assert.Equal(CommandType.StoredProcedure, command.Type);
        }

        [Theory]
        [InlineData(IsolationLevel.Chaos)]
        [InlineData(IsolationLevel.ReadCommitted)]
        [InlineData(IsolationLevel.ReadUncommitted)]
        [InlineData(IsolationLevel.RepeatableRead)]
        [InlineData(IsolationLevel.Serializable)]
        [InlineData(IsolationLevel.Snapshot)]
        [InlineData(IsolationLevel.Unspecified)]
        public void TestBeginTransaction(IsolationLevel i)
        {
            using (var command = new TestDataCommand())
            {
                command.ConnectionString = "sqlConnectionString";
                var tran = command.BeginTransaction(i);
                Assert.Equal(i, tran.IsolationLevel);
            }
        }

        [Fact]
        public async void TestExecuteDataReaderAsync()
        {
            using (var command = new TestDataCommand())
            {
                command.ConnectionString = "sqlConnectionString";
                var reader = await command.ExecuteDataReaderAsync(_Students[0]);
                Assert.IsType<SetDataReader<Student>>(reader);
            }
        }

        [Fact]
        public void TestExecuteDataReader()
        {
            using (var command = new TestDataCommand())
            {
                command.ConnectionString = "sqlConnectionString";
                var reader = command.ExecuteDataReader(_Students[0]);
                Assert.IsType<SetDataReader<Student>>(reader);
            }
        }

        [Fact]
        public async void TestExecuteEntityAsync()
        {
            using (var command = new TestDataCommand())
            {
                command.ConnectionString = "sqlConnectionString";
                var s = await command.ExecuteEntityAsync<Student>(_Students[1]);
                _Test(_Students[0], s);
            }
        }

        [Fact]
        public void TestExecuteEntity()
        {
            using (var command = new TestDataCommand())
            {
                command.ConnectionString = "sqlConnectionString";
                var s = command.ExecuteEntity<Student>(_Students[1]);
                _Test(_Students[0], s);
            }
        }

        [Fact]
        public async void TestExecuteEntityListAsync()
        {
            using (var command = new TestDataCommand())
            {
                command.ConnectionString = "sqlConnectionString";
                var ss = await command.ExecuteEntityListAsync<Student>(_Students[1]);
                Assert.Equal(_Students.Count, ss.Count);
                for (int i = 0; i < ss.Count; i++)
                {
                    _Test(_Students[i], ss[i]);
                }
            }
        }

        [Fact]
        public async void TestExecuteScalarListAsync()
        {
            using (var command = new TestDataCommand())
            {
                command.ConnectionString = "sqlConnectionString";
                var ss = await command.ExecuteScalarListAsync<DateTime>(_Students[1]);
                Assert.Equal(_Students.Count, ss.Count);
                for (int i = 0; i < ss.Count; i++)
                {
                    Assert.Equal(_Students[i].DateTime2, ss[i]);
                }
            }
        }

        [Fact]
        public void TestExecuteScalarList()
        {
            using (var command = new TestDataCommand())
            {
                command.ConnectionString = "sqlConnectionString";
                var ss = command.ExecuteScalarList<DateTime>(_Students[1]);
                Assert.Equal(_Students.Count, ss.Count);
                for (int i = 0; i < ss.Count; i++)
                {
                    Assert.Equal(_Students[i].DateTime2, ss[i]);
                }
            }
        }

        [Fact]
        public void TestExecuteEntityList()
        {
            using (var command = new TestDataCommand())
            {
                command.ConnectionString = "sqlConnectionString";
                var ss = command.ExecuteEntityList<Student>(_Students[1]);
                Assert.Equal(_Students.Count, ss.Count);
                for (int i = 0; i < ss.Count; i++)
                {
                    _Test(_Students[i], ss[i]);
                }
            }
        }

        [Fact]
        public async void TestExecuteMultipleReaderAsync()
        {
            using (var command = new TestDataCommand())
            {
                command.ConnectionString = "sqlConnectionString";
                using (var reader = await command.ExecuteMultipleAsync(_Students[2]))
                {
                    var s = await reader.ExecuteEntityAsync<Student>();
                    _Test(_Students[0], s);
                    var ss = await reader.ExecuteEntityListAsync<Student>();
                    Assert.Equal(_Students.Count, ss.Count);
                    for (int i = 0; i < ss.Count; i++)
                    {
                        _Test(_Students[i], ss[i]);
                    }
                    var s1 = await reader.ExecuteScalarAsync<DateTime?>();
                    Assert.Equal(_Students[0].DateTime2, s1);
                }
            }
        }

        [Fact]
        public void TestExecuteMultipleReader()
        {
            using (var command = new TestDataCommand())
            {
                command.ConnectionString = "sqlConnectionString";
                using (var reader = command.ExecuteMultiple(_Students[2]))
                {
                    var s =  reader.ExecuteEntity<Student>();
                    _Test(_Students[0], s);
                    var ss =  reader.ExecuteEntityList<Student>();
                    Assert.Equal(_Students.Count, ss.Count);
                    for (int i = 0; i < ss.Count; i++)
                    {
                        _Test(_Students[i], ss[i]);
                    }
                    var s1 =  reader.ExecuteScalar<DateTime?>();
                    Assert.Equal(_Students[0].DateTime2, s1);
                }
            }
        }

        [Fact]
        public async void TestExecuteNonQueryAsync()
        {
            using (var command = new TestDataCommand())
            {
                command.ConnectionString = "sqlConnectionString";
                var num = await command.ExecuteNonQueryAsync(_Students[2]);
                Assert.Equal(3, num);
            }
        }

        [Fact]
        public void TestExecuteNonQuery()
        {
            using (var command = new TestDataCommand())
            {
                command.ConnectionString = "sqlConnectionString";
                var num = command.ExecuteNonQuery(_Students[2]);
                Assert.Equal(3, num);
                num = 0;
                num = command.ExecuteNonQuery();
                Assert.Equal(3, num);
            }
        }

        [Fact]
        public void TestExecuteNonQueryList()
        {
            using (var command = new TestDataCommand())
            {
                command.Text = "update top(1) Name = @Name where Age = @Age";
                command.ConnectionString = "sqlConnectionString";
                var num = command.ExecuteNonQuerys(_Students);
                var com = command.Connection.Command;
                Assert.Equal("update top(1) Name = @Name0 where Age = @Age0;update top(1) Name = @Name1 where Age = @Age1;update top(1) Name = @Name2 where Age = @Age2;", com.CommandText);
                for (int i = 0; i < _Students.Count; i++)
                {
                    Assert.Equal(_Students[i].Name, com.Parameters[$"@Name{i}"].Value);
                    Assert.Equal(_Students[i].Age, com.Parameters[$"@Age{i}"].Value);
                }
            }
        }

        [Fact]
        public async void TestExecuteNonQueryAsyncList()
        {
            using (var command = new TestDataCommand())
            {
                command.Text = "update top(1) Name = @Name where Age = @Age";
                command.ConnectionString = "sqlConnectionString";
                var num = await command.ExecuteNonQuerysAsync(_Students);
                var com = command.Connection.Command;
                Assert.Equal("update top(1) Name = @Name0 where Age = @Age0;update top(1) Name = @Name1 where Age = @Age1;update top(1) Name = @Name2 where Age = @Age2;", com.CommandText);
                for (int i = 0; i < _Students.Count; i++)
                {
                    Assert.Equal(_Students[i].Name, com.Parameters[$"@Name{i}"].Value);
                    Assert.Equal(_Students[i].Age, com.Parameters[$"@Age{i}"].Value);
                }
            }
        }

        [Fact]
        public void TestInParams()
        {
            using (var command = new TestDataCommand())
            {
                command.Text = "update top(1) Name = @Name where Age in @Age";
                command.ConnectionString = "sqlConnectionString";
                var num = command.ExecuteNonQuery(new { Name = "1", Age = new List<int>() { 1,2,3} });
                var com = command.Connection.Command;
                Assert.Equal("update top(1) Name = @Name where Age in (@Age0,@Age1,@Age2)", com.CommandText);
                Assert.Equal("1", com.Parameters["@Name"].Value);
                for (int i = 0; i < 3; i++)
                {
                    var p = com.Parameters[$"@Age{i}"];
                    Assert.Equal(i + 1, p.Value);
                    Assert.Equal(DbType.Int32, p.DbType);
                }
            }
        }
    }
}
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using VIC.DataAccess;
using VIC.DataAccess.Abstraction;
using VIC.DataAccess.Abstraction.Converter;
using VIC.DataAccess.Config;
using VIC.DataAccess.Core;
using VIC.DataAccess.Core.Converter;
using Xunit;

namespace UT.VIC.DataAccess.Config
{
    public class TestParamConverter : ParamConverter
    {
        public TestParamConverter(IDbTypeConverter dc) : base(dc)
        {
        }

        protected override Type GetParameterType()
        {
            return typeof(TestParamConverter);
        }
    }

    public class TestDataCommand : DataCommand
    {
        public TestDataCommand() : base(new TestParamConverter(new DbTypeConverter()), new ScalarConverter(), new EntityConverter(new DbFuncNameConverter()))
        {
        }

        protected override DbConnection CreateConnection(string connectionString)
        {
            return new TestDbConnection();
        }
    }


    public class TestDbConnection : DbConnection
    {
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
            throw new NotImplementedException();
        }

        protected override DbCommand CreateDbCommand()
        {
            throw new NotImplementedException();
        }
    }

    public class TestUseDataAccessConfig
    {
        [Fact]
        public void TestNoConfig()
        {
            var sp = new ServiceCollection()
                .UseDataAccessConfig(Directory.GetCurrentDirectory(), true, "test0.xml", "test1.xml")
                .BuildServiceProvider();

            var db = sp.GetService<IDbManager>();

            var command = db.GetCommand("select");
            Assert.Null(command);
        }

        private DbConfig c = new DbConfig()
        {
            ConnectionStrings = new List<DataConnection>() { new DataConnection() { ConnectionString = "test", Name = "te" } },
            SqlConfigs = new List<DbSql>() { new DbSql()
            {
                 CommandName = "select",
                  ConnectionName = "te",
                   Text =  "sql",
                    Timeout = 100,
                     Type = System.Data.CommandType.TableDirect,
                      PreParameters = new List<Parameter>()
                      { new Parameter()
                      { Type = System.Data.DbType.AnsiStringFixedLength,
                           Direction = System.Data.ParameterDirection.Input,
                            Name = "@go",
                             Size = 56,
                              IsNullable = false,
                               Precision = 8,
                                Scale = 8
                      } }
            } }
        };

        [Fact]
        public void TestHasConfig()
        {
            //var stream = new MemoryStream();
            //var serializer = new System.Xml.Serialization.XmlSerializer(typeof(DbConfig));
            //serializer.Serialize(stream, c);
            //stream.Position = 0L;
            //using (TextReader rader = new StreamReader(stream))
            //{
            //    var d = rader.ReadToEnd();
            //}
            var sp = new ServiceCollection()
                .AddTransient<IDataCommand,TestDataCommand>()
                .UseDataAccessConfig(Directory.GetCurrentDirectory(), true, "test10.xml", "test11.xml", "test12.xml")
                .BuildServiceProvider();

            var db = sp.GetService<IDbManager>();

            var command = db.GetCommand("select");
            Assert.NotNull(command);
            Assert.Equal("test", command.ConnectionString);
            Assert.Equal(100, command.Timeout);
            Assert.Equal(CommandType.TableDirect, command.Type);
            Assert.Equal("sql", command.Text);
            Assert.Equal(2, command.PreParameters.Count);
            var p = command.PreParameters["@go"];
            Assert.Equal("@go", p.ParameterName);
            Assert.Equal(ParameterDirection.Input, p.Direction);
            Assert.Equal(DbType.AnsiStringFixedLength, p.DbType);
            Assert.Equal(56, p.Size);
            Assert.Equal(false, p.IsNullable);
            Assert.Equal(8, p.Precision);
            Assert.Equal(8, p.Scale);

            p = command.PreParameters["@go1"];
            Assert.Equal("@go1", p.ParameterName);
            Assert.Equal(ParameterDirection.Output, p.Direction);
            Assert.Equal(DbType.AnsiString, p.DbType);
            Assert.Equal(0, p.Size);
            Assert.Equal(true, p.IsNullable);
            Assert.Equal(0, p.Precision);
            Assert.Equal(0, p.Scale);

            var sql = @"
      1
      2
      ";

            command = db.GetCommand("select1");
            Assert.NotNull(command);
            Assert.Equal("test1", command.ConnectionString);
            Assert.Equal(0, command.Timeout);
            Assert.Equal(CommandType.Text, command.Type);
            Assert.Equal(sql, command.Text);

            command = db.GetCommand("select2");
            Assert.Null(command);
        }
    }
}
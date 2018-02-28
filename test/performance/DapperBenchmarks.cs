//using BenchmarkDotNet.Attributes;
//using BenchmarkDotNet.Attributes.Columns;
//using System;
//using System.Collections.Generic;
//using System.Data.Common;
//using System.Text;
//using System.Data;
//using System.Data.SqlClient;
//using Microsoft.Extensions.DependencyInjection;
//using Dapper;
//using System.Linq;
//using Chloe.Infrastructure;
//using Chloe.SqlServer;
//using VIC.DataAccess.MSSql;
//using VIC.DataAccess.MSSql.Core;
//using VIC.DataAccess.Abstraction.Converter;
//using VIC.DataAccess.Abstraction;
//using VIC.DataAccess.Core.Converter;
//using VIC.DataAccess.MSSql.Core.Converter;
//using UT.VIC.DataAccess.MSSql;

//namespace performance
//{
//    public class TestConnection : DbConnection
//    {
//        public override string ConnectionString { get; set; }

//        public override string Database => "";

//        public override string DataSource => "";

//        public override string ServerVersion => "";

//        public override ConnectionState State => ConnectionState.Open;

//        public override void ChangeDatabase(string databaseName)
//        {

//        }

//        public override void Close()
//        {

//        }

//        public override void Open()
//        {

//        }

//        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
//        {
//            return null;
//        }

//        protected override DbCommand CreateDbCommand()
//        {
//            return new TestCommand();
//        }
//    }

//    public class TestCommand : DbCommand
//    {
//        private List<UT.VIC.DataAccess.MSSql.Student> _Students = new List<Student>()
//        {
//            new Student()
//            {
//                Age = 1,
//                Name = "Victor1",
//                ClassNumber = 2,
//                Long = 3L,
//                Decimal = 4M,
//                Byte = 2,
//                DateTime = new DateTime(1990,2,3),
//                Double = 4.4D,
//                Float = 33.3F,
//                Short = 77,
//                Guid = Guid.Parse("E82E26DA-AED1-4FD3-BFF9-73BE66F28EED"),
//                Bool = true
//            },
//            new Student() { Age = 2, Name = "Victor2" },
//            new Student()
//            {
//                Age = 3,
//                Name = "Victor3",
//                ClassNumber = 2,
//                Long2 = 3L,
//                Decimal2 = 4M,
//                Byte2 = 2,
//                DateTime2 = new DateTime(1990,2,3),
//                Double2 = 4.4D,
//                Float2 = 33.3F,
//                Short2 = 77,
//                Guid2 = Guid.Parse("E82E26DA-AED1-4FD3-BFF9-73BE66F28EED"),
//                Bool2 = false
//            },
//        };

//        public override string CommandText { get; set; }
//        public override int CommandTimeout { get; set; }
//        public override CommandType CommandType { get; set; }
//        public override bool DesignTimeVisible { get; set; }
//        public override UpdateRowSource UpdatedRowSource { get; set; }
//        protected override DbConnection DbConnection { get; set; }

//        protected override DbParameterCollection DbParameterCollection => null;

//        protected override DbTransaction DbTransaction { get; set; }

//        public override void Cancel()
//        {
            
//        }

//        public override int ExecuteNonQuery()
//        {
//            return 0;
//        }

//        public override object ExecuteScalar()
//        {
//            return 0;
//        }

//        public override void Prepare()
//        {
            
//        }

//        protected override DbParameter CreateDbParameter()
//        {
//            return null;
//        }

//        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
//        {
//            return new ListDataReader<Student>(_Students);
//        }
//    }

//    public class TestIDbConnectionFactory : IDbConnectionFactory
//    {
//        private TestConnection conn;
//        public TestIDbConnectionFactory(TestConnection con)
//        {
//            conn = con;
//        }

//        public IDbConnection CreateConnection()
//        {
//            return conn;
//        }
//    }

//    [AllStatisticsColumn]
//    [MemoryDiagnoser]
//    public class DapperBenchmarks
//    {
//        private static TestConnection connection = new TestConnection();
//        private IEntityConverter<Student> converter;
//        private TestIDbConnectionFactory testI;
//        private MsSqlContext context;
//        private IServiceProvider sp;
//        public DapperBenchmarks()
//        {
//            testI = new TestIDbConnectionFactory(connection);
//            context = new MsSqlContext(testI);
//            sp = new ServiceCollection()
//                .AddSingleton<IDbFuncNameConverter, DbFuncNameConverter>()
//                .AddSingleton<IDbTypeConverter, DbTypeConverter>()
//                .AddSingleton<IScalarConverter, ScalarConverter>()
//                .AddSingleton<IEntityConverter, EntityConverter>()
//                .AddSingleton<IParamConverter, MSSqlParamConverter>()
//                .AddTransient<IDataCommand, TestMSSqlDataCommand>()
//                .BuildServiceProvider();
//            var com = sp.GetRequiredService<IDataCommand>();
//            com.ConnectionString = "te";
//            com.ExecuteEntityList<Student>();
//            //i.GetRequiredService<ServiceLocator>();
//            //converter = i.GetRequiredService<IEntityConverter<Student>>();
//            connection.Query<Student>("").ToList();
//            //connection.CreateCommand().ExecuteEntities<Student>().ToList();
//            context.SqlQuery<Student>("").ToList();
//        }

//        public class TestMSSqlDataCommand : MSSqlDataCommand
//        {
//            public TestMSSqlDataCommand(IParamConverter pc, IScalarConverter sc, IEntityConverter ec) : base(pc, sc, ec)
//            {
//            }

//            protected override DbConnection CreateConnection(string connectionString)
//            {
//                return connection;
//            }
//        }

//        [Benchmark]
//        public void Dapper()
//        {
//            connection.Query<Student>("").ToList();
//        }

//        [Benchmark]
//        public void VicData()
//        {
//            var com = sp.GetRequiredService<IDataCommand>();
//            com.ConnectionString = "te";
//            com.ExecuteEntityList<Student>();
//            //connection.CreateCommand().ExecuteEntities<Student>().ToList();
//        }

//        [Benchmark]
//        public void Chloe()
//        {
//            context.SqlQuery<Student>("").ToList();
//        }

//        [Benchmark]
//        public void VicDataOnlyConverter()
//        {
//            var com = sp.GetRequiredService<IDataCommand>();
//            com.ConnectionString = "te";
//            var reader = com.ExecuteDataReader();
//            var converter = sp.GetRequiredService<IEntityConverter>().GetConverter<Student>(reader);
//            while (reader.Read())
//            {
//                converter(reader);
//            }
//        }
//    }
//}

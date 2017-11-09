using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Threading.Tasks;
using VIC.DataAccess.Abstraction.Converter;
using VIC.DataAccess.Core;

namespace VIC.DataAccess.MSSql.Core
{
    public class MSSqlDataCommand : DataCommand
    {
        public MSSqlDataCommand(IParamConverter pc, IScalarConverter sc, IEntityConverter ec) : base(pc, sc, ec)
        {
        }

        protected override DbConnection CreateConnection(string connectionString)
        {
            return new SqlConnection(connectionString);
        }

        public async override Task ExecuteBulkCopyAsync<T>(List<T> data)
        {
            var conn = _Conn as SqlConnection;
            await conn.OpenAsync();
            try
            {
                using (var sqlBulkCopy = new SqlBulkCopy(conn))
                {
                    sqlBulkCopy.DestinationTableName = Text;
                    var reader = new BulkCopyDataReader<T>(data);
                    reader.ColumnMappings.ForEach(i => sqlBulkCopy.ColumnMappings.Add(i));
                    await sqlBulkCopy.WriteToServerAsync(reader);
                }
            }
            finally
            {
                conn.Close();
            }
        }

        public override void ExecuteBulkCopy<T>(List<T> data)
        {
            var conn = _Conn as SqlConnection;
            conn.Open();
            try
            {
                using (var sqlBulkCopy = new SqlBulkCopy(conn))
                {
                    sqlBulkCopy.DestinationTableName = Text;
                    var reader = new BulkCopyDataReader<T>(data);
                    reader.ColumnMappings.ForEach(i => sqlBulkCopy.ColumnMappings.Add(i));
                    sqlBulkCopy.WriteToServer(reader);
                }
            }
            finally
            {
                conn.Close();
            }
        }
    }
}
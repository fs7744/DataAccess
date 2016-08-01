using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Threading.Tasks;
using VIC.DataAccess.Core;

namespace VIC.DataAccess.MSSql.Core
{
    public class MSSqlDataCommand : SqlDataCommand
    {
        public MSSqlDataCommand(DbSql sql) : base(sql)
        {
        }

        protected override DbConnection CreateConnection(string connectionString)
        {
            return new SqlConnection(connectionString);
        }

        public async Task ExecuteBulkCopyAsync<T>(List<T> data) where T : class, new()
        {
            var conn = _Conn as SqlConnection;
            await conn.OpenAsync();
            using (var sqlBulkCopy = new SqlBulkCopy(conn))
            {
                sqlBulkCopy.DestinationTableName = CommandText;
                var reader = new BulkCopyDataReader<T>(data);
                reader.ColumnMappings.ForEach(i => sqlBulkCopy.ColumnMappings.Add(i));
                await sqlBulkCopy.WriteToServerAsync(reader);
            }
        }
    }
}
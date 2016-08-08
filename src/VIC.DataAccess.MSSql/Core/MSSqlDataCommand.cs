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

        public async Task ExecuteBulkCopyAsync<T>(List<T> data) where T : class, new()
        {
            var conn = _Conn as SqlConnection;
            await conn.OpenAsync();
            using (var sqlBulkCopy = new SqlBulkCopy(conn))
            {
                sqlBulkCopy.DestinationTableName = Text;
                var reader = new BulkCopyDataReader<T>(data);
                reader.ColumnMappings.ForEach(i => sqlBulkCopy.ColumnMappings.Add(i));
                await sqlBulkCopy.WriteToServerAsync(reader);
            }
        }
    }
}
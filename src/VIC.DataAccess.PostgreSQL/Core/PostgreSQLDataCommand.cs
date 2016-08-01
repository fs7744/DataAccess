using Npgsql;
using System.Data.Common;
using VIC.DataAccess.Core;

namespace VIC.DataAccess.PostgreSQL.Core
{
    public class PostgreSQLDataCommand : SqlDataCommand
    {
        public PostgreSQLDataCommand(DbSql sql) : base(sql)
        {
        }

        protected override DbConnection CreateConnection(string connectionString)
        {
            return new NpgsqlConnection(connectionString);
        }
    }
}
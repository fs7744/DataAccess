using Microsoft.Data.Sqlite;
using System.Data.Common;
using VIC.DataAccess.Core;

namespace VIC.DataAccess.Sqlite.Core
{
    public class SqliteDataCommand : SqlDataCommand
    {
        public SqliteDataCommand(DbSql sql) : base(sql)
        {
        }

        protected override DbConnection CreateConnection(string connectionString)
        {
            return new SqliteConnection(connectionString);
        }
    }
}
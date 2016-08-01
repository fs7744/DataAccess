using Pomelo.Data.MySql;
using System.Data.Common;
using VIC.DataAccess.Core;

namespace VIC.DataAccess.MySql.Core
{
    public class MySqlDataCommand : SqlDataCommand
    {
        public MySqlDataCommand(DbSql sql) : base(sql)
        {
        }

        protected override DbConnection CreateConnection(string connectionString)
        {
            return new MySqlConnection(connectionString);
        }
    }
}
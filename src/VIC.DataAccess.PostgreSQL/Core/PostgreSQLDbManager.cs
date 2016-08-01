using VIC.DataAccess.Abstratiion;
using VIC.DataAccess.Core;

namespace VIC.DataAccess.PostgreSQL.Core
{
    public class PostgreSQLDbManager : SqlDbManager
    {
        public PostgreSQLDbManager(DbConfig config) : base(config)
        {
        }

        protected override IDataCommand CreateCommand(DbSql sql)
        {
            return new PostgreSQLDataCommand(sql);
        }
    }
}
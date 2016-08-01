using VIC.DataAccess.Abstratiion;
using VIC.DataAccess.Core;

namespace VIC.DataAccess.Sqlite.Core
{
    public class SqliteDbManager : SqlDbManager
    {
        public SqliteDbManager(DbConfig config) : base(config)
        {
        }

        protected override IDataCommand CreateCommand(DbSql sql)
        {
            return new SqliteDataCommand(sql);
        }
    }
}
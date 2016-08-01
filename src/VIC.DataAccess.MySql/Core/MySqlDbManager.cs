using VIC.DataAccess.Abstratiion;
using VIC.DataAccess.Core;

namespace VIC.DataAccess.MySql.Core
{
    public class MySqlDbManager : SqlDbManager
    {
        public MySqlDbManager(DbConfig config) : base(config)
        {
        }

        protected override IDataCommand CreateCommand(DbSql sql)
        {
            return new MySqlDataCommand(sql);
        }
    }
}
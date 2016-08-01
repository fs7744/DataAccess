using VIC.DataAccess.Abstratiion;
using VIC.DataAccess.Core;

namespace VIC.DataAccess.MSSql.Core
{
    public class MSSqlDbManager : SqlDbManager
    {
        public MSSqlDbManager(DbConfig config) : base(config)
        {
        }

        protected override IDataCommand CreateCommand(DbSql sql)
        {
            return new MSSqlDataCommand(sql);
        }
    }
}
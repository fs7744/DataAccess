using System.Collections.Generic;
using VIC.DataAccess.Abstraction;

namespace VIC.DataAccess.Config
{
    public abstract class DbManager : IDbManager
    {
        public Dictionary<string, DbSql> SqlConfigs { get; private set; }

        public DbManager(DbConfig config)
        {
            foreach (var item in config.SqlConfigs.Values)
            {
                var connectionString = string.Empty;
                config.ConnectionStrings.TryGetValue(item.ConnectionName, out connectionString);
                item.ConnectionString = connectionString;
            }
            SqlConfigs = config.SqlConfigs;
        }

        public IDataCommand GetCommand(string commandName)
        {
            DbSql sql = null;
            return SqlConfigs.TryGetValue(commandName, out sql) ? CreateCommand(sql) : null;
        }

        protected abstract IDataCommand CreateCommand(DbSql sql);
    }
}
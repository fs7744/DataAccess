using System;
using System.Collections.Generic;
using VIC.DataAccess.Abstratiion;
using VIC.DataAccess.Sqlserver.Core;

namespace VIC.DataAccess.Core
{
    public class SqlDbManager : IDbManager
    {
        public Dictionary<string, DbSql> SqlConfigs { get; private set; }

        public SqlDbManager(DbConfig config) 
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
            return SqlConfigs.TryGetValue(commandName, out sql) ? new SqlDataCommand(sql) : null;
        }
    }
}
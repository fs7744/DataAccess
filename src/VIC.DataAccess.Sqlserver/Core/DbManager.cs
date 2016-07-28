using System;
using System.Collections.Generic;
using VIC.DataAccess.Abstratiion;

namespace VIC.DataAccess.Core
{
    public class DbManager : IDbManager
    {
        public IDictionary<string, DbSql> SqlConfigs { get; private set; }

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
            throw new NotImplementedException();
        }
    }
}
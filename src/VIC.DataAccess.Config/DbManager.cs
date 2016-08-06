using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using VIC.DataAccess.Abstraction;
using VIC.ObjectConfig;
using VIC.ObjectConfig.Abstraction;

namespace VIC.DataAccess.Config
{
    public class DbManager : IDbManager
    {
        public const string DbConfigKey = "DB";
        private IConfig _Config;
        private IServiceProvider _ServiceProvider;

        public Dictionary<string, DbSql> SqlConfigs
        {
            get
            {
                return _Config.Get<DbConfig>(DbConfigKey)?.Sqls;
            }
        }

        public DbManager(IConfig config, IServiceProvider serviceProvider)
        {
            _Config = config;
            _ServiceProvider = serviceProvider;
        }

        public IDataCommand GetCommand(string commandName)
        {
            DbSql sql = null;
            return SqlConfigs.TryGetValue(commandName, out sql) ? CreateCommand(sql) : null;
        }

        protected IDataCommand CreateCommand(DbSql sql)
        {
            var command = _ServiceProvider.GetService<IDataCommand>();
            command.ConnectionString = sql.ConnectionString;
            command.Text = sql.Text;
            command.Type = sql.Type;
            command.Timeout = sql.Timeout;
            sql.PreParameters?.ForEach(i => command.AddPreParam(i));
            return command;
        }
    }
}
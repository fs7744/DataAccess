using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Threading.Tasks;
using VIC.DataAccess.Abstratiion;

namespace VIC.DataAccess.Core
{
    public class SqlDbManager : IDbManager
    {
        public Dictionary<string, DbSql> SqlConfigs { get; private set; }

        public SqlDbManager(DbConfig config)
        {
            var paramConverters = new ConcurrentDictionary<Type, Func<dynamic, List<SqlParameter>>>();
            var readerConverters = new ConcurrentDictionary<Type, Func<DbDataReader, dynamic>>();
            var scalarConverters = new Dictionary<Type, Func<DbDataReader, Task<dynamic>>>();
            scalarConverters.Add(typeof(long), async i => await i.ReadAsync() ? i.GetInt64(0) : default(long));
            scalarConverters.Add(typeof(bool), async i => await i.ReadAsync() ? i.GetBoolean(0) : default(bool));
            scalarConverters.Add(typeof(string), async i => await i.ReadAsync() ? i.GetString(0) : default(string));
            scalarConverters.Add(typeof(DateTime), async i => await i.ReadAsync() ? i.GetDateTime(0) : default(DateTime));
            scalarConverters.Add(typeof(decimal), async i => await i.ReadAsync() ? i.GetDecimal(0) : default(decimal));
            scalarConverters.Add(typeof(double), async i => await i.ReadAsync() ? i.GetDouble(0) : default(double));
            scalarConverters.Add(typeof(int), async i => await i.ReadAsync() ? i.GetInt32(0) : default(int));
            scalarConverters.Add(typeof(float), async i => await i.ReadAsync() ? i.GetFloat(0) : default(float));
            scalarConverters.Add(typeof(short), async i => await i.ReadAsync() ? i.GetInt16(0) : default(short));
            scalarConverters.Add(typeof(byte), async i => await i.ReadAsync() ? i.GetByte(0) : default(byte));
            scalarConverters.Add(typeof(Guid), async i => await i.ReadAsync() ? i.GetGuid(0) : default(Guid));

            foreach (var item in config.SqlConfigs.Values)
            {
                var connectionString = string.Empty;
                config.ConnectionStrings.TryGetValue(item.ConnectionName, out connectionString);
                item.ConnectionString = connectionString;
                item.ScalarConverters = scalarConverters;
                item.ParamConverters = paramConverters;
                item.ReaderConverters = readerConverters;
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
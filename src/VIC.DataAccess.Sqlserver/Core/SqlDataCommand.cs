using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using VIC.DataAccess.Abstratiion;

namespace VIC.DataAccess.Core
{
    public class SqlDataCommand : IDataCommand
    {
        private DbSql _Sql;

        public SqlDataCommand(DbSql sql)
        {
            _Sql = sql;
        }

        #region IDataCommand

        public Task<DbDataReader> ExecuteDataReaderAsync(dynamic parameter = null)
        {
            return ExecuteDataReaderAsync(CommandBehavior.Default);
        }

        public async Task<T> ExecuteEntityAsync<T>(dynamic paramter = null)
        {
            using (DbDataReader reader = await ExecuteDataReaderAsync(CommandBehavior.SingleRow, paramter))
            {
                return await reader.ReadAsync() ? _Sql.GetReaderConverter(typeof(T))(reader) : default(T);
            }
        }

        public async Task<List<T>> ExecuteEntityListAsync<T>(dynamic paramter = null)
        {
            using (DbDataReader reader = await ExecuteDataReaderAsync(CommandBehavior.SingleResult, paramter))
            {
                var list = new List<T>();
                var func = _Sql.GetReaderConverter(typeof(T));
                while (await reader.ReadAsync())
                {
                    list.Add(func(reader));
                }
                return list;
            }
        }

        public async Task<IMultipleReader> ExecuteMultipleAsync(dynamic parameter = null)
        {
            DbDataReader reader = await ExecuteDataReaderAsync(CommandBehavior.Default);
            return new MultipleReader(reader, _Sql);
        }

        public async Task<T> ExecuteScalarAsync<T>(dynamic paramter = null)
        {
            using (DbDataReader reader = await ExecuteDataReaderAsync(CommandBehavior.SingleRow, paramter))
            {
                return await reader.ReadAsync() ? await _Sql.GetScalarConverter(typeof(T))(reader) : default(T);
            }
        }

        public async Task<int> ExecuteNonQueryAsync(dynamic parameter = null)
        {
            SqlCommand command = CreateCommand(parameter);
            await command.Connection.OpenAsync();
            return await command.ExecuteNonQueryAsync();
        }

        public async void ExecuteBulkCopyAsync<T>(List<T> data) where T : class, new()
        {
            var conn = new SqlConnection(_Sql.ConnectionString);
            await conn.OpenAsync();
            using (var sqlBulkCopy = new SqlBulkCopy(conn))
            {
                sqlBulkCopy.DestinationTableName = _Sql.Sql;
                var reader = new BulkCopyDataReader<T>(data);
                reader.ColumnMappings.ForEach(i => sqlBulkCopy.ColumnMappings.Add(i));
                await sqlBulkCopy.WriteToServerAsync(reader);
            }
        }

        #endregion IDataCommand

        #region private

        private async Task<DbDataReader> ExecuteDataReaderAsync(CommandBehavior behavior, dynamic parameter = null)
        {
            var command = CreateCommand(parameter);
            await command.Connection.OpenAsync();
            return command.ExecuteReaderAsync(CommandBehavior.CloseConnection | behavior);
        }

        private SqlCommand CreateCommand(dynamic parameter = null)
        {
            var conn = new SqlConnection(_Sql.ConnectionString);
            var command = conn.CreateCommand();
            command.CommandText = _Sql.Sql;
            command.CommandType = _Sql.Type;
            if (parameter != null)
            {
                List<SqlParameter> paramList = _Sql.GetParamConverter(parameter.GetType())(parameter);
                SetSpecialParameters(paramList);
                command.Parameters.AddRange(paramList.ToArray());
            }
            return command;
        }

        private void SetSpecialParameters(List<SqlParameter> paramList)
        {
            var sps = _Sql.SpecialParameters;
            if (sps != null && sps.Count > 0)
            {
                paramList.Where(i => sps.ContainsKey(i.ParameterName)).ToList()
                    .ForEach(i =>
                    {
                        var sp = sps[i.ParameterName];
                        i.DbType = sp.DbType;
                        i.Size = sp.Size;
                        i.IsNullable = sp.IsNullable;
                        i.Direction = sp.Direction;
                    });
            }
        }

        #endregion private
    }
}
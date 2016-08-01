using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using VIC.DataAccess.Abstratiion;

namespace VIC.DataAccess.Core
{
    public abstract class SqlDataCommand : IDataCommand, IDisposable
    {
        protected DbSql _Sql;
        protected DbConnection _Conn;
        protected DbTransaction _Tran;

        public string CommandText { get; set; }

        public SqlDataCommand(DbSql sql)
        {
            _Sql = sql;
            _Conn = CreateConnection(_Sql.ConnectionString);
            CommandText = _Sql.Sql;
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
            DbCommand command = CreateCommand(parameter);
            await command.Connection.OpenAsync();
            return await command.ExecuteNonQueryAsync();
        }

        public IDbTransaction BeginTransaction(IsolationLevel level)
        {
            if (_Tran == null)
            {
                _Tran = _Conn.BeginTransaction(level);
            }
            return _Tran;
        }

        #endregion IDataCommand

        protected abstract DbConnection CreateConnection(string connectionString);

        #region private

        private async Task<DbDataReader> ExecuteDataReaderAsync(CommandBehavior behavior, dynamic parameter = null)
        {
            var command = CreateCommand(parameter);
            await command.Connection.OpenAsync();
            return command.ExecuteReaderAsync(CommandBehavior.CloseConnection | behavior);
        }

        private DbCommand CreateCommand(dynamic parameter = null)
        {
            var command = _Conn.CreateCommand();
            command.CommandText = CommandText;
            command.CommandType = _Sql.Type;
            if (_Tran != null)
            {
                command.Transaction = _Tran;
            }
            if (parameter != null)
            {
                List<DbParameter> paramList = _Sql.GetParamConverter(parameter.GetType())(parameter);
                SetSpecialParameters(paramList);
                command.Parameters.AddRange(paramList.ToArray());
            }
            return command;
        }

        private void SetSpecialParameters(List<DbParameter> paramList)
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

        #region IDisposable Support

        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _Tran.Dispose();
                    _Conn.Dispose();
                }

                _Tran = null;
                _Conn = null;
                _Sql = null;
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion IDisposable Support
    }
}
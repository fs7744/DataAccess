using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VIC.DataAccess.Abstraction;
using VIC.DataAccess.Abstraction.Converter;

namespace VIC.DataAccess.Core
{
    public abstract class DataCommand : IDataCommand, IDisposable
    {
        protected IParamConverter _PC;
        protected IScalarConverter _SC;
        protected IEntityConverter _EC;
        protected DbConnection _Conn;
        protected DbTransaction _Tran;

        private string _ConnectionString;

        public string ConnectionString
        {
            get { return _ConnectionString; }
            set
            {
                _ConnectionString = value;
                if (_Conn != null)
                {
                    _Conn.Close();
                    _Conn.Dispose();
                }
                _Conn = CreateConnection(value);
            }
        }

        public string Text { get; set; }

        public int Timeout { get; set; }

        public CommandType Type { get; set; }

        public DbParameterCollection PreParameters { get; } = new DbParameterCollection();

        public DataCommand(IParamConverter pc, IScalarConverter sc, IEntityConverter ec)
        {
            _PC = pc;
            _SC = sc;
            _EC = ec;
        }

        #region IDataCommand

        public Task<DbDataReader> ExecuteDataReaderAsync(dynamic parameter = null, CommandBehavior behavior = CommandBehavior.Default)
        {
            return ExecuteDataReaderAsync(CancellationToken.None, parameter, behavior);
        }

        public Task<DbDataReader> ExecuteDataReaderAsync(CancellationToken cancellationToken, dynamic parameter = null, CommandBehavior behavior = CommandBehavior.Default)
        {
            return GetDataReaderAsync(behavior, cancellationToken, parameter);
        }

        public Task<T> ExecuteEntityAsync<T>(dynamic parameter = null)
        {
            return ExecuteEntityAsync<T>(CancellationToken.None, parameter);
        }

        public async Task<T> ExecuteEntityAsync<T>(CancellationToken cancellationToken, dynamic paramter = null)
        {
            using (DbDataReader reader = await GetDataReaderAsync(CommandBehavior.SingleRow, cancellationToken, paramter))
            {
                return await reader.ReadAsync(cancellationToken) ? _EC.Convert<T>(reader) : default(T);
            }
        }

        public Task<List<T>> ExecuteEntityListAsync<T>(dynamic paramter = null)
        {
            return ExecuteEntityListAsync<T>(CancellationToken.None, paramter);
        }

        public async Task<List<T>> ExecuteEntityListAsync<T>(CancellationToken cancellationToken, dynamic paramter = null)
        {
            using (DbDataReader reader = await GetDataReaderAsync(CommandBehavior.SingleResult, cancellationToken, paramter))
            {
                var list = new List<T>();
                while (await reader.ReadAsync(cancellationToken))
                {
                    list.Add(_EC.Convert<T>(reader));
                }

                return list;
            }
        }

        public Task<IMultipleReader> ExecuteMultipleAsync(dynamic parameter = null)
        {
            return ExecuteMultipleAsync(CancellationToken.None, parameter);
        }

        public async Task<IMultipleReader> ExecuteMultipleAsync(CancellationToken cancellationToken, dynamic parameter = null)
        {
            DbDataReader reader = await GetDataReaderAsync(CommandBehavior.Default, cancellationToken, parameter);
            return new MultipleReader(reader, _SC, _EC);
        }

        public Task<T> ExecuteScalarAsync<T>(dynamic paramter = null)
        {
            return ExecuteScalarAsync<T>(CancellationToken.None, paramter);
        }

        public async Task<T> ExecuteScalarAsync<T>(CancellationToken cancellationToken, dynamic paramter = null)
        {
            using (DbDataReader reader = await GetDataReaderAsync(CommandBehavior.SingleRow, cancellationToken, paramter))
            {
                return await reader.ReadAsync(cancellationToken) ? _SC.Convert<T>(reader) : default(T);
            }
        }

        public Task<int> ExecuteNonQueryAsync(dynamic parameter = null)
        {
            return ExecuteNonQueryAsync(CancellationToken.None, parameter);
        }

        public async Task<int> ExecuteNonQueryAsync(CancellationToken cancellationToken, dynamic parameter = null)
        {
            DbCommand command = CreateCommand(parameter);
            await OpenAsync(cancellationToken);
            return await command.ExecuteNonQueryAsync(cancellationToken);
        }

        public IDbTransaction BeginTransaction(IsolationLevel level)
        {
            if (_Tran == null)
            {
                _Tran = _Conn.BeginTransaction(level);
            }
            return _Tran;
        }

        public void AddPreParam(DataParameter parameter)
        {
            PreParameters.Add(parameter);
        }

        #endregion IDataCommand

        protected abstract DbConnection CreateConnection(string connectionString);

        #region private

        private async Task<DbDataReader> GetDataReaderAsync(CommandBehavior behavior, CancellationToken cancellationToken, dynamic parameter = null)
        {
            DbCommand command = CreateCommand(parameter);
            await OpenAsync(cancellationToken);
            return await command.ExecuteReaderAsync(CommandBehavior.CloseConnection | behavior, cancellationToken);
        }

        private Task OpenAsync(CancellationToken cancellationToken)
        {
            return _Conn.OpenAsync(cancellationToken);
        }

        private DbCommand CreateCommand(dynamic parameter = null)
        {
            if (_Conn == null)
                throw new ArgumentNullException(nameof(ConnectionString), "can't be null");

            var command = _Conn.CreateCommand();
            command.CommandText = Text;
            command.CommandType = Type;
            command.CommandTimeout = Timeout;
            if (_Tran != null)
            {
                command.Transaction = _Tran;
            }
            if (parameter != null)
            {
                List<DbParameter> paramList = _PC.Convert(parameter.GetType(), parameter);
                SetSpecialParameters(paramList);
                command.Parameters.AddRange(paramList.ToArray());
            }
            return command;
        }

        private void SetSpecialParameters(List<DbParameter> paramList)
        {
            var sps = PreParameters;
            if (sps.Count == 0) return;
            foreach (var sp in sps.GetParams().Where(j => j != null))
            {
                var i = paramList.FirstOrDefault(j => j.ParameterName == sp.ParameterName);
                if (i != null)
                {
                    i.DbType = sp.DbType;
                    i.Size = sp.Size;
                    i.IsNullable = sp.IsNullable;
                    i.Direction = sp.Direction;
                    i.Precision = sp.Precision;
                    i.Scale = sp.Scale;
                }
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
                    if (_Tran != null)
                    {
                        _Tran.Dispose();
                    }

                    if (_Conn != null)
                    {
                        _Conn.Close();
                        _Conn.Dispose();
                    }
                }

                _PC = null;
                _SC = null;
                _EC = null;
                _Tran = null;
                _Conn = null;
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
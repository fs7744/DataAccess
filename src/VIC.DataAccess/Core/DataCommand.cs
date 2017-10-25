using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
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

        public CommandType Type { get; set; } = CommandType.Text;

        public DataParameterCollection PreParameters { get; } = new DataParameterCollection();

        public DataCommand(IParamConverter pc, IScalarConverter sc, IEntityConverter ec)
        {
            _PC = pc;
            _SC = sc;
            _EC = ec;
        }

        public IDbTransaction BeginTransaction(IsolationLevel level)
        {
            if (_Tran == null)
            {
                _Tran = _Conn.BeginTransaction(level);
            }
            return _Tran;
        }

        #region AsyncIDataCommand

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
                return await reader.ReadAsync(cancellationToken) ? _EC.GetConverter<T>(reader)(reader) : default(T);
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
                var converter = _EC.GetConverter<T>(reader);
                while (await reader.ReadAsync(cancellationToken))
                {
                    list.Add(converter(reader));
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

        public Task<int> ExecuteNonQueryAsync<T>(T parameter = null) where T : class
        {
            return ExecuteNonQueryAsync(CancellationToken.None, parameter);
        }

        public Task<int> ExecuteNonQueryAsync()
        {
            DbParameter p = null;
            return ExecuteNonQueryAsync(p);
        }

        public async Task<int> ExecuteNonQueryAsync<T>(CancellationToken cancellationToken, T parameter = null) where T : class
        {
            DbCommand command = CreateCommand(parameter);
            await OpenAsync(cancellationToken);
            return await command.ExecuteNonQueryAsync(cancellationToken);
        }

        public Task<int> ExecuteNonQuerysAsync<T>(List<T> parameters = null, int batchSize = 200) where T : class
        {
            return ExecuteNonQuerysAsync(CancellationToken.None, parameters, batchSize);
        }

        public async Task<int> ExecuteNonQuerysAsync<T>(CancellationToken cancellationToken, List<T> parameters = null, int batchSize = 200) where T : class
        {
            DbCommand command = CreateCommandByParam(i => { });
            await OpenAsync(cancellationToken);
            var total = 0;
            foreach (var item in parameters.Page(batchSize))
            {
                command.Parameters.Clear();
                SetParams(command, item.ToList());
                total += await command.ExecuteNonQueryAsync(cancellationToken);
            }
            return total;
        }

        public virtual Task ExecuteBulkCopyAsync<T>(List<T> data) where T : class
        {
            throw new NotImplementedException();
        }

        #endregion AsyncIDataCommand

        #region SyncIDataCommand

        public DbDataReader ExecuteDataReader(dynamic parameter = null, CommandBehavior behavior = CommandBehavior.Default)
        {
            return GetDataReader(behavior, parameter);
        }

        public T ExecuteEntity<T>(dynamic paramter = null)
        {
            using (DbDataReader reader = GetDataReader(CommandBehavior.SingleRow, paramter))
            {
                return reader.Read() ? _EC.GetConverter<T>(reader)(reader) : default(T);
            }
        }

        public List<T> ExecuteEntityList<T>(dynamic paramter = null)
        {
            using (DbDataReader reader = GetDataReader(CommandBehavior.SingleResult, paramter))
            {
                var list = new List<T>();
                var converter = _EC.GetConverter<T>(reader);
                while (reader.Read())
                {
                    list.Add(converter(reader));
                }

                return list;
            }
        }

        public IMultipleReader ExecuteMultiple(dynamic parameter = null)
        {
            DbDataReader reader = GetDataReader(CommandBehavior.Default, parameter);
            return new MultipleReader(reader, _SC, _EC);
        }

        public T ExecuteScalar<T>(dynamic paramter = null)
        {
            using (DbDataReader reader = GetDataReader(CommandBehavior.SingleRow, paramter))
            {
                return reader.Read() ? _SC.Convert<T>(reader) : default(T);
            }
        }

        public int ExecuteNonQuery(dynamic parameter = null)
        {
            DbCommand command = CreateCommand(parameter);
            Open();
            return command.ExecuteNonQuery();
        }

        #endregion SyncIDataCommand

        protected abstract DbConnection CreateConnection(string connectionString);

        #region private

        private async Task<DbDataReader> GetDataReaderAsync(CommandBehavior behavior, CancellationToken cancellationToken, dynamic parameter = null)
        {
            DbCommand command = CreateCommand(parameter);
            await OpenAsync(cancellationToken);
            return await command.ExecuteReaderAsync(CommandBehavior.CloseConnection | behavior, cancellationToken);
        }

        private DbDataReader GetDataReader(CommandBehavior behavior, dynamic parameter = null)
        {
            DbCommand command = CreateCommand(parameter);
            Open();
            return command.ExecuteReader(CommandBehavior.CloseConnection | behavior);
        }

        private void Open()
        {
            if (_Conn.State == ConnectionState.Closed || _Conn.State == ConnectionState.Broken)
            {
                _Conn.Open();
            }
        }

        private Task OpenAsync(CancellationToken cancellationToken)
        {
            return _Conn.State == ConnectionState.Closed || _Conn.State == ConnectionState.Broken
                ? _Conn.OpenAsync(cancellationToken)
                : Task.CompletedTask;
        }

        private DbCommand CreateCommandByParam(Action<DbCommand> setParams)
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
            setParams(command);
            return command;
        }

        private DbCommand CreateCommand(dynamic parameter = null)
        {
            return CreateCommandByParam(command =>
            {
                if (parameter != null)
                {
                    List<DbParameter> paramList = _PC.Convert(parameter.GetType(), parameter);
                    SetSpecialParameters(paramList);
                    command.Parameters.AddRange(paramList.ToArray());
                }
            });
        }

        private void SetParams<T>(DbCommand command, List<T> parameters = null) where T : class
        {
            if (parameters == null || parameters.Count <= 0) return;
            var sb = new StringBuilder();
            var paramLists = parameters.Select(parameter =>
            {
                List<DbParameter> paramList = _PC.Convert(parameter.GetType(), parameter);
                SetSpecialParameters(paramList);
                return paramList;
            }).ToArray();
            var templateSB = new StringBuilder(Text);
            var first = paramLists[0];
            for (int i = 0; i < first.Count; i++)
            {
                var p = first[i];
                templateSB.Replace(p.ParameterName, $"{p.ParameterName}{{0}}");
            }
            templateSB.Append(";");
            var template = templateSB.ToString();
            for (int i = 0; i < paramLists.Length; i++)
            {
                var p = paramLists[i];
                sb.AppendFormat(template, i);
                p.ForEach(j => j.ParameterName = $"{j.ParameterName}{i}");
                command.Parameters.AddRange(p.ToArray());
            }
            command.CommandText = sb.ToString();
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

        public int ExecuteNonQuery()
        {
            DbParameter p = null;
            return ExecuteNonQuery(p);
        }

        public int ExecuteNonQuery<T>(T parameter = null) where T : class
        {
            DbCommand command = CreateCommand(parameter);
            Open();
            return command.ExecuteNonQuery();
        }

        public int ExecuteNonQuerys<T>(List<T> parameters, int batchSize = 200) where T : class
        {
            DbCommand command = CreateCommandByParam(i => { });
            Open();
            return parameters.Page(batchSize)
                .Select(i =>
                {
                    command.Parameters.Clear();
                    SetParams(command, i.ToList());
                    return command.ExecuteNonQuery();
                })
                .Sum();
        }

        public virtual void ExecuteBulkCopy<T>(List<T> data) where T : class
        {
            throw new NotImplementedException();
        }

        #endregion IDisposable Support
    }
}
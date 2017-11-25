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

        public int Timeout { get; set; } = 30;

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
            try
            {
                using (DbDataReader reader = await GetDataReaderAsync(CommandBehavior.SingleRow, cancellationToken, paramter))
                {
                    return await reader.ReadAsync(cancellationToken) ? _EC.GetConverter<T>(reader)(reader) : default(T);
                }
            }
            finally
            {
                Close();
            }
        }

        public Task<List<T>> ExecuteEntityListAsync<T>(dynamic paramter = null)
        {
            return ExecuteEntityListAsync<T>(CancellationToken.None, paramter);
        }

        public async Task<List<T>> ExecuteEntityListAsync<T>(CancellationToken cancellationToken, dynamic paramter = null)
        {
            try
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
            finally
            {
                Close();
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
            try
            {
                using (DbDataReader reader = await GetDataReaderAsync(CommandBehavior.SingleRow, cancellationToken, paramter))
                {
                    return await reader.ReadAsync(cancellationToken) ? _SC.Convert<T>(reader) : default(T);
                }
            }
            finally
            {
                Close();
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
            try
            {
                DbCommand command = CreateCommand(parameter);
                await OpenAsync(cancellationToken);
                return await command.ExecuteNonQueryAsync(cancellationToken);
            }
            finally
            {
                Close();
            }
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

        public Task<List<T>> ExecuteScalarListAsync<T>(dynamic paramter = null)
        {
            return ExecuteScalarListAsync<T>(CancellationToken.None);
        }

        public async Task<List<T>> ExecuteScalarListAsync<T>(CancellationToken cancellationToken, dynamic paramter = null)
        {
            try
            {
                using (DbDataReader reader = await GetDataReaderAsync(CommandBehavior.SingleResult, cancellationToken, paramter))
                {
                    var list = new List<T>();
                    while (await reader.ReadAsync(cancellationToken))
                    {
                        list.Add(_SC.Convert<T>(reader));
                    }

                    return list;
                }
            }
            finally
            {
                Close();
            }
        }

        #endregion AsyncIDataCommand

        #region SyncIDataCommand

        public DbDataReader ExecuteDataReader(dynamic parameter = null, CommandBehavior behavior = CommandBehavior.Default)
        {
            return GetDataReader(behavior, parameter);
        }

        public T ExecuteEntity<T>(dynamic paramter = null)
        {
            try
            {
                using (DbDataReader reader = GetDataReader(CommandBehavior.SingleRow, paramter))
                {
                    return reader.Read() ? _EC.GetConverter<T>(reader)(reader) : default(T);
                }
            }
            finally
            {
                Close();
            }
        }

        public List<T> ExecuteEntityList<T>(dynamic paramter = null)
        {
            try
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
            finally
            {
                Close();
            }
        }

        public IMultipleReader ExecuteMultiple(dynamic parameter = null)
        {
            DbDataReader reader = GetDataReader(CommandBehavior.Default, parameter);
            return new MultipleReader(reader, _SC, _EC);
        }

        public T ExecuteScalar<T>(dynamic paramter = null)
        {
            try
            {
                using (DbDataReader reader = GetDataReader(CommandBehavior.SingleRow, paramter))
                {
                    return reader.Read() ? _SC.Convert<T>(reader) : default(T);
                }
            }
            finally
            {
                Close();
            }
        }

        public int ExecuteNonQuery(dynamic parameter = null)
        {
            DbCommand command = CreateCommand(parameter);
            Open();
            try
            {
                return command.ExecuteNonQuery();
            }
            finally
            {
                Close();
            }
        }

        public List<T> ExecuteScalarList<T>(dynamic paramter = null)
        {
            try
            {
                using (DbDataReader reader = GetDataReader(CommandBehavior.SingleResult, paramter))
                {
                    var list = new List<T>();
                    while (reader.Read())
                    {
                        list.Add(_SC.Convert<T>(reader));
                    }

                    return list;
                }
            }
            finally
            {
                Close();
            }
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

        public void Close()
        {
            if (_Conn.State != ConnectionState.Closed)
            {
                _Conn.Close();
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
                    var converter = _PC.GetConverter(parameter.GetType());
                    List<DbParameter> paramList = converter.Item1(parameter);
                    SetSpecialParameters(paramList);
                    command.Parameters.AddRange(paramList.ToArray());
                    IGrouping<string, DbParameter>[] paramLists = converter.Item2(parameter);
                    if (paramLists.Length == 0) return;
                    SetInParams(command, paramLists);
                }
            });
        }

        private void SetInParams(DbCommand command, IGrouping<string, DbParameter>[] paramLists)
        {
            var templateSB = new StringBuilder(Text);
            foreach (var item in paramLists)
            {
                var sb = new StringBuilder("(");
                foreach (var p in item)
                {
                    sb.Append(p.ParameterName);
                    command.Parameters.Add(p);
                    sb.Append(",");
                }
                sb.Remove(sb.Length - 1, 1);
                sb.Append(")");
                templateSB.Replace(item.Key, sb.ToString());
            }
            command.CommandText = templateSB.ToString();
        }

        private void SetParams<T>(DbCommand command, List<T> parameters = null) where T : class
        {
            if (parameters == null || parameters.Count <= 0) return;
            var sb = new StringBuilder();
            var paramLists = parameters.Select(parameter =>
            {
                var converter = _PC.GetConverter(parameter.GetType());
                List<DbParameter> paramList = converter.Item1(parameter);
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
            try
            {
                return command.ExecuteNonQuery();
            }
            finally
            {
                Close();
            }
        }

        public int ExecuteNonQuerys<T>(List<T> parameters, int batchSize = 200) where T : class
        {
            DbCommand command = CreateCommandByParam(i => { });
            Open();
            try
            {
                return parameters.Page(batchSize)
                    .Select(i =>
                    {
                        command.Parameters.Clear();
                        SetParams(command, i.ToList());
                        return command.ExecuteNonQuery();
                    })
                    .Sum();
            }
            finally
            {
                Close();
            }
        }

        public virtual void ExecuteBulkCopy<T>(List<T> data) where T : class
        {
            throw new NotImplementedException();
        }

        #endregion IDisposable Support
    }
}
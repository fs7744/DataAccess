using System.Collections.Generic;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using VIC.DataAccess.Abstraction;
using VIC.DataAccess.Abstraction.Converter;

namespace VIC.DataAccess.Core
{
    public class MultipleReader : IMultipleReader
    {
        protected DbDataReader _Reader;
        protected IScalarConverter _SC;
        protected IEntityConverter _EC;

        public MultipleReader(DbDataReader reader, IScalarConverter sc, IEntityConverter ec)
        {
            _SC = sc;
            _EC = ec;
            _Reader = reader;
        }

        public async Task<List<T>> ExecuteEntityListAsync<T>(CancellationToken cancellationToken, dynamic paramter = null)
        {
            var list = new List<T>();
            if (_Reader.HasRows)
            {
                var converter = _EC.GetConverter<T>(_Reader);
                while (await _Reader.ReadAsync(cancellationToken))
                {
                    list.Add(converter(_Reader));
                }
                await _Reader.NextResultAsync(cancellationToken);
            }
            return list;
        }

        public Task<List<T>> ExecuteEntityListAsync<T>()
        {
            return ExecuteEntityListAsync<T>(CancellationToken.None);
        }

        public Task<T> ExecuteEntityAsync<T>()
        {
            return ExecuteEntityAsync<T>(CancellationToken.None);
        }

        public async Task<T> ExecuteEntityAsync<T>(CancellationToken cancellationToken)
        {
            var result = default(T);
            if (_Reader.HasRows)
            {
                if (await _Reader.ReadAsync(cancellationToken))
                {
                    result = _EC.GetConverter<T>(_Reader)(_Reader);
                }
                await _Reader.NextResultAsync(cancellationToken);
            }
            return result;
        }

        public Task<T> ExecuteScalarAsync<T>()
        {
            return ExecuteScalarAsync<T>(CancellationToken.None);
        }

        public async Task<T> ExecuteScalarAsync<T>(CancellationToken cancellationToken)
        {
            var result = default(T);
            if (_Reader.HasRows)
            {
                if (await _Reader.ReadAsync(cancellationToken))
                {
                    result = _SC.Convert<T>(_Reader);
                }
                await _Reader.NextResultAsync(cancellationToken);
            }
            return result;
        }

        #region IDisposable Support

        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _Reader.Close();
                    _Reader.Dispose();
                }

                _Reader = null;
                _EC = null;
                _SC = null;
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        public List<T> ExecuteEntityList<T>()
        {
            var list = new List<T>();
            if (_Reader.HasRows)
            {
                var converter = _EC.GetConverter<T>(_Reader);
                while (_Reader.Read())
                {
                    list.Add(converter(_Reader));
                }
                _Reader.NextResult();
            }
            return list;
        }

        public T ExecuteEntity<T>()
        {
            var result = default(T);
            if (_Reader.HasRows)
            {
                if (_Reader.Read())
                {
                    result = _EC.GetConverter<T>(_Reader)(_Reader);
                }
                 _Reader.NextResult();
            }
            return result;
        }

        public T ExecuteScalar<T>()
        {
            var result = default(T);
            if (_Reader.HasRows)
            {
                if (_Reader.Read())
                {
                    result = _SC.Convert<T>(_Reader);
                }
                _Reader.NextResult();
            }
            return result;
        }

        #endregion IDisposable Support
    }
}
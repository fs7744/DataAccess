using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using VIC.DataAccess.Abstraction;

namespace VIC.DataAccess.Core
{
    public class MultipleReader : IMultipleReader, IDisposable
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

        public async Task<T> ExecuteEntityAsync<T>()
        {
            var result = default(T);
            if (_Reader.HasRows)
            {
                if (await _Reader.ReadAsync())
                {
                    result = await _EC.Convert<T>(_Reader);
                }
                await _Reader.NextResultAsync();
            }
            return result;
        }

        public async Task<List<T>> ExecuteEntityListAsync<T>()
        {
            var list = new List<T>();
            if (_Reader.HasRows)
            {
                while (await _Reader.ReadAsync())
                {
                    list.Add(_EC.Convert<T>(_Reader));
                }
                await _Reader.NextResultAsync();
            }
            return list;
        }

        public async Task<T> ExecuteScalarAsync<T>()
        {
            var result = default(T);
            if (_Reader.HasRows)
            {
                if (await _Reader.ReadAsync())
                {
                    result = _SC.Convert<T>(_Reader);
                }
                await _Reader.NextResultAsync();
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
                    _Reader.Dispose();
                }

                _Reader = null;
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
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using VIC.DataAccess.Abstratiion;

namespace VIC.DataAccess.Core
{
    public class MultipleReader : IMultipleReader, IDisposable
    {
        private DbSql _Sql;
        private DbDataReader _Reader;

        public MultipleReader(DbDataReader reader, DbSql sql)
        {
            _Sql = sql;
            _Reader = reader;
        }

        public async Task<T> ExecuteEntityAsync<T>()
        {
            var result = default(T);
            if (_Reader.HasRows)
            {
                if (await _Reader.ReadAsync())
                {
                    result = await _Sql.GetReaderConverter(typeof(T))(_Reader);
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
                var func = _Sql.GetReaderConverter(typeof(T));
                while (await _Reader.ReadAsync())
                {
                    list.Add(func(_Reader));
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
                    result = await _Sql.GetScalarConverter(typeof(T))(_Reader);
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
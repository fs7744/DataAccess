using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using VIC.DataAccess.Core;

namespace VIC.DataAccess.Abstraction
{
    public interface IDataCommand : IDisposable
    {
        string ConnectionString { get; set; }

        string Text { get; set; }

        int Timeout { get; set; }

        CommandType Type { get; set; }

        DataParameterCollection PreParameters { get; }

        Task<List<T>> ExecuteEntityListAsync<T>(dynamic paramter = null);

        Task<List<T>> ExecuteEntityListAsync<T>(CancellationToken cancellationToken, dynamic paramter = null);

        Task<T> ExecuteEntityAsync<T>(dynamic parameter = null);

        Task<T> ExecuteEntityAsync<T>(CancellationToken cancellationToken, dynamic paramter = null);

        Task<T> ExecuteScalarAsync<T>(dynamic parameter = null);

        Task<T> ExecuteScalarAsync<T>(CancellationToken cancellationToken, dynamic parameter = null);

        Task<DbDataReader> ExecuteDataReaderAsync(dynamic parameter = null, CommandBehavior behavior = CommandBehavior.Default);

        Task<DbDataReader> ExecuteDataReaderAsync(CancellationToken cancellationToken, dynamic parameter = null, CommandBehavior behavior = CommandBehavior.Default);

        Task<IMultipleReader> ExecuteMultipleAsync(dynamic parameter = null);

        Task<IMultipleReader> ExecuteMultipleAsync(CancellationToken cancellationToken, dynamic parameter = null);

        ValueTask<int> ExecuteNonQueryAsync();

        ValueTask<int> ExecuteNonQueryAsync<T>(T parameter = null) where T : class;

        ValueTask<int> ExecuteNonQueryAsync<T>(CancellationToken cancellationToken, T parameter = null) where T : class;

        ValueTask<int> ExecuteNonQuerysAsync<T>(List<T> parameters = null, int batchSize = 200) where T : class;

        ValueTask<int> ExecuteNonQuerysAsync<T>(CancellationToken cancellationToken, List<T> parameters = null, int batchSize = 200) where T : class;

        Task ExecuteBulkCopyAsync<T>(List<T> data) where T : class;

        Task<List<T>> ExecuteScalarListAsync<T>(CancellationToken cancellationToken, dynamic paramter = null);

        Task<List<T>> ExecuteScalarListAsync<T>(dynamic paramter = null);

        IDbTransaction BeginTransaction(IsolationLevel level = IsolationLevel.ReadUncommitted);

        DbDataReader ExecuteDataReader(dynamic parameter = null, CommandBehavior behavior = CommandBehavior.Default);

        T ExecuteEntity<T>(dynamic paramter = null);

        List<T> ExecuteEntityList<T>(dynamic paramter = null);

        IMultipleReader ExecuteMultiple(dynamic parameter = null);

        T ExecuteScalar<T>(dynamic paramter = null);

        int ExecuteNonQuery();

        int ExecuteNonQuery<T>(T parameter = null) where T : class;

        int ExecuteNonQuerys<T>(List<T> parameters = null, int batchSize = 1000) where T : class;

        void ExecuteBulkCopy<T>(List<T> data) where T : class;

        void Close();

        List<T> ExecuteScalarList<T>(dynamic paramter = null);
    }
}
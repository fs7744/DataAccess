using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;

namespace VIC.DataAccess.Abstratiion
{
    public interface IDataCommand
    {
        Task<List<T>> ExecuteEntityListAsync<T>(dynamic paramter = null);

        Task<T> ExecuteEntityAsync<T>(dynamic paramter = null);

        Task<T> ExecuteScalarAsync<T>(dynamic paramter = null);

        Task<DbDataReader> ExecuteDataReaderAsync(dynamic parameter = null);

        Task<IMultipleReader> ExecuteMultipleAsync(dynamic parameter = null);

        Task<int> ExecuteNonQueryAsync(dynamic parameter = null);

        void ExecuteBulkCopyAsync<T>(List<T> data) where T : class, new();
    }
}
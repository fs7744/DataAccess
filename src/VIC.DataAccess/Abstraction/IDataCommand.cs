using System.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data.Common;

namespace VIC.DataAccess.Abstratiion
{
    public interface IDataCommand
    {
        List<T> ExecuteEntityList<T>(dynamic paramter = null);

        T ExecuteEntity<T>(dynamic paramter = null);

        T ExecuteScalar<T>(dynamic paramter = null);

        Task<DbDataReader> ExecuteDataReaderAsync(dynamic parameter = null);

        IMultipleReader ExecuteMultiple(dynamic parameter = null);

        void ExecuteBulkCopy<T>(List<T> data) where T : class, new();
    }
}
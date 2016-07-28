using System.Data;
using System.Collections.Generic;

namespace VIC.DataAccess.Abstratiion
{
    public interface IDataCommand
    {
        List<T> ExecuteEntityList<T>(dynamic paramter = null);

        T ExecuteEntity<T>(dynamic paramter = null);

        T ExecuteScalar<T>(dynamic paramter = null);

        IDataReader ExecuteDataReader(dynamic parameter = null);

        IMultipleReader ExecuteMultiple(dynamic parameter = null);

        void ExecuteBulkCopy<T>(List<T> data) where T : class, new();
    }
}

using System.Collections.Generic;
using System.Data;

namespace VIC.DataAccess.Abstratiion
{
    public interface IMultipleReader
    {
        List<T> ExecuteEntityList<T>();

        T ExecuteEntity<T>();

        T ExecuteScalar<T>();

        IDataReader ExecuteDataReader();
    }
}
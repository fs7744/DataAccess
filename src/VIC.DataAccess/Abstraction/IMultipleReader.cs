using System.Collections.Generic;
using System.Threading.Tasks;

namespace VIC.DataAccess.Abstratiion
{
    public interface IMultipleReader
    {
        Task<List<T>> ExecuteEntityListAsync<T>();

        Task<T> ExecuteEntityAsync<T>();

        Task<T> ExecuteScalarAsync<T>();
    }
}
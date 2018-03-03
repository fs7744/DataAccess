using System.Collections.Generic;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using VIC.DataAccess.Core;

namespace VIC.DataAccess.Abstraction
{
    public interface IBatchUpdater
    {
        int ExecuteNonQuerys<T>(DataParameterCollection preParameters, DbConnection conn, DbTransaction tran, string text, int timeout, List<T> parameters, int batchSize) where T : class;

        ValueTask<int> ExecuteNonQuerysAsync<T>(DataParameterCollection preParameters, DbConnection conn, DbTransaction tran, string text, int timeout, List<T> parameters, int batchSize, CancellationToken cancellationToken) where T : class;
    }
}
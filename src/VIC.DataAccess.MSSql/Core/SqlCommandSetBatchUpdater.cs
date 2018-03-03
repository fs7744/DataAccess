using DotNetCore.Collections.Paginable;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VIC.DataAccess.Abstraction;
using VIC.DataAccess.Abstraction.Converter;
using VIC.DataAccess.Core;

namespace VIC.DataAccess.MSSql.Core
{
    public class SqlCommandSetBatchUpdater : IBatchUpdater
    {
        private readonly IParamConverter pc;

        public SqlCommandSetBatchUpdater(IParamConverter pc)
        {
            this.pc = pc;
        }

        public int ExecuteNonQuerys<T>(DataParameterCollection preParameters, DbConnection conn, DbTransaction tran, string text, int timeout, List<T> parameters, int batchSize) where T : class
        {
            var commandSet = new SqlClientSqlCommandSet
            {
                Connection = (SqlConnection)conn,
                CommandTimeout = timeout,
                Transaction = tran as SqlTransaction
            };
            var converter = pc.GetConverter(parameters.First().GetType());
            return parameters.ToPaginable(batchSize)
                .Select(i =>
                {
                    foreach (var parameter in i)
                    {
                        var sqlCommand = new SqlCommand();
                        List<DbParameter> paramList = converter.Item1(parameter);
                        preParameters.SetSpecialParameters(paramList);
                        sqlCommand.CommandText = text;
                        sqlCommand.CommandType = CommandType.Text;
                        foreach (var item in paramList)
                        {
                            sqlCommand.Parameters.Add(item);
                        }
                        commandSet.Append(sqlCommand);
                    }
                    return commandSet.ExecuteNonQuery();
                })
                .Sum();
        }

        public ValueTask<int> ExecuteNonQuerysAsync<T>(DataParameterCollection preParameters, DbConnection conn, DbTransaction tran, string text, int timeout, List<T> parameters, int batchSize, CancellationToken cancellationToken) where T : class
        {
            return new ValueTask<int>(ExecuteNonQuerys(preParameters, conn, tran, text, timeout, parameters, batchSize));
        }
    }
}
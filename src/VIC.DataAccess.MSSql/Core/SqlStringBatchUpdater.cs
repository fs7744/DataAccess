using DotNetCore.Collections.Paginable;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VIC.DataAccess.Abstraction;
using VIC.DataAccess.Abstraction.Converter;
using VIC.DataAccess.Core;

namespace VIC.DataAccess.MSSql.Core
{
    public class SqlStringBatchUpdater : IBatchUpdater
    {
        private readonly IParamConverter pc;

        public SqlStringBatchUpdater(IParamConverter pc)
        {
            this.pc = pc;
        }

        private void SetParams<T>(DataParameterCollection preParameters, DbCommand command, List<T> parameters = null) where T : class
        {
            if (parameters == null || parameters.Count <= 0) return;
            var sb = new StringBuilder();
            var paramLists = parameters.Select(parameter =>
            {
                var converter = pc.GetConverter(parameter.GetType());
                List<DbParameter> paramList = converter.Item1(parameter);
                preParameters.SetSpecialParameters(paramList);
                return paramList;
            }).ToArray();
            var templateSB = new StringBuilder(command.CommandText);
            var first = paramLists[0];
            for (int i = 0; i < first.Count; i++)
            {
                var p = first[i];
                templateSB.Replace(p.ParameterName, $"{p.ParameterName}{{0}}");
            }
            templateSB.Append(";");
            var template = templateSB.ToString();
            for (int i = 0; i < paramLists.Length; i++)
            {
                var p = paramLists[i];
                sb.AppendFormat(template, i);
                p.ForEach(j => j.ParameterName = $"{j.ParameterName}{i}");
                command.Parameters.AddRange(p.ToArray());
            }
            command.CommandText = sb.ToString();
        }

        public int ExecuteNonQuerys<T>(DataParameterCollection preParameters, DbConnection conn, DbTransaction tran, string text, int timeout, List<T> parameters, int batchSize) where T : class
        {
            var command = CreateCommand(conn, tran, text, timeout);
            return parameters.ToPaginable(batchSize)
                .Select(i =>
                {
                    command.Parameters.Clear();
                    SetParams(preParameters, command, i.Select(j => j.Value).ToList());
                    return command.ExecuteNonQuery();
                })
                .Sum();
        }

        private DbCommand CreateCommand(DbConnection conn, DbTransaction tran, string text, int timeout)
        {
            var command = conn.CreateCommand();
            command.CommandText = text;
            command.CommandType = CommandType.Text;
            command.CommandTimeout = timeout;
            command.Transaction = tran;
            return command;
        }

        public async ValueTask<int> ExecuteNonQuerysAsync<T>(DataParameterCollection preParameters, DbConnection conn, DbTransaction tran, string text, int timeout, List<T> parameters, int batchSize, CancellationToken cancellationToken) where T : class
        {
            var command = CreateCommand(conn, tran, text, timeout);
            var total = 0;
            foreach (var item in parameters.ToPaginable(batchSize))
            {
                command.Parameters.Clear();
                SetParams(preParameters, command, item.Select(j => j.Value).ToList());
                total += await command.ExecuteNonQueryAsync(cancellationToken);
            }
            return total;
        }
    }
}
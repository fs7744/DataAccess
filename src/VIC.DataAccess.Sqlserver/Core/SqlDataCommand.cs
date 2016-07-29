using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using VIC.DataAccess.Abstratiion;

namespace VIC.DataAccess.Sqlserver.Core
{
    public class SqlDataCommand : IDataCommand
    {
        private DbSql _sql;

        public SqlDataCommand(DbSql sql)
        {
            _sql = sql;
        }

        public void ExecuteBulkCopyAsync<T>(List<T> data) where T : class, new()
        {
            throw new NotImplementedException();
        }

        public Task<DbDataReader> ExecuteDataReaderAsync(dynamic parameter = null)
        {
            return ExecuteDataReaderAsync(CommandBehavior.Default);
        }

        public async Task<T> ExecuteEntityAsync<T>(dynamic paramter = null)
        {
            var reader = await ExecuteDataReaderAsync(CommandBehavior.SingleRow, paramter);
            return ReaderToIEnumerable<T>(reader).FirstOrDefault();
        }

        public async Task<List<T>> ExecuteEntityListAsync<T>(dynamic paramter = null)
        {
            var reader = await ExecuteDataReaderAsync(CommandBehavior.SingleResult, paramter);
            return ReaderToIEnumerable<T>(reader).ToList();
        }

        public Task<IMultipleReader> ExecuteMultipleAsync(dynamic parameter = null)
        {
            throw new NotImplementedException();
        }

        public Task<T> ExecuteScalarAsync<T>(dynamic paramter = null)
        {
            throw new NotImplementedException();
        }

        private async Task<DbDataReader> ExecuteDataReaderAsync(CommandBehavior behavior, dynamic parameter = null)
        {
            var conn = new SqlConnection(_sql.ConnectionString);
            var command = conn.CreateCommand();
            command.CommandText = _sql.Sql;
            command.CommandType = _sql.Type;
            if (parameter != null)
            {
                List<SqlParameter> paramList = GetParamConverter(parameter.GetType())(parameter);
                SetSpecialParameters(paramList);
                command.Parameters.AddRange(paramList.ToArray());
            }

            await conn.OpenAsync();
            return await command.ExecuteReaderAsync(CommandBehavior.CloseConnection | behavior);
        }

        private IEnumerable<T> ReaderToIEnumerable<T>(DbDataReader reader)
        {
            var func = GetReaderConverter(typeof(T));
            yield return func(reader);
        }

        private void SetSpecialParameters(List<SqlParameter> paramList)
        {
            var sps = _sql.SpecialParameters;
            if (sps != null && sps.Count > 0)
            {
                paramList.Where(i => sps.ContainsKey(i.ParameterName)).ToList()
                    .ForEach(i =>
                    {
                        var sp = sps[i.ParameterName];
                        i.DbType = sp.DbType;
                        i.Size = sp.Size;
                        i.IsNullable = sp.IsNullable;
                        i.Direction = sp.Direction;
                    });
            }
        }

        private Func<DbDataReader, dynamic> GetReaderConverter(Type type)
        {
            DbDataReader a = null;
            var b = a.GetColumnSchema();
            return _sql.ReaderConverters.GetOrAdd(type, t =>
            {
                type.GetTypeInfo().GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(i => i.CanWrite);
                var r = Expression.Parameter(SqlTypeExtensions.DbDataReaderType, "r");
                var cols = Expression.Variable(SqlTypeExtensions.DbColumnsType, "cols");
                var colsAssign = Expression.Assign(cols, Expression.Call(r, "GetColumnSchema", new Type[0]));
                
                return Expression.Lambda<Func<DbDataReader, dynamic>>(null, r).Compile();
            });
        }

        private Func<dynamic, List<SqlParameter>> GetParamConverter(Type type)
        {
            return _sql.ParamConverters.GetOrAdd(type, (Type t) =>
            {
                var p = Expression.Parameter(t, "p");
                var vs = Expression.Variable(SqlTypeExtensions.SqlParameterListType, "vs");
                var vsAssign = Expression.Assign(vs, Expression.New(SqlTypeExtensions.SqlParameterListType));
                var v = Expression.Variable(SqlTypeExtensions.SqlParameterType, "v");
                var vAssign = Expression.Assign(v, Expression.New(SqlTypeExtensions.SqlParameterType));
                var ps = t.GetTypeInfo()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(i => i.CanRead)
                .Select(i =>
                {
                    return Expression.Block(new ParameterExpression[1] { v },
                         new Expression[6] {
                                 vAssign,
                                 Expression.Assign(Expression.Property(v, "ParameterName"), Expression.Constant(DbParameter.ParameterNamePrefix + i.Name)),
                                 Expression.Assign(Expression.Property(v, "DbType"),Expression.Constant(i.PropertyType.ToDbType())),
                                 Expression.Assign(Expression.Property(v, "Value"),Expression.Property(p, i)),
                                 Expression.Assign(Expression.Property(v, "IsNullable"),Expression.Constant(true)),
                                 Expression.Call(vs,"Add",new Type[1] { SqlTypeExtensions.SqlParameterListType },v)
                     });
                }).ToList<Expression>();
                ps.Insert(0, vsAssign);
                ps.Add(vs);
                return Expression.Lambda<Func<dynamic, List<SqlParameter>>>(Expression.Block(new ParameterExpression[1] { vs }, ps), p).Compile();
            });
        }
    }
}
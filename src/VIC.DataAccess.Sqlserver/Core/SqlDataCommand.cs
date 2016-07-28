using System;
using System.Collections.Generic;
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

        public void ExecuteBulkCopy<T>(List<T> data) where T : class, new()
        {
            throw new NotImplementedException();
        }

        public Task<DbDataReader> ExecuteDataReaderAsync(dynamic parameter = null)
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
            return command.ExecuteReaderAsync()
                .ContinueWith<DbDataReader>(i => i.Result);
        }

        public T ExecuteEntity<T>(dynamic paramter = null)
        {
            throw new NotImplementedException();
        }

        public List<T> ExecuteEntityList<T>(dynamic paramter = null)
        {
            throw new NotImplementedException();
        }

        public IMultipleReader ExecuteMultiple(dynamic parameter = null)
        {
            throw new NotImplementedException();
        }

        public T ExecuteScalar<T>(dynamic paramter = null)
        {
            throw new NotImplementedException();
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
                        i.SqlDbType = sp.DbType;
                        i.Size = sp.Size;
                        i.IsNullable = sp.IsNullable;
                        i.Direction = sp.Direction;
                    });
            }
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
                                 Expression.Assign(Expression.Property(v, "DbType"),Expression.Constant(i.PropertyType.ToSqlDbType())),
                                 Expression.Assign(Expression.Property(v, "SqlValue"),Expression.Property(p, i)),
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
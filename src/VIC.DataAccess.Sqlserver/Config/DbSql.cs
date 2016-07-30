using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using VIC.DataAccess.Core;

namespace VIC.DataAccess
{
    public class DbSql
    {
        public string SqlName { get; set; }

        public string ConnectionName { get; set; }

        internal string ConnectionString { get; set; }

        public string Sql { get; set; }

        public CommandType Type { get; set; }

        public Dictionary<string, DbParameter> SpecialParameters { get; set; }

        internal ConcurrentDictionary<Type, Func<dynamic, List<SqlParameter>>> ParamConverters { get; set; }

        internal ConcurrentDictionary<Type, Func<DbDataReader, dynamic>> ReaderConverters { get; set; }

        internal Dictionary<Type, Func<DbDataReader, Task<dynamic>>> ScalarConverters { get; set; }

        internal Func<DbDataReader, dynamic> GetReaderConverter(Type type)
        {
            return ReaderConverters.GetOrAdd(type, t =>
            {
                var setter = Expression.Constant(GetSetter(type));
                var v = Expression.Variable(type, "v");
                var vAssign = Expression.Assign(v, Expression.New(type));
                var r = Expression.Parameter(SqlTypeExtensions.DbDataReaderType, "r");
                var lab = Expression.Label();
                var i = Expression.Variable(SqlTypeExtensions.IntType, "i");
                var iAssign = Expression.Assign(i, Expression.Constant(0));
                var loop = Expression.Loop(Expression.IfThenElse(Expression.Equal(i, Expression.Property(r, "FieldCount")),
                    Expression.Break(lab),
                    Expression.Block(
                        Expression.Invoke(setter, v,
                            Expression.Call(
                                Expression.Call(r, "GetName", new Type[1] { SqlTypeExtensions.IntType },
                                    new Expression[1] { i }), "GetHashCode", new Type[0])
                            , i, r)
                        , Expression.AddAssign(i, Expression.Constant(1)))));
                var block = Expression.Block(new ParameterExpression[2] { v, i },
                    new Expression[4] { vAssign, iAssign, loop, v });
                return Expression.Lambda<Func<DbDataReader, dynamic>>(block, r).Compile();
            });
        }

        internal Action<dynamic, int, int, DbDataReader> GetSetter(Type type)
        {
            var v = Expression.Parameter(type, "v");
            var name = Expression.Parameter(SqlTypeExtensions.IntType, "name");
            var index = Expression.Parameter(SqlTypeExtensions.IntType, "i");
            var r = Expression.Parameter(SqlTypeExtensions.DbDataReaderType, "r");
            var switchCases = type.GetTypeInfo().GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(i => i.CanWrite)
            .Select(i => Expression.SwitchCase(Expression.Constant(i.Name.GetHashCode()),
                Expression.Assign(Expression.Property(v, i),
                    Expression.Call(r, i.PropertyType.ToDbFuncName(),
                        new Type[1] { SqlTypeExtensions.IntType }, new Expression[1] { index })))).ToArray();
            return Expression.Lambda<Action<dynamic, int, int, DbDataReader>>(
                Expression.Block(Expression.Switch(name, switchCases)), v, name, index, r).Compile();
        }

        internal Func<dynamic, List<SqlParameter>> GetParamConverter(Type type)
        {
            return ParamConverters.GetOrAdd(type, (Type t) =>
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

        internal Func<DbDataReader, Task<dynamic>> GetScalarConverter(Type type)
        {
            Func<DbDataReader, Task<dynamic>> result = null;
            ScalarConverters.TryGetValue(type, out result);
            return result;
        }
    }
}
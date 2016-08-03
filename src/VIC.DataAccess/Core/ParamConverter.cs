using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using VIC.DataAccess.Abstraction;

namespace VIC.DataAccess.Core
{
    public class ParamConverter : IParamConverter
    {
        protected ConcurrentDictionary<Type, Func<dynamic, List<DbParameter>>> _PCs =
           new ConcurrentDictionary<Type, Func<dynamic, List<DbParameter>>>();

        protected IDbTypeConverter _DC;

        public ParamConverter(IDbTypeConverter dc)
        {
            _DC = dc;
        }

        public List<DbParameter> Convert(Type type, dynamic parameters)
        {
            return _PCs.GetOrAdd(type, (Type t) =>
            {
                var o = Expression.Parameter(TypeHelper.ObjectType, "o");
                var p = Expression.Variable(t, "p");
                var passign = Expression.Assign(p, Expression.Convert(o, t));
                var vs = Expression.Variable(TypeHelper.SqlParameterListType, "vs");
                var vsAssign = Expression.Assign(vs, Expression.New(TypeHelper.SqlParameterListType));
                var v = Expression.Variable(TypeHelper.SqlParameterType, "v");
                var vAssign = Expression.Assign(v, Expression.New(TypeHelper.SqlParameterType));
                var ps = TypeExtensions.GetProperties(t, BindingFlags.Instance | BindingFlags.Public)
                .Where(i => i.CanRead)
                .Select(i =>
                {
                    return Expression.Block(new ParameterExpression[] { v },
                         new Expression[] {
                                 vAssign,
                                 Expression.Assign(Expression.Property(v, "ParameterName"), Expression.Constant(DataParameter.ParameterNamePrefix + i.Name)),
                                 Expression.Assign(Expression.Property(v, "DbType"),Expression.Constant(_DC.Convert(i.PropertyType))),
                                 Expression.Assign(Expression.Property(v, "Value"),Expression.Convert(Expression.Property(p, i),TypeHelper.ObjectType)),
                                 Expression.Assign(Expression.Property(v, "IsNullable"),Expression.Constant(true)),
                                 Expression.Call(vs,"Add",new Type[0] ,v)
                     });
                }).ToList<Expression>();
                ps.Insert(0, vsAssign);
                ps.Insert(0, passign);
                ps.Add(vs);
                var bb = Expression.Block(new ParameterExpression[] { vs, p }, ps);
                var dd = Expression.Lambda<Func<dynamic, List<DbParameter>>>(bb, o);
                var f = dd.Compile();
                return f;
            })(parameters);
        }
    }
}
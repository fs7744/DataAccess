using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using VIC.DataAccess.Abstraction.Converter;

namespace VIC.DataAccess.Core.Converter
{
    public abstract class ParamConverter : IParamConverter
    {
        protected ConcurrentDictionary<Type, Tuple<Func<dynamic, List<DbParameter>>, Func<dynamic, IGrouping<string, DbParameter>[]>>> _PCs =
           new ConcurrentDictionary<Type, Tuple<Func<dynamic, List<DbParameter>>, Func<dynamic, IGrouping<string, DbParameter>[]>>>();

        private MethodInfo toStr = typeof(object).GetMethod("ToString");
        private MethodInfo concat = typeof(string).GetMethods().Where(i => i.Name == "Concat" && i.GetParameters().Length == 2 && i.GetParameters().First().ParameterType == typeof(string)).ToList()[0];

        protected IDbTypeConverter _DC;

        public ParamConverter(IDbTypeConverter dc)
        {
            _DC = dc;
        }

        protected abstract Type GetParameterType();

        public Tuple<Func<dynamic, List<DbParameter>>, Func<dynamic, IGrouping<string, DbParameter>[]>> GetConverter(Type type)
        {
            return _PCs.GetOrAdd(type, (Type t) =>
            {
                var ps = TypeExtensions.GetProperties(t, BindingFlags.Instance | BindingFlags.Public)
                .Where(i => i.CanRead)
                .GroupBy(i => i.PropertyType != typeof(string) && typeof(IEnumerable).IsAssignableFrom(i.PropertyType))
                .ToArray();
                var result = ps[0].Key
                    ? Tuple.Create(CreateSimpleDbParameters(t, ps.Length == 2 ? ps[1] : null), CreateEnumerableDbParameters(t, ps[0]))
                    : Tuple.Create(CreateSimpleDbParameters(t, ps[0]), CreateEnumerableDbParameters(t, ps.Length == 2 ? ps[1] : null));
                return result;
            });
        }

        private Func<dynamic, IGrouping<string, DbParameter>[]> CreateEnumerableDbParameters(Type t, IGrouping<bool, PropertyInfo> pis)
        {
            if (pis == null) return d => new IGrouping<string, DbParameter>[0];
            var o = Expression.Parameter(TypeHelper.ObjectType, "o");
            var p = Expression.Variable(t, "p");
            var passign = Expression.Assign(p, Expression.Convert(o, t));
            var vs = Expression.Variable(TypeHelper.SqlParameterListType, "vs");
            var vsAssign = Expression.Assign(vs, Expression.New(TypeHelper.SqlParameterListType));
            var v = Expression.Variable(GetParameterType(), "v");
            var vAssign = Expression.Assign(v, Expression.New(GetParameterType()));
            var index = Expression.Variable(typeof(int), "i");
            var enumeratorVar = Expression.Variable(typeof(IEnumerator), "enumerator");
            var ps = pis.Select(i =>
            {
                var pt = i.PropertyType;
                var realType = pt.HasElementType ? pt.GetElementType() : pt.GenericTypeArguments.FirstOrDefault();
                var moveNext = Expression.Call(enumeratorVar, typeof(IEnumerator).GetMethod("MoveNext"));
                var breakLabel = Expression.Label("LoopBreak");
                var loop = Expression.Block(new[] { v },
                    new Expression[] {
                        Expression.Assign(enumeratorVar, Expression.Call(Expression.Property(p, i), typeof(IEnumerable).GetMethod("GetEnumerator"))),
                        Expression.Assign(index, Expression.Constant(0)),
                        Expression.Loop(Expression.IfThenElse(moveNext,
                            Expression.Block(new[] { v },new Expression[]
                            {
                                 vAssign,
                                 Expression.Assign(Expression.Property(v, "SourceColumn"), Expression.Constant(DataParameter.ParameterNamePrefix + i.Name)),
                                 Expression.Assign(Expression.Property(v, "ParameterName"), Expression.Call(concat, Expression.Constant(DataParameter.ParameterNamePrefix + i.Name), Expression.Call(index, toStr))),
                                 Expression.Assign(Expression.Property(v, "DbType"),Expression.Constant(_DC.Convert(realType))),
                                 Expression.Assign(Expression.Property(v, "Value"), Expression.Property(enumeratorVar, "Current")),
                                 Expression.Assign(Expression.Property(v, "IsNullable"),Expression.Constant(true)),
                                 Expression.Assign(Expression.Property(v, "Direction"),Expression.Constant(ParameterDirection.Input)),
                                 Expression.Call(vs,"Add",new Type[0] ,v),
                                 Expression.AddAssign(index, Expression.Constant(1))
                            }), Expression.Break(breakLabel)), breakLabel)
                    });
                return loop;
            }).ToList<Expression>();
            ps.Insert(0, vsAssign);
            ps.Insert(0, passign);
            ps.Add(vs);
            var bb = Expression.Block(new ParameterExpression[] { vs, p, enumeratorVar, index }, ps);
            var dd = Expression.Lambda<Func<dynamic, List<DbParameter>>>(bb, o);
            var f = dd.Compile();
            return d =>
            {
                List<DbParameter> result = f(d);
                return result.GroupBy(i => i.SourceColumn).ToArray();
            };
        }

        private Func<dynamic, List<DbParameter>> CreateSimpleDbParameters(Type t, IGrouping<bool, PropertyInfo> pis)
        {
            if (pis == null) return d => new List<DbParameter>();
            var o = Expression.Parameter(TypeHelper.ObjectType, "o");
            var p = Expression.Variable(t, "p");
            var passign = Expression.Assign(p, Expression.Convert(o, t));
            var vs = Expression.Variable(TypeHelper.SqlParameterListType, "vs");
            var vsAssign = Expression.Assign(vs, Expression.New(TypeHelper.SqlParameterListType));
            var v = Expression.Variable(GetParameterType(), "v");
            var vAssign = Expression.Assign(v, Expression.New(GetParameterType()));
            var ps = pis.Select(i =>
            {
                return Expression.Block(new ParameterExpression[] { v },
                     new Expression[] {
                                 vAssign,
                                 Expression.Assign(Expression.Property(v, "ParameterName"), Expression.Constant(DataParameter.ParameterNamePrefix + i.Name)),
                                 Expression.Assign(Expression.Property(v, "DbType"),Expression.Constant(_DC.Convert(i.PropertyType))),
                                 Expression.Assign(Expression.Property(v, "Value"),Expression.Convert(Expression.Property(p, i),TypeHelper.ObjectType)),
                                 Expression.Assign(Expression.Property(v, "IsNullable"),Expression.Constant(true)),
                                 Expression.Assign(Expression.Property(v, "Direction"),Expression.Constant(ParameterDirection.Input)),
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
        }
    }
}
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Vic.Data.Abstraction;

namespace Vic.Data
{
    public class ExpressionEntityConverter<T> : IEntityConverter<T> where T : class
    {
        protected Func<DbDataReader, T> cache;
        protected readonly IDbValueGetConverter dbValueGetter;
        private readonly object _lock = new object();

        public ExpressionEntityConverter(IDbValueGetConverter fc)
        {
            dbValueGetter = fc;
        }

        public T Convert(DbDataReader reader)
        {
            if (cache == null)
            {
                lock (_lock)
                {
                    if (cache == null)
                    {
                        cache = CreateConverter(typeof(T), reader);
                    }
                }
            }
            return cache(reader);
        }

        private Func<DbDataReader, T> CreateConverter(Type t, DbDataReader reader)
        {
            var v = Expression.Variable(t, "v");
            var vAssign = Expression.Assign(v, Expression.New(t.GetConstructor(Type.EmptyTypes)));
            var r = Expression.Parameter(typeof(DbDataReader), "r");
            var properties = TypeExtensions.GetProperties(t, BindingFlags.Instance | BindingFlags.Public)
              .Where(j => j.CanWrite).ToDictionary(x => x.Name, StringComparer.OrdinalIgnoreCase);
            var setters = Enumerable.Range(0, reader.FieldCount)
                .Select(i => CreateSetter(i, reader, r, v, properties))
                .Where(i => i != null)
                .ToList();
            setters.Insert(0, vAssign);
            setters.Add(v);
            var block = Expression.Block(new ParameterExpression[] { v }, setters);
            return Expression.Lambda<Func<DbDataReader, T>>(block, r).Compile();
        }

        private Expression CreateSetter(int i, DbDataReader reader, ParameterExpression r, ParameterExpression v, Dictionary<string, PropertyInfo> properties)
        {
            var name = reader.GetName(i);
            if (properties.TryGetValue(name, out PropertyInfo info))
            {
                var getter = Expression.Call(r, dbValueGetter.Convert(info.PropertyType), new Expression[] { Expression.Constant(i) });
                return Expression.Call(v, info.GetSetMethod(), getter);
            }
            else
            {
                return null;
            }
        }
    }
}
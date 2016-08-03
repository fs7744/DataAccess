using System;
using System.Collections.Concurrent;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using VIC.DataAccess.Abstraction;

namespace VIC.DataAccess.Core
{
    public class EntityConverter : IEntityConverter
    {
        protected ConcurrentDictionary<Type, Func<DbDataReader, dynamic>> _ECs =
            new ConcurrentDictionary<Type, Func<DbDataReader, dynamic>>();

        protected IDbFuncNameConverter _FC;

        public EntityConverter(IDbFuncNameConverter fc)
        {
            _FC = fc;
        }

        public dynamic Convert<T>(DbDataReader reader)
        {
            var type = typeof(T);
            return _ECs.GetOrAdd(type, t =>
            {
                var setter = Expression.Constant(GetSetter(type));
                var v = Expression.Variable(type, "v");
                var vAssign = Expression.Assign(v, Expression.New(type));
                var r = Expression.Parameter(TypeHelper.DbDataReaderType, "r");
                var lab = Expression.Label("exit");
                var i = Expression.Variable(TypeHelper.IntType, "i");
                var iAssign = Expression.Assign(i, Expression.Constant(0));
                var loop = Expression.Loop(Expression.IfThenElse(Expression.Equal(i, Expression.Property(r, "FieldCount")),
                    Expression.Break(lab),
                    Expression.Block(
                        Expression.Invoke(setter, v,
                            Expression.Call(
                                Expression.Call(r, "GetName", new Type[0],
                                    new Expression[] { i }), "GetHashCode", new Type[0])
                            , i, r)
                        , Expression.AddAssign(i, Expression.Constant(1)))), lab);
                var block = Expression.Block(new ParameterExpression[] { v, i },
                    new Expression[4] { vAssign, iAssign, loop, v });
                return Expression.Lambda<Func<DbDataReader, dynamic>>(block, r).Compile();
            })(reader);
        }

        protected Action<dynamic, int, int, DbDataReader> GetSetter(Type type)
        {
            var o = Expression.Parameter(TypeHelper.ObjectType, "o");
            var v = Expression.Variable(type, "v");
            var passign = Expression.Assign(v, Expression.Convert(o, type));
            var name = Expression.Parameter(TypeHelper.IntType, "name");
            var index = Expression.Parameter(TypeHelper.IntType, "i");
            var r = Expression.Parameter(TypeHelper.DbDataReaderType, "r");
            var switchCases = TypeExtensions.GetProperties(type, BindingFlags.Instance | BindingFlags.Public)
            .Where(i => i.CanWrite)
            .Select(i => Expression.SwitchCase(
                Expression.Block(TypeHelper.VoidType, Expression.Assign(Expression.Property(v, i),
                    Expression.Call(r, _FC.Convert(i.PropertyType),
                        new Type[0], new Expression[1] { index }))), Expression.Constant(i.Name.GetHashCode(), TypeHelper.IntType))).ToArray();

            var dd = Expression.Switch(name, switchCases);
            return Expression.Lambda<Action<dynamic, int, int, DbDataReader>>(
                Expression.Block(new ParameterExpression[] { v }, new Expression[] { passign, dd }), o, name, index, r).Compile();
        }
    }
}
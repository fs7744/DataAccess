using System;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Vic.Data.Abstraction;

namespace Vic.Data
{
    public class EmitEntityConverter<T> : IEntityConverter<T> where T : class
    {
        protected Func<DbDataReader, T> cache;
        protected readonly IDbValueGetConverter dbValueGetter;
        private readonly object _lock = new object();

        public EmitEntityConverter(IDbValueGetConverter fc)
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

        private Func<DbDataReader, T> CreateConverter(Type type, DbDataReader reader)
        {
            var getItem = typeof(IDataRecord).GetProperties(BindingFlags.Instance | BindingFlags.Public)
                        .Where(p => p.GetIndexParameters().Length > 0 && p.GetIndexParameters()[0].ParameterType == typeof(int))
                        .Select(p => p.GetGetMethod()).First();
            var dynamicMethod = new DynamicMethod($"invoker_{Guid.NewGuid()}", type, new Type[] { typeof(IDataReader) }, type, true);
            ILGenerator il = dynamicMethod.GetILGenerator();

            il.DeclareLocal(type);
            //il.DeclareLocal(typeof(int));
            //il.DeclareLocal(typeof(object));
            il.Emit(OpCodes.Newobj, type.GetConstructor(Type.EmptyTypes));
            il.Emit(OpCodes.Stloc_0);
            //il.Emit(OpCodes.Ldc_I4_0);
            //il.Emit(OpCodes.Stloc_1);
            //il.BeginExceptionBlock();
            il.Emit(OpCodes.Ldloc_0);// [target]

            var setters = TypeExtensions.GetProperties(type, BindingFlags.Instance | BindingFlags.Public)
            .Where(i => i.CanWrite)
            .ToList();
            var readers = Enumerable.Range(0, reader.FieldCount)
                .Select(i =>
                {
                    var name = reader.GetName(i);
                    var setter = setters.FirstOrDefault(j => j.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
                    return Tuple.Create(i, setter);
                })
                .Where(i => i.Item2 != null)
                .ToArray();
            foreach (var item in readers)
            {
                Label isDbNullLabel = il.DefineLabel();
                Label finishLabel = il.DefineLabel();
                il.Emit(OpCodes.Ldarg_0);
                EmitInt(il, item.Item1);
                var method = item.Item2.SetMethod;
                var parameterType = method.GetParameters().Select(x => x.ParameterType).First();
                //il.Emit(OpCodes.Dup);// stack is now [target][reader][index][index]
                //il.Emit(OpCodes.Stloc_1);// stack is now [target][reader][index]
                il.Emit(OpCodes.Callvirt, getItem);
                //il.Emit(OpCodes.Callvirt, dbValueGetter.Convert(item.Item2.PropertyType));
                //il.Emit(OpCodes.Dup); // stack is now [target][value-as-object][value-as-object]
                //il.Emit(OpCodes.Stloc_2);
                il.Emit(OpCodes.Dup); // stack is now [target][value][value]
                il.Emit(OpCodes.Isinst, typeof(DBNull)); // stack is now [target][value-as-object][DBNull or null]
                il.Emit(OpCodes.Brtrue_S, isDbNullLabel);
                //if (parameterType.IsValueType)
                //{
                il.Emit(OpCodes.Unbox_Any, parameterType);
                //}
                //else
                //{
                //    il.EmitConvertFromObject(parameterType);
                //}
                il.Emit(OpCodes.Callvirt, method);
                il.Emit(OpCodes.Ldloc_0);
                il.Emit(OpCodes.Br_S, finishLabel);
                il.MarkLabel(isDbNullLabel); // incoming stack: [target][value]
                il.Emit(OpCodes.Pop); // stack is now [target]
                //il.Emit(OpCodes.Pop);
                il.MarkLabel(finishLabel);
            }
            //il.Emit(OpCodes.Ldloc_0);
            il.Emit(OpCodes.Ret);
            var funcType = System.Linq.Expressions.Expression.GetFuncType(typeof(IDataReader), type);
            return (Func<IDataReader, T>)dynamicMethod.CreateDelegate(funcType);
        }

        public void EmitInt(ILGenerator ilGenerator, int value)
        {
            OpCode c;
            switch (value)
            {
                case -1:
                    c = OpCodes.Ldc_I4_M1;
                    break;

                case 0:
                    c = OpCodes.Ldc_I4_0;
                    break;

                case 1:
                    c = OpCodes.Ldc_I4_1;
                    break;

                case 2:
                    c = OpCodes.Ldc_I4_2;
                    break;

                case 3:
                    c = OpCodes.Ldc_I4_3;
                    break;

                case 4:
                    c = OpCodes.Ldc_I4_4;
                    break;

                case 5:
                    c = OpCodes.Ldc_I4_5;
                    break;

                case 6:
                    c = OpCodes.Ldc_I4_6;
                    break;

                case 7:
                    c = OpCodes.Ldc_I4_7;
                    break;

                case 8:
                    c = OpCodes.Ldc_I4_8;
                    break;

                default:
                    if (value >= -128 && value <= 127)
                    {
                        ilGenerator.Emit(OpCodes.Ldc_I4_S, (sbyte)value);
                    }
                    else
                    {
                        ilGenerator.Emit(OpCodes.Ldc_I4, value);
                    }
                    return;
            }
            ilGenerator.Emit(c);
        }
    }
}
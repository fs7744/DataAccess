using System;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Concurrent;
using System.Text;

namespace VIC.DataAccess.Core.Converter
{
    public static class EmitEntityConverter<T>
    {
        private static ConcurrentDictionary<int, Func<IDataReader, T>> cache
            = new ConcurrentDictionary<int, Func<IDataReader, T>>();

        private static readonly MethodInfo getItem = typeof(IDataRecord).GetProperties(BindingFlags.Instance | BindingFlags.Public)
                        .Where(p => p.GetIndexParameters().Length > 0 && p.GetIndexParameters()[0].ParameterType == typeof(int))
                        .Select(p => p.GetGetMethod()).First();

        public static Func<IDataReader, T> GetConverter(IDataReader reader)
        {
            var key = GetKey(reader);
            return cache.GetOrAdd(key, k => CreateConverter(typeof(T), reader));
        }

        private static int GetKey(IDataReader reader)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                sb.Append(reader.GetName(i).ToLower());
            }
            return sb.ToString().GetHashCode();
        }

        private static Func<IDataReader, T> CreateConverter(Type type, IDataReader reader)
        {
            var dynamicMethod = new DynamicMethod($"invoker_{Guid.NewGuid()}", type, new Type[] { typeof(IDataReader) }, type, true);
            ILGenerator il = dynamicMethod.GetILGenerator();

            il.DeclareLocal(type);
            il.Emit(OpCodes.Newobj, type.GetConstructor(Type.EmptyTypes));
            il.Emit(OpCodes.Stloc_0);
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
                il.Emit(OpCodes.Callvirt, getItem);
                il.Emit(OpCodes.Dup); // stack is now [target][value][value]
                il.Emit(OpCodes.Isinst, typeof(DBNull)); // stack is now [target][value-as-object][DBNull or null]
                il.Emit(OpCodes.Brtrue_S, isDbNullLabel);
                il.Emit(OpCodes.Unbox_Any, parameterType);
                il.Emit(OpCodes.Callvirt, method);
                il.Emit(OpCodes.Ldloc_0);
                il.Emit(OpCodes.Br_S, finishLabel);
                il.MarkLabel(isDbNullLabel); // incoming stack: [target][value]
                il.Emit(OpCodes.Pop); // stack is now [target]
                il.MarkLabel(finishLabel);
            }
            il.Emit(OpCodes.Ret);
            var funcType = System.Linq.Expressions.Expression.GetFuncType(typeof(IDataReader), type);
            return (Func<IDataReader, T>)dynamicMethod.CreateDelegate(funcType);
        }

        public static void EmitInt(ILGenerator ilGenerator, int value)
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
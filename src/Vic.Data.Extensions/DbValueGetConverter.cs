using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using Vic.Data.Abstraction;

namespace Vic.Data
{
    public class DbValueGetConverter : IDbValueGetConverter
    {
        private Dictionary<Type, MethodInfo> cache;
        private MethodInfo getValue;

        public DbValueGetConverter()
        {
            var type = typeof(IDataRecord);
            getValue = type.GetMethod("GetValue");
            cache = new Dictionary<Type, MethodInfo>()
            {
                { typeof(long),     type.GetMethod("GetInt64") },
                { typeof(bool),     type.GetMethod("GetBoolean") },
                { typeof(string),   type.GetMethod("GetString") },
                { typeof(DateTime), type.GetMethod("GetDateTime") },
                { typeof(decimal),  type.GetMethod("GetDecimal")},
                { typeof(double),   type.GetMethod("GetDouble") },
                { typeof(int),      type.GetMethod("GetInt32")},
                { typeof(float),    type.GetMethod("GetFloat")  },
                { typeof(short),    type.GetMethod("GetInt16")  },
                { typeof(byte),     type.GetMethod("GetByte")  },
                { typeof(Guid),     type.GetMethod("GetGuid")  },
                { typeof(object),   getValue   },
            };
        }

        public MethodInfo Convert(Type type)
        {
            return cache.TryGetValue(GetRealType(type), out MethodInfo result) ? result : getValue;
        }

        public Type GetRealType(Type type)
        {
            return type.GetTypeInfo().IsGenericType && typeof(Nullable<>) == type.GetGenericTypeDefinition()
                ? type.GenericTypeArguments[0]
                : type;
        }
    }
}
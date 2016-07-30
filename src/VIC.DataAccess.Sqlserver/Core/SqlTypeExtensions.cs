using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Reflection;

namespace VIC.DataAccess.Core
{
    public static class SqlTypeExtensions
    {
        public static readonly Type SqlParameterType = typeof(SqlParameter);
        public static readonly Type SqlParameterListType = typeof(List<SqlParameter>);
        public static readonly Type NullableType = typeof(Nullable<>);

        public static readonly Type DbDataReaderType = typeof(DbDataReader);

        public static readonly Type IntType = typeof(int);

        public static readonly Dictionary<Type, DbType> TypeToDbType = new Dictionary<Type, DbType>()
        {
            { typeof(long),             DbType.Int64 },
            { typeof(bool),             DbType.Boolean },
            { typeof(string),           DbType.String },
            { typeof(DateTime),         DbType.DateTime2 },
            { typeof(decimal),          DbType.Decimal},
            { typeof(double),           DbType.Double },
            { typeof(int),              DbType.Int32},
            { typeof(float),            DbType.Single  },
            { typeof(short),            DbType.Int16  },
            { typeof(byte),             DbType.Byte  },
            { typeof(Guid),             DbType.Guid  }
        };

        public static readonly Dictionary<Type, string> TypeToDbFuncName = new Dictionary<Type, string>()
        {
            { typeof(long),             "GetInt64" },
            { typeof(bool),             "GetBoolean" },
            { typeof(string),           "GetString" },
            { typeof(DateTime),         "GetDateTime" },
            { typeof(decimal),          "GetDecimal"},
            { typeof(double),           "GetDouble" },
            { typeof(int),              "GetInt32"},
            { typeof(float),            "GetFloat"  },
            { typeof(short),            "GetInt16"  },
            { typeof(byte),             "GetByte"  },
            { typeof(Guid),             "GetGuid"  }
        };

        public static DbType ToDbType(this Type type)
        {
            var result = DbType.String;
            TypeToDbType.TryGetValue(GetRealType(type), out result);
            return result;
        }

        private static Type GetRealType(Type type)
        {
            return type.GetTypeInfo().IsGenericType && NullableType == type.GetGenericTypeDefinition()
                ? type.GenericTypeArguments[0]
                : type;
        }

        public static string ToDbFuncName(this Type type)
        {
            var result = "GetValue";
            TypeToDbFuncName.TryGetValue(GetRealType(type), out result);
            return result;
        }
    }
}
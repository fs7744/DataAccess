using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;

namespace VIC.DataAccess.Sqlserver.Core
{
    public static class SqlTypeExtensions
    {
        public static readonly Type SqlParameterType = typeof(SqlParameter);
        public static readonly Type SqlParameterListType = typeof(List<SqlParameter>);
        public static readonly Type NullableType = typeof(Nullable<>);

        public static readonly Dictionary<Type, DbType> TypeToDbType = new Dictionary<Type, DbType>()
        {
            { typeof(long),             DbType.Int64 },
            { typeof(byte[]),           DbType.Binary },
            { typeof(bool),             DbType.Boolean },
            { typeof(string),           DbType.String },
            { typeof(char[]),           DbType.String},
            { typeof(DateTime),         DbType.DateTime2 },
            { typeof(DateTimeOffset),   DbType.DateTimeOffset},
            { typeof(decimal),          DbType.Decimal},
            { typeof(double),           DbType.Double },
            { typeof(int),              DbType.Int32},
            { typeof(float),            DbType.Single  },
            { typeof(short),            DbType.Int16  },
            { typeof(TimeSpan),         DbType.Time },
            { typeof(byte),             DbType.Byte  },
            { typeof(Guid),             DbType.Guid  }
        };

        public static DbType ToDbType(this Type type)
        {
            var result = DbType.String;
            var key = type.GetTypeInfo().IsGenericType && NullableType == type.GetGenericTypeDefinition()
                ? type.GenericTypeArguments[0]
                : type;
            TypeToDbType.TryGetValue(key, out result);
            return result;
        }
    }
}
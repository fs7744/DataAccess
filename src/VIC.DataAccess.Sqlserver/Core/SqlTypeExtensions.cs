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

        public static readonly Dictionary<Type, SqlDbType> TypeToSqlDbType = new Dictionary<Type, SqlDbType>()
        {
            {typeof(long),SqlDbType.BigInt },
            {typeof(byte[]),SqlDbType.VarBinary},
            {typeof(bool),SqlDbType.Bit},
            {typeof(string),SqlDbType.NVarChar},
            {typeof(char[]),SqlDbType.NVarChar},
            {typeof(DateTime),SqlDbType.DateTime2 },
            {typeof(DateTimeOffset),SqlDbType.DateTimeOffset},
            {typeof(decimal),SqlDbType.Decimal},
            {typeof(double),SqlDbType.Float },
            {typeof(int),SqlDbType.Int},
            {typeof(float),SqlDbType.Real },
            {typeof(short),SqlDbType.SmallInt },
            {typeof(TimeSpan),SqlDbType.Time },
            {typeof(byte),SqlDbType.TinyInt },
            {typeof(Guid),SqlDbType.UniqueIdentifier  }
        };

        public static SqlDbType ToSqlDbType(this Type type)
        {
            var result = SqlDbType.NVarChar;
            var key = type.GetTypeInfo().IsGenericType && NullableType == type.GetGenericTypeDefinition()
                ? type.GenericTypeArguments[0]
                : type;
            TypeToSqlDbType.TryGetValue(key, out result);
            return result;
        }
    }
}
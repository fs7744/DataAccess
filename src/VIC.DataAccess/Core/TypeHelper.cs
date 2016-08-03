using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Reflection;

namespace VIC.DataAccess.Core
{
    public static class TypeHelper
    {
        public static Type SqlParameterType = typeof(DbParameter);
        public static readonly Type SqlParameterListType = typeof(List<DbParameter>);
        public static readonly Type NullableType = typeof(Nullable<>);
        public static readonly Type DbDataReaderType = typeof(DbDataReader);
        public static readonly Type IntType = typeof(int);
        public static readonly Type ObjectType = typeof(object);
        public static readonly Type VoidType = typeof(void);

        public static Type GetRealType(this Type type)
        {
            return type.GetTypeInfo().IsGenericType && NullableType == type.GetGenericTypeDefinition()
                ? type.GenericTypeArguments[0]
                : type;
        }
    }
}
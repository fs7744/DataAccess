using System;
using System.Collections.Generic;
using System.Data;
using VIC.DataAccess.Abstraction;

namespace VIC.DataAccess.Core
{
    public class DbTypeConverter : IDbTypeConverter
    {
        private Dictionary<Type, DbType> _DCs = new Dictionary<Type, DbType>()
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

        public DbType Convert(Type type)
        {
            var result = DbType.String;
            _DCs.TryGetValue(type.GetRealType(), out result);
            return result;
        }
    }
}
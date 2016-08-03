using System;
using System.Collections.Generic;
using System.Data.Common;
using VIC.DataAccess.Abstraction;

namespace VIC.DataAccess.Core
{
    public class ScalarConverter : IScalarConverter
    {
        private Dictionary<Type, Func<DbDataReader, dynamic>> _SCs
            = new Dictionary<Type, Func<DbDataReader, dynamic>>()
            {
                { typeof(long), i => i.GetInt64(0)},
                { typeof(bool), i => i.GetBoolean(0)},
                { typeof(string), i => i.GetString(0)},
                { typeof(DateTime), i => i.GetDateTime(0)},
                { typeof(decimal), i => i.GetDecimal(0)},
                { typeof(double), i => i.GetDouble(0)},
                { typeof(int), i => i.GetInt32(0)},
                { typeof(float), i => i.GetFloat(0)},
                { typeof(short), i => i.GetInt16(0)},
                { typeof(byte), i => i.GetByte(0)},
                { typeof(Guid), i => i.GetGuid(0)},
            };

        public dynamic Convert<T>(DbDataReader reader)
        {
            Func<DbDataReader, dynamic> func = null;
            return _SCs.TryGetValue(typeof(T), out func) ? func(reader) : default(T);
        }
    }
}
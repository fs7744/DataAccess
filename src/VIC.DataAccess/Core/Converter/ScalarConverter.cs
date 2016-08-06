using System;
using System.Collections.Generic;
using System.Data.Common;
using VIC.DataAccess.Abstraction.Converter;

namespace VIC.DataAccess.Core.Converter
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
                { typeof(long?), i => i.IsDBNull(0) ? (long?)i.GetInt64(0) : null},
                { typeof(bool?), i => i.IsDBNull(0) ? (bool?)i.GetBoolean(0) : null},
                { typeof(DateTime?), i => i.IsDBNull(0) ? (DateTime?)i.GetDateTime(0) : null},
                { typeof(decimal?), i => i.IsDBNull(0) ? (decimal?)i.GetDecimal(0) : null},
                { typeof(double?), i => i.IsDBNull(0) ? (double?)i.GetDouble(0) : null},
                { typeof(int?), i => i.IsDBNull(0) ? (int?)i.GetInt32(0) : null},
                { typeof(float?), i => i.IsDBNull(0) ? (float?)i.GetFloat(0) : null},
                { typeof(short?), i => i.IsDBNull(0) ? (short?)i.GetInt16(0) : null},
                { typeof(byte?), i => i.IsDBNull(0) ? (byte?)i.GetByte(0) : null},
                { typeof(Guid?), i => i.IsDBNull(0) ? (Guid?)i.GetGuid(0) : null},
            };

        public dynamic Convert<T>(DbDataReader reader)
        {
            Func<DbDataReader, dynamic> func = null;
            return _SCs.TryGetValue(typeof(T), out func) ? func(reader) : default(T);
        }
    }
}
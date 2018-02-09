using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace VIC.DataAccess.Extensions
{
    public static class DataRecordHelper
    {
        public static string GetStringWithNull(this IDataRecord record, int index)
        {
            return record.IsDBNull(index) ? null : record.GetString(index);
        }

        public static string GetStringWithNull(this IDataRecord record, string name)
        {
            return record.GetStringWithNull(record.GetOrdinal(name));
        }


        public static bool GetBoolean(this IDataRecord record, string name)
        {
            return record.GetBoolean(record.GetOrdinal(name));
        }

        public static bool? GetBooleanWithNull(this IDataRecord record, int index)
        {
            return record.IsDBNull(index) ? (bool?)null : record.GetBoolean(index);
        }

        public static bool? GetBooleanWithNull(this IDataRecord record, string name)
        {
            return record.GetBooleanWithNull(record.GetOrdinal(name));
        }


        public static byte GetByte(this IDataRecord record, string name)
        {
            return record.GetByte(record.GetOrdinal(name));
        }

        public static byte? GetByteWithNull(this IDataRecord record, int index)
        {
            return record.IsDBNull(index) ? (byte?)null : record.GetByte(index);
        }

        public static byte? GetByteWithNull(this IDataRecord record, string name)
        {
            return record.GetByteWithNull(record.GetOrdinal(name));
        }


        public static char GetChar(this IDataRecord record, string name)
        {
            return record.GetChar(record.GetOrdinal(name));
        }

        public static char? GetCharWithNull(this IDataRecord record, int index)
        {
            return record.IsDBNull(index) ? (char?)null : record.GetChar(index);
        }

        public static char? GetCharWithNull(this IDataRecord record, string name)
        {
            return record.GetCharWithNull(record.GetOrdinal(name));
        }


        public static DateTime GetDateTime(this IDataRecord record, string name)
        {
            return record.GetDateTime(record.GetOrdinal(name));
        }

        public static DateTime? GetDateTimeWithNull(this IDataRecord record, int index)
        {
            return record.IsDBNull(index) ? (DateTime?)null : record.GetDateTime(index);
        }

        public static DateTime? GetDateTimeWithNull(this IDataRecord record, string name)
        {
            return record.GetDateTimeWithNull(record.GetOrdinal(name));
        }


        public static decimal GetDecimal(this IDataRecord record, string name)
        {
            return record.GetDecimal(record.GetOrdinal(name));
        }

        public static decimal? GetDecimalWithNull(this IDataRecord record, int index)
        {
            return record.IsDBNull(index) ? (decimal?)null : record.GetDecimal(index);
        }

        public static decimal? GetDecimalWithNull(this IDataRecord record, string name)
        {
            return record.GetDecimalWithNull(record.GetOrdinal(name));
        }


        public static double GetDouble(this IDataRecord record, string name)
        {
            return record.GetDouble(record.GetOrdinal(name));
        }

        public static double? GetDoubleWithNull(this IDataRecord record, int index)
        {
            return record.IsDBNull(index) ? (double?)null : record.GetDouble(index);
        }

        public static double? GetDoubleWithNull(this IDataRecord record, string name)
        {
            return record.GetDoubleWithNull(record.GetOrdinal(name));
        }


        public static float GetFloat(this IDataRecord record, string name)
        {
            return record.GetFloat(record.GetOrdinal(name));
        }

        public static float? GetFloatWithNull(this IDataRecord record, int index)
        {
            return record.IsDBNull(index) ? (float?)null : record.GetFloat(index);
        }

        public static float? GetFloatWithNull(this IDataRecord record, string name)
        {
            return record.GetFloatWithNull(record.GetOrdinal(name));
        }


        public static Guid GetGuid(this IDataRecord record, string name)
        {
            return record.GetGuid(record.GetOrdinal(name));
        }

        public static Guid? GetGuidWithNull(this IDataRecord record, int index)
        {
            return record.IsDBNull(index) ? (Guid?)null : record.GetGuid(index);
        }

        public static Guid? GetGuidWithNull(this IDataRecord record, string name)
        {
            return record.GetGuidWithNull(record.GetOrdinal(name));
        }


        public static short GetInt16(this IDataRecord record, string name)
        {
            return record.GetInt16(record.GetOrdinal(name));
        }

        public static short? GetInt16WithNull(this IDataRecord record, int index)
        {
            return record.IsDBNull(index) ? (short?)null : record.GetInt16(index);
        }

        public static short? GetInt16WithNull(this IDataRecord record, string name)
        {
            return record.GetInt16WithNull(record.GetOrdinal(name));
        }


        public static int GetInt32(this IDataRecord record, string name)
        {
            return record.GetInt32(record.GetOrdinal(name));
        }

        public static int? GetInt32WithNull(this IDataRecord record, int index)
        {
            return record.IsDBNull(index) ? (int?)null : record.GetInt32(index);
        }

        public static int? GetInt32WithNull(this IDataRecord record, string name)
        {
            return record.GetInt32WithNull(record.GetOrdinal(name));
        }


        public static long GetInt64(this IDataRecord record, string name)
        {
            return record.GetInt64(record.GetOrdinal(name));
        }

        public static long? GetInt64WithNull(this IDataRecord record, int index)
        {
            return record.IsDBNull(index) ? (long?)null : record.GetInt64(index);
        }

        public static long? GetInt64WithNull(this IDataRecord record, string name)
        {
            return record.GetInt64WithNull(record.GetOrdinal(name));
        }
    }
}

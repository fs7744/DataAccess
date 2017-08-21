using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Vic.Data
{
    public class ListDataReader<T> : DbDataReader
    {
        protected IEnumerator<T> _Data;
        protected Dictionary<string, int> _NameIndexs = new Dictionary<string, int>();
        protected List<PropertyInfo> _PropertyInfos = new List<PropertyInfo>();
        protected List<Func<T, dynamic>> _Getters = new List<Func<T, dynamic>>();

        public ListDataReader(IEnumerable<T> data)
        {
            _Data = data.GetEnumerator();
            var type = typeof(T);
            var properties = TypeExtensions.GetProperties(type,
                    BindingFlags.Instance |
                    BindingFlags.Public).Where(i => i.CanRead);
            var index = 0;
            foreach (var property in properties)
            {
                _NameIndexs.Add(property.Name, index++);
                _PropertyInfos.Add(property);
                var v = Expression.Parameter(type, "v");
                var func = Expression.Lambda<Func<T, dynamic>>(Expression.Convert(Expression.Property(v, property), typeof(object)), v).Compile();
                _Getters.Add(func);
            }
        }

        public override object this[string name]
        {
            get
            {
                return GetDynamicValue(_NameIndexs[name]);
            }
        }

        public override object this[int ordinal]
        {
            get
            {
                return GetDynamicValue(ordinal);
            }
        }

        public override int Depth
        {
            get
            {
                return 0;
            }
        }

        public override int FieldCount
        {
            get
            {
                return _Getters.Count;
            }
        }

        public override bool HasRows
        {
            get
            {
                return true;
            }
        }

        public override bool IsClosed
        {
            get
            {
                return true;
            }
        }

        public override int RecordsAffected
        {
            get
            {
                return 0;
            }
        }

        public override bool GetBoolean(int ordinal)
        {
            return GetDynamicValue(ordinal);
        }

        public override byte GetByte(int ordinal)
        {
            return GetDynamicValue(ordinal);
        }

        public override long GetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length)
        {
            throw new NotSupportedException();
        }

        public override char GetChar(int ordinal)
        {
            return GetDynamicValue(ordinal);
        }

        public override long GetChars(int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length)
        {
            throw new NotSupportedException();
        }

        public override string GetDataTypeName(int ordinal)
        {
            return _PropertyInfos[ordinal].PropertyType.Name;
        }

        public override DateTime GetDateTime(int ordinal)
        {
            return GetDynamicValue(ordinal);
        }

        public override decimal GetDecimal(int ordinal)
        {
            return GetDynamicValue(ordinal);
        }

        public override double GetDouble(int ordinal)
        {
            return GetDynamicValue(ordinal);
        }

        public override IEnumerator GetEnumerator()
        {
            return _Data;
        }

        public override Type GetFieldType(int ordinal)
        {
            return _PropertyInfos[ordinal].PropertyType;
        }

        public override float GetFloat(int ordinal)
        {
            return GetDynamicValue(ordinal);
        }

        public override Guid GetGuid(int ordinal)
        {
            return GetDynamicValue(ordinal);
        }

        public override short GetInt16(int ordinal)
        {
            return GetDynamicValue(ordinal);
        }

        public override int GetInt32(int ordinal)
        {
            return GetDynamicValue(ordinal);
        }

        public override long GetInt64(int ordinal)
        {
            return GetDynamicValue(ordinal);
        }

        public override string GetName(int ordinal)
        {
            return _PropertyInfos[ordinal].Name;
        }

        public override int GetOrdinal(string name)
        {
            return _NameIndexs[name];
        }

        public override string GetString(int ordinal)
        {
            return GetDynamicValue(ordinal);
        }

        public override object GetValue(int ordinal)
        {
            return _Getters[ordinal](_Data.Current);
        }

        public override int GetValues(object[] values)
        {
            int min = Math.Min(FieldCount, values.Length);
            for (int i = 0; i < min; i++)
            {
                values[i] = _Getters[i](_Data.Current);
            }
            return min;
        }

        public override bool IsDBNull(int ordinal)
        {
            return GetDynamicValue(ordinal) == null;
        }

        public override bool NextResult()
        {
            return false;
        }

        public override bool Read()
        {
            return _Data.MoveNext();
        }

        private dynamic GetDynamicValue(int ordinal)
        {
            return _Getters[ordinal](_Data.Current);
        }
    }
}
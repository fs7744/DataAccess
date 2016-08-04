using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace VIC.DataAccess.Core
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
                    BindingFlags.Public |
                    BindingFlags.DeclaredOnly).Where(i => i.CanRead);
            var index = 0;
            foreach (var property in properties)
            {
                _NameIndexs.Add(property.Name, index++);
                _PropertyInfos.Add(property);
                var v = Expression.Parameter(type, "v");
                var func = Expression.Lambda<Func<T, dynamic>>(Expression.Property(v, property), v).Compile();
                _Getters.Add(func);
            }
        }

        public override object this[string name]
        {
            get
            {
                return _Getters[_NameIndexs[name]](_Data.Current);
            }
        }

        public override object this[int ordinal]
        {
            get
            {
                return _Getters[ordinal](_Data.Current);
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
            return _Getters[ordinal](_Data.Current);
        }

        public override byte GetByte(int ordinal)
        {
            return _Getters[ordinal](_Data.Current);
        }

        public override long GetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length)
        {
            return _Getters[ordinal](_Data.Current);
        }

        public override char GetChar(int ordinal)
        {
            return _Getters[ordinal](_Data.Current);
        }

        public override long GetChars(int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length)
        {
            return _Getters[ordinal](_Data.Current);
        }

        public override string GetDataTypeName(int ordinal)
        {
            return _PropertyInfos[ordinal].PropertyType.Name;
        }

        public override DateTime GetDateTime(int ordinal)
        {
            return _Getters[ordinal](_Data.Current);
        }

        public override decimal GetDecimal(int ordinal)
        {
            return _Getters[ordinal](_Data.Current);
        }

        public override double GetDouble(int ordinal)
        {
            return _Getters[ordinal](_Data.Current);
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
            return _Getters[ordinal](_Data.Current);
        }

        public override Guid GetGuid(int ordinal)
        {
            return _Getters[ordinal](_Data.Current);
        }

        public override short GetInt16(int ordinal)
        {
            return _Getters[ordinal](_Data.Current);
        }

        public override int GetInt32(int ordinal)
        {
            return _Getters[ordinal](_Data.Current);
        }

        public override long GetInt64(int ordinal)
        {
            return _Getters[ordinal](_Data.Current);
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
            return _Getters[ordinal](_Data.Current);
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
            return _Getters[ordinal](_Data.Current) == null;
        }

        public override bool NextResult()
        {
            return false;
        }

        public override bool Read()
        {
            return _Data.MoveNext();
        }
    }
}
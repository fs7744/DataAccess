using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq.Expressions;
using System.Reflection;

namespace VIC.DataAccess.Core
{
    public class BulkCopyDataReader<T> : DbDataReader
    {
        private IEnumerator<T> _Data;
        private Dictionary<string, Tuple<int, PropertyInfo>> _NameIndexs = new Dictionary<string, Tuple<int, PropertyInfo>>();
        private List<Tuple<Func<T, dynamic>, PropertyInfo>> _Getters = new List<Tuple<Func<T, dynamic>, PropertyInfo>>();

        public BulkCopyDataReader(List<T> data)
        {
            _Data = data.GetEnumerator();
            var type = typeof(T);
            var properties = type.GetTypeInfo().GetProperties(
                    BindingFlags.GetProperty |
                    BindingFlags.Instance |
                    BindingFlags.Public |
                    BindingFlags.DeclaredOnly);
            var index = 0;
            foreach (var property in properties)
            {
                _NameIndexs.Add(property.Name, Tuple.Create(index++, property));
                var v = Expression.Parameter(type, "v");
                var func = Expression.Lambda<Func<T, dynamic>>(Expression.Property(v, property), v).Compile();
                _Getters.Add(Tuple.Create(func, property));
                ColumnMappings.Add(new SqlBulkCopyColumnMapping(property.Name, property.Name));
            }
        }

        public override object this[string name]
        {
            get
            {
                return _Getters[_NameIndexs[name].Item1].Item1(_Data.Current);
            }
        }

        public override object this[int ordinal]
        {
            get
            {
                return _Getters[ordinal].Item1(_Data.Current);
            }
        }

        public List<SqlBulkCopyColumnMapping> ColumnMappings { get; private set; } = new List<SqlBulkCopyColumnMapping>();

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
            return _Getters[ordinal].Item1(_Data.Current);
        }

        public override byte GetByte(int ordinal)
        {
            return _Getters[ordinal].Item1(_Data.Current);
        }

        public override long GetBytes(int ordinal, long dataOffset, byte[] buffer, int bufferOffset, int length)
        {
            return _Getters[ordinal].Item1(_Data.Current);
        }

        public override char GetChar(int ordinal)
        {
            return _Getters[ordinal].Item1(_Data.Current);
        }

        public override long GetChars(int ordinal, long dataOffset, char[] buffer, int bufferOffset, int length)
        {
            return _Getters[ordinal].Item1(_Data.Current);
        }

        public override string GetDataTypeName(int ordinal)
        {
            return _Getters[ordinal].Item2.PropertyType.Name;
        }

        public override DateTime GetDateTime(int ordinal)
        {
            return _Getters[ordinal].Item1(_Data.Current);
        }

        public override decimal GetDecimal(int ordinal)
        {
            return _Getters[ordinal].Item1(_Data.Current);
        }

        public override double GetDouble(int ordinal)
        {
            return _Getters[ordinal].Item1(_Data.Current);
        }

        public override IEnumerator GetEnumerator()
        {
            return _Data;
        }

        public override Type GetFieldType(int ordinal)
        {
            return _Getters[ordinal].Item2.PropertyType;
        }

        public override float GetFloat(int ordinal)
        {
            return _Getters[ordinal].Item1(_Data.Current);
        }

        public override Guid GetGuid(int ordinal)
        {
            return _Getters[ordinal].Item1(_Data.Current);
        }

        public override short GetInt16(int ordinal)
        {
            return _Getters[ordinal].Item1(_Data.Current);
        }

        public override int GetInt32(int ordinal)
        {
            return _Getters[ordinal].Item1(_Data.Current);
        }

        public override long GetInt64(int ordinal)
        {
            return _Getters[ordinal].Item1(_Data.Current);
        }

        public override string GetName(int ordinal)
        {
            return _Getters[ordinal].Item2.Name;
        }

        public override int GetOrdinal(string name)
        {
            return _NameIndexs[name].Item1;
        }

        public override string GetString(int ordinal)
        {
            return _Getters[ordinal].Item1(_Data.Current);
        }

        public override object GetValue(int ordinal)
        {
            return _Getters[ordinal].Item1(_Data.Current);
        }

        public override int GetValues(object[] values)
        {
            int min = Math.Min(FieldCount, values.Length);
            for (int i = 0; i < min; i++)
            {
                values[i] = _Getters[i].Item1(_Data.Current);
            }
            return min;
        }

        public override bool IsDBNull(int ordinal)
        {
            return _Getters[ordinal].Item1(_Data.Current) == null;
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
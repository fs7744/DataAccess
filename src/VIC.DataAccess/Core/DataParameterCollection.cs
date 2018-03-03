using System.Collections.Generic;
using System.Data.Common;
using System.Linq;

namespace VIC.DataAccess.Core
{
    public class DataParameterCollection
    {
        private Dictionary<string, DataParameter> _Params = new Dictionary<string, DataParameter>();

        public DataParameterCollection()
        {
        }

        public DataParameter this[string parameterName]
        {
            get
            {
                return _Params[parameterName];
            }

            set
            {
                _Params[parameterName] = value;
            }
        }

        public int Count
        {
            get
            {
                return _Params.Count;
            }
        }

        public void Add(DataParameter item)
        {
            _Params.Add(item.ParameterName, item);
        }

        public void Clear()
        {
            _Params.Clear();
        }

        public bool Contains(string parameterName)
        {
            return _Params.ContainsKey(parameterName);
        }

        public bool Contains(DataParameter item)
        {
            return _Params.ContainsValue(item);
        }

        public bool Remove(string parameterName)
        {
            return _Params.Remove(parameterName);
        }

        public IEnumerable<DataParameter> GetParams()
        {
            return _Params.Values;
        }

        public void SetSpecialParameters(List<DbParameter> paramList)
        {
            var sps = this;
            if (sps.Count == 0) return;
            foreach (var sp in sps.GetParams().Where(j => j != null))
            {
                var i = paramList.FirstOrDefault(j => j.ParameterName == sp.ParameterName);
                if (i != null)
                {
                    i.DbType = sp.DbType;
                    i.Size = sp.Size;
                    i.IsNullable = sp.IsNullable;
                    i.Direction = sp.Direction;
                    i.Precision = sp.Precision;
                    i.Scale = sp.Scale;
                }
            }
        }
    }
}
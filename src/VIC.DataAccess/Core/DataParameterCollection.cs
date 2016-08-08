using System.Collections.Generic;

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
    }
}
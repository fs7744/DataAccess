using System.Data;

namespace VIC.DataAccess
{
    public class DbParameter
    {
        public const string ParameterNamePrefix = "@";

        public SqlDbType DbType { get; set; }

        public ParameterDirection Direction { get; set; } = ParameterDirection.Input;

        public bool IsNullable { get; set; } = true;

        private string _ParameterName;

        public string ParameterName
        {
            get { return _ParameterName; }
            set
            {
                _ParameterName = value != null
                    && !value.StartsWith(ParameterNamePrefix)
                    ? ParameterNamePrefix + value
                    : value;
            }
        }

        public int Size { get; set; }
    }
}
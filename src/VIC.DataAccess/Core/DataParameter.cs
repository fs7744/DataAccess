using System.Data;

namespace VIC.DataAccess
{
    public class DataParameter
    {
        public const string ParameterNamePrefix = "@";

        public DbType DbType { get; set; }

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

        public byte Precision { get; set; }

        public byte Scale { get; set; }

        public int Size { get; set; }
    }
}
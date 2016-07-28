using System.Data;

namespace VIC.DataAccess
{
    public class DbParameter
    {
        public DbType DbType { get; set; }

        public ParameterDirection Direction { get; set; }

        public bool IsNullable { get; set; }

        public string ParameterName { get; set; }
    }
}
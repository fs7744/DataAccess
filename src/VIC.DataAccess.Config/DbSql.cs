using System.Collections.Generic;
using System.Data;

namespace VIC.DataAccess.Config
{
    public class DbSql
    {
        public string SqlName { get; set; }

        public string ConnectionName { get; set; }

        internal string ConnectionString { get; set; }

        public string Sql { get; set; }

        public CommandType Type { get; set; }

        public Dictionary<string, DataParameter> SpecialParameters { get; set; }
    }
}
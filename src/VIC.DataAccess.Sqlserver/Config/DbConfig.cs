using System.Collections.Generic;

namespace VIC.DataAccess
{
    public class DbConfig
    {
        public IDictionary<string, string> ConnectionStrings { get; set; }

        public IDictionary<string, DbSql> SqlConfigs { get; set; }
    }
}
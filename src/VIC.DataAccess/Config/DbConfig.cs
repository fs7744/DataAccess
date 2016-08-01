using System.Collections.Generic;

namespace VIC.DataAccess
{
    public class DbConfig
    {
        public Dictionary<string, string> ConnectionStrings { get; set; }

        public Dictionary<string, DbSql> SqlConfigs { get; set; }
    }
}
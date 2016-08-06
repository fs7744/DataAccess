using System.Collections.Generic;

namespace VIC.DataAccess.Config
{
    public class DbConfig
    {
        public List<DbConnection> ConnectionStrings { get; set; }

        public List<DbSql> SqlConfigs { get; set; }

        internal new Dictionary<string, DbSql> Sqls { get; set; }
    }
}
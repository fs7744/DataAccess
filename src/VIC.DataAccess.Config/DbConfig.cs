using System.Collections.Generic;

namespace VIC.DataAccess.Config
{
    public class DbConfig
    {
        public List<DataConnection> ConnectionStrings { get; set; }

        public List<DbSql> SqlConfigs { get; set; }

        internal Dictionary<string, DbSql> Sqls { get; set; }
    }
}
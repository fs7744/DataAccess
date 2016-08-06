using System.Collections.Generic;
using System.Data;

namespace VIC.DataAccess.Config
{
    public class DbSql
    {
        public string CommandName { get; set; }

        internal string ConnectionString { get; set; }

        public string Text { get; set; }

        public CommandType Type { get; set; }

        public List<DataParameter> PreParameters { get; set; }

        public int Timeout { get; internal set; }

        public string ConnectionName { get; internal set; }
    }
}
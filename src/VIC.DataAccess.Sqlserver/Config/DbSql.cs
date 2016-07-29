using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace VIC.DataAccess
{
    public class DbSql
    {
        public string SqlName { get; set; }

        public string ConnectionName { get; set; }

        internal string ConnectionString { get; set; }

        public string Sql { get; set; }

        public CommandType Type { get; set; }

        public Dictionary<string, DbParameter> SpecialParameters { get; set; }

        internal ConcurrentDictionary<Type, Func<dynamic, List<SqlParameter>>> ParamConverters
            = new ConcurrentDictionary<Type, Func<dynamic, List<SqlParameter>>>();

        internal ConcurrentDictionary<Type, Func<DbDataReader, dynamic>> ReaderConverters
            = new ConcurrentDictionary<Type, Func<DbDataReader, dynamic>>();
    }
}
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using VIC.DataAccess.Core;

namespace VIC.DataAccess.MSSql.Core
{
    public class BulkCopyDataReader<T> : ListDataReader<T>
    {
        public BulkCopyDataReader(List<T> data) : base(data)
        {
            ColumnMappings.AddRange(_PropertyInfos.Select(i => new SqlBulkCopyColumnMapping(i.Name, i.Name)));
        }

        public List<SqlBulkCopyColumnMapping> ColumnMappings { get; private set; } = new List<SqlBulkCopyColumnMapping>();
    }
}
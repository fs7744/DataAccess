using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace VIC.DataAccess.Core
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
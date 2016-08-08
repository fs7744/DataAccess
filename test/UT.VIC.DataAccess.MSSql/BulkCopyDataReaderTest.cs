using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using VIC.DataAccess.MSSql.Core;
using Xunit;

namespace UT.VIC.DataAccess.MSSql
{
    public class Student
    {
        public int Age { get; set; }

        public string Name { get; set; }
    }

    public class BulkCopyDataReaderTest
    {
        [Fact]
        public void TestColumnMappings()
        {
            var reader = new BulkCopyDataReader<Student>(new List<Student>());
            Assert.Equal(2, reader.ColumnMappings.Count);
            var ps = TypeExtensions.GetProperties(typeof(Student),
                    BindingFlags.Instance |
                    BindingFlags.Public).ToList();
            for (int i = 0; i < ps.Count; i++)
            {
                Assert.Equal(ps[i].Name, reader.ColumnMappings[i].SourceColumn);
                Assert.Equal(ps[i].Name, reader.ColumnMappings[i].DestinationColumn);
            }
        }
    }
}
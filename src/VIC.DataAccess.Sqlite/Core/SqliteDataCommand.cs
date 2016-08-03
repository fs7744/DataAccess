using Microsoft.Data.Sqlite;
using System.Data.Common;
using VIC.DataAccess.Abstraction;
using VIC.DataAccess.Core;

namespace VIC.DataAccess.SQLite.Core
{
    public class SQLiteDataCommand : DataCommand
    {
        public SQLiteDataCommand(IParamConverter pc, IScalarConverter sc, IEntityConverter ec) : base(pc, sc, ec)
        {
        }

        protected override DbConnection CreateConnection(string connectionString)
        {
            return new SqliteConnection(connectionString);
        }
    }
}
using Npgsql;
using System.Data.Common;
using VIC.DataAccess.Abstraction;
using VIC.DataAccess.Core;

namespace VIC.DataAccess.PostgreSQL.Core
{
    public class PostgreSQLDataCommand : DataCommand
    {
        public PostgreSQLDataCommand(IParamConverter pc, IScalarConverter sc, IEntityConverter ec) : base(pc, sc, ec)
        {
        }

        protected override DbConnection CreateConnection(string connectionString)
        {
            return new NpgsqlConnection(connectionString);
        }
    }
}
using Npgsql;
using System.Data.Common;
using VIC.DataAccess.Abstraction.Converter;
using VIC.DataAccess.Core;

namespace VIC.DataAccess.PostgreSQL.Core
{
    public class PostgreSQLDataCommand : DataCommand
    {
        public PostgreSQLDataCommand(IParamConverter pc, IScalarConverter sc, IEntityConverter ec) : base(pc, sc, ec, null)
        {
        }

        protected override DbConnection CreateConnection(string connectionString)
        {
            return new NpgsqlConnection(connectionString);
        }
    }
}
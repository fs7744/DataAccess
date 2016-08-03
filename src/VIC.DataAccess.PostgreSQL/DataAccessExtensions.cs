using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using VIC.DataAccess.Abstraction;
using VIC.DataAccess.Core;
using VIC.DataAccess.PostgreSQL.Core;

namespace VIC.DataAccess
{
    public static class DataAccessExtensions
    {
        public static IServiceCollection UseDataAccess(this IServiceCollection service)
        {
            TypeHelper.SqlParameterType = typeof(NpgsqlParameter);
            return service.AddTransient<IDataCommand, PostgreSQLDataCommand>();
        }
    }
}
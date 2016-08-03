using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using VIC.DataAccess.Abstraction;
using VIC.DataAccess.Core;
using VIC.DataAccess.SQLite.Core;

namespace VIC.DataAccess
{
    public static class DataAccessExtensions
    {
        public static IServiceCollection UseDataAccess(this IServiceCollection service)
        {
            TypeHelper.SqlParameterType = typeof(SqliteParameter);
            return service.AddTransient<IDataCommand, SQLiteDataCommand>();
        }
    }
}
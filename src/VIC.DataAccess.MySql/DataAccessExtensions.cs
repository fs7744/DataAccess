using Microsoft.Extensions.DependencyInjection;
using Pomelo.Data.MySql;
using VIC.DataAccess.Abstraction;
using VIC.DataAccess.Core;
using VIC.DataAccess.MySql.Core;

namespace VIC.DataAccess
{
    public static class DataAccessExtensions
    {
        public static IServiceCollection UseDataAccess(this IServiceCollection service)
        {
            TypeHelper.SqlParameterType = typeof(MySqlParameter);
            return service.AddTransient<IDataCommand, MySqlDataCommand>();
        }
    }
}
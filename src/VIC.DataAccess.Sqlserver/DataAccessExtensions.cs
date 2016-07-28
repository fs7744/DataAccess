using Microsoft.Extensions.DependencyInjection;
using VIC.DataAccess.Abstratiion;
using VIC.DataAccess.Core;

namespace VIC.DataAccess
{
    public static class DataAccessExtensions
    {
        public static IServiceCollection UseDataAccess(this IServiceCollection service, DbConfig config)
        {
            service.AddSingleton<IDbManager>(new SqlDbManager(config));
            return service;
        }
    }
}
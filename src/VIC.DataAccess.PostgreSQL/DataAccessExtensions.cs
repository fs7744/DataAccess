using Microsoft.Extensions.DependencyInjection;
using VIC.DataAccess.Abstraction;
using VIC.DataAccess.Abstraction.Converter;
using VIC.DataAccess.Core.Converter;
using VIC.DataAccess.PostgreSQL.Core;
using VIC.DataAccess.PostgreSQL.Core.Converter;

namespace VIC.DataAccess.PostgreSQL
{
    public static class DataAccessExtensions
    {
        public static IServiceCollection UseDataAccess(this IServiceCollection service)
        {
            return service.AddSingleton<IDbFuncNameConverter, DbFuncNameConverter>()
                .AddSingleton<IDbTypeConverter, DbTypeConverter>()
                .AddSingleton<IScalarConverter, ScalarConverter>()
                .AddSingleton<IEntityConverter, EntityConverter>()
                .AddSingleton<IParamConverter, PostgreSQLParamConverter>()
                .AddTransient<IDataCommand, PostgreSQLDataCommand>();
        }
    }
}
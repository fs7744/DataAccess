using Microsoft.Extensions.DependencyInjection;
using VIC.DataAccess.Abstraction;
using VIC.DataAccess.Abstraction.Converter;
using VIC.DataAccess.Core.Converter;
using VIC.DataAccess.MySql.Core;
using VIC.DataAccess.MySql.Core.Converter;

namespace VIC.DataAccess.MySql
{
    public static class DataAccessExtensions
    {
        public static IServiceCollection UseDataAccess(this IServiceCollection service)
        {
            return service.AddSingleton<IDbFuncNameConverter, DbFuncNameConverter>()
                .AddSingleton<IDbTypeConverter, DbTypeConverter>()
                .AddSingleton<IScalarConverter, ScalarConverter>()
                .AddSingleton<IEntityConverter, EntityConverter>()
                .AddSingleton<IParamConverter, MySqlParamConverter>()
                .AddTransient<IDataCommand, MySqlDataCommand>();
        }
    }
}
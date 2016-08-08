using Microsoft.Extensions.DependencyInjection;
using VIC.DataAccess.Abstraction;
using VIC.DataAccess.Abstraction.Converter;
using VIC.DataAccess.Core.Converter;
using VIC.DataAccess.SQLite.Core;
using VIC.DataAccess.SQLite.Core.Converter;

namespace VIC.DataAccess.SQLite
{
    public static class DataAccessExtensions
    {
        public static IServiceCollection UseDataAccess(this IServiceCollection service)
        {
            return service.AddSingleton<IDbFuncNameConverter, DbFuncNameConverter>()
                .AddSingleton<IDbTypeConverter, DbTypeConverter>()
                .AddSingleton<IScalarConverter, ScalarConverter>()
                .AddSingleton<IEntityConverter, EntityConverter>()
                .AddSingleton<IParamConverter, SQLiteParamConverter>()
                .AddTransient<IDataCommand, SQLiteDataCommand>();
        }
    }
}
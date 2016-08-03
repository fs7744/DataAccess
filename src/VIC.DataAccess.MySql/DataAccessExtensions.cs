using Microsoft.Extensions.DependencyInjection;
using Pomelo.Data.MySql;
using VIC.DataAccess.Abstraction;
using VIC.DataAccess.Abstraction.Converter;
using VIC.DataAccess.Core;
using VIC.DataAccess.Core.Converter;
using VIC.DataAccess.MySql.Core;

namespace VIC.DataAccess
{
    public static class DataAccessExtensions
    {
        public static IServiceCollection UseDataAccess(this IServiceCollection service)
        {
            TypeHelper.SqlParameterType = typeof(MySqlParameter);
            return service.AddSingleton<IDbFuncNameConverter, DbFuncNameConverter>()
                .AddSingleton<IDbTypeConverter, DbTypeConverter>()
                .AddSingleton<IScalarConverter, ScalarConverter>()
                .AddSingleton<IEntityConverter, EntityConverter>()
                .AddSingleton<IParamConverter, ParamConverter>()
                .AddTransient<IDataCommand, MySqlDataCommand>();
        }
    }
}
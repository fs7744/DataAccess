using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using VIC.DataAccess.Abstraction;
using VIC.DataAccess.Abstraction.Converter;
using VIC.DataAccess.Core;
using VIC.DataAccess.Core.Converter;
using VIC.DataAccess.PostgreSQL.Core;

namespace VIC.DataAccess
{
    public static class DataAccessExtensions
    {
        public static IServiceCollection UseDataAccess(this IServiceCollection service)
        {
            TypeHelper.SqlParameterType = typeof(NpgsqlParameter);
            return service.AddSingleton<IDbFuncNameConverter, DbFuncNameConverter>()
                .AddSingleton<IDbTypeConverter, DbTypeConverter>()
                .AddSingleton<IScalarConverter, ScalarConverter>()
                .AddSingleton<IEntityConverter, EntityConverter>()
                .AddSingleton<IParamConverter, ParamConverter>()
                .AddTransient<IDataCommand, PostgreSQLDataCommand>();
        }
    }
}
using Microsoft.Extensions.DependencyInjection;
using VIC.DataAccess.Abstraction;
using VIC.DataAccess.Core;

namespace VIC.DataAccess
{
    public static partial class DataAccessExtensions
    {
        private static IServiceCollection InitConverter(IServiceCollection service)
        {
            service.AddSingleton<IDbFuncNameConverter, DbFuncNameConverter>();
            service.AddSingleton<IDbTypeConverter, DbTypeConverter>();
            service.AddSingleton<IScalarConverter, ScalarConverter>();
            service.AddSingleton<IEntityConverter, EntityConverter>();
            service.AddSingleton<IParamConverter, ParamConverter>();
            return service;
        }
    }
}
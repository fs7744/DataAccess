using AspectCore.Extensions.DependencyInjection;
using AspectCore.Injector;
using Microsoft.Extensions.DependencyInjection;
using VIC.DataAccess.Aop;
using VIC.DataAccess.zipkin.zipkin;

namespace VIC.DataAccess.Extensions
{
    public static class ZipkinExtensions
    {
        public static IServiceContainer AddZipkin(this IServiceCollection serviceContainer)
        {
            return serviceContainer
                .AddSingleton<IDataAccessTrace, DataAccessZipkinTrace>()
                .ToServiceContainer()
                .AddDataAccessAop();
        }
    }
}
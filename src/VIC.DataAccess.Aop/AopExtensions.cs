using AspectCore.Configuration;
using AspectCore.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using VIC.DataAccess.Aop;

namespace VIC.DataAccess.Extensions
{
    public static class AopExtensions
    {
        public static IServiceCollection AddDataAccessAop(this IServiceCollection serviceContainer)
        {
            return serviceContainer.AddDynamicProxy(config =>
            {
                config.Interceptors.AddTyped<DataAccessInterceptor>(Predicates.ForNameSpace("VIC.DataAccess*"), Predicates.ForMethod("Execute*"));
            });
        }
    }
}
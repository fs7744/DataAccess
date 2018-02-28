using AspectCore.Configuration;
using AspectCore.Injector;
using VIC.DataAccess.Aop;

namespace VIC.DataAccess.Extensions
{
    public static class AopExtensions
    {
        public static IServiceContainer AddDataAccessAop(this IServiceContainer serviceContainer)
        {
            return serviceContainer.Configure(config =>
            {
                config.Interceptors.AddTyped<DataAccessInterceptor>(Predicates.ForNameSpace("VIC.DataAccess*"), Predicates.ForMethod("Execute*"));
            });
        }
    }
}
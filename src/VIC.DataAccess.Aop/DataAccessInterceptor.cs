using AspectCore.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using VIC.DataAccess.Abstraction;

namespace VIC.DataAccess.Aop
{
    [NonAspect]
    public class DataAccessInterceptor : AbstractInterceptor
    {
        public async override Task Invoke(AspectContext context, AspectDelegate next)
        {
            var trace = context.ServiceProvider.GetService<IDataAccessTrace>();
            if (trace != null && context.Implementation is IDataCommand command)
            {
                var startDateTime = DateTime.Now;
                Exception err = null;
                try
                {
                    await context.Invoke(next);
                }
                catch (Exception ex)
                {
                    err = ex;
                    throw ex;
                }
                finally
                {
                    trace.Record(startDateTime, DateTime.Now, context, err);
                }
            }
            else
            {
                await context.Invoke(next);
            }
        }
    }
}
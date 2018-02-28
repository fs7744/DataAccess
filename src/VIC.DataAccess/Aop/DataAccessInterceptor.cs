using AspectCore.DynamicProxy;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using VIC.DataAccess.Abstraction;

namespace VIC.DataAccess.Aop
{
    [NonAspect]
    public class DataAccessInterceptor : AbstractInterceptor
    {
        private readonly IDataAccessTrace trace;
        private readonly string rn = Environment.NewLine;

        public DataAccessInterceptor(IDataAccessTrace trace)
        {
            this.trace = trace;
        }

        public async override Task Invoke(AspectContext context, AspectDelegate next)
        {
            if (trace != null && context.Implementation is IDataCommand command)
            {
                var stopwatch = Stopwatch.StartNew();
                Exception err = null;
                try
                {
                    await context.Invoke(next);
                }
                catch (Exception ex)
                {
                    err = ex;
                    throw ex;
                    //err = $"Timeout: {command.Timeout},{rn}Exception: {ex.Message},{rn}StackTrace: {ex.StackTrace},{rn}Connection: {command.ConnectionString},{rn}sql: {command.Text},{rn}";
                }
                finally
                {
                    stopwatch.Stop();
                    trace.Record(stopwatch, context, err);
                    //var str = $"Executed method:{context.ServiceMethod.GetReflector().DisplayName},{rn}Elapsed: {stopwatch.Elapsed},{rn}{err}";
                }
            }
            else
            {
                await context.Invoke(next);
            }
        }
    }
}
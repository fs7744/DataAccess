using AspectCore.DynamicProxy;
using AspectCore.Extensions.Reflection;
using Newtonsoft.Json;
using System;
using VIC.DataAccess.Abstraction;
using VIC.DataAccess.Aop;
using zipkin4net;

namespace VIC.DataAccess.Zipkin
{
    public class DataAccessZipkinTrace : IDataAccessTrace
    {
        public void Record(DateTime startDateTime, DateTime endDateTime, AspectContext context, Exception err)
        {
            if (Trace.Current == null) return;
            var trace = Trace.Current.Child();
            trace.Record(Annotations.LocalOperationStart("DataAccess"), startDateTime);
            trace.Record(Annotations.LocalOperationStop(), endDateTime);
            trace.Record(Annotations.Tag("method", context.ServiceMethod.GetReflector().DisplayName));
            trace.Record(Annotations.ServiceName("db"));
            if (err == null) return;
            trace.Record(Annotations.Tag("error", err.ToString()));
            if (context.Implementation is IDataCommand command)
            {
                trace.Record(Annotations.Tag("sql", command.Text));
                trace.Record(Annotations.Tag("connection", command.ConnectionString));
                trace.Record(Annotations.Tag("timeout", command.Timeout.ToString()));
                trace.Record(Annotations.Tag("parameters", JsonConvert.SerializeObject(context.Parameters)));
            }
        }
    }
}
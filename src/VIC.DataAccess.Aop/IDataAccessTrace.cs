using AspectCore.DynamicProxy;
using System;

namespace VIC.DataAccess.Aop
{
    public interface IDataAccessTrace
    {
        void Record(DateTime startDateTime, DateTime endDateTime, AspectContext context, Exception err);
    }
}
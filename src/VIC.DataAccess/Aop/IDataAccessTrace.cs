using System;
using System.Diagnostics;
using AspectCore.DynamicProxy;

namespace VIC.DataAccess.Aop
{
    public interface IDataAccessTrace
    {
        void Record(Stopwatch stopwatch, AspectContext context, Exception err);
    }
}
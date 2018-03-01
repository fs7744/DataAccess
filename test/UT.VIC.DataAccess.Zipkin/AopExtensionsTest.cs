using AspectCore.DynamicProxy;
using AspectCore.Extensions.DependencyInjection;
using AspectCore.Extensions.Reflection;
using AspectCore.Injector;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using VIC.DataAccess.Abstraction;
using VIC.DataAccess.Aop;
using VIC.DataAccess.Extensions;
using Xunit;

namespace UT.VIC.DataAccess.Aop
{
    public class AopExtensionsTest
    {
        private IServiceProvider provider;
        private TestDataAccessTrace testData;

        public AopExtensionsTest()
        {
            var mockDataCommand = new Mock<IDataCommand>();
            mockDataCommand.Setup(i => i.ExecuteScalarListAsync<int>(null))
                .ReturnsAsync(() => new List<int>() { 3 });
            mockDataCommand.Setup(i => i.ExecuteScalarListAsync<int>(3))
                .ThrowsAsync(new Exception("test"));
            mockDataCommand.Setup(i => i.BeginTransaction(IsolationLevel.Chaos))
                .Returns(() => null);

            provider = new ServiceCollection()
                .AddSingleton<IDataAccessTrace, TestDataAccessTrace>()
                .AddSingleton(mockDataCommand.Object)
                .AddDataAccessAop()
                .ToServiceContainer()
                .Build();

            var info = provider.GetRequiredService<IDataAccessTrace>();
            var field = info.GetType().GetReflector().GetMemberInfo().GetField("_implementation", BindingFlags.NonPublic | BindingFlags.Instance);
            testData = field.GetValue(info) as TestDataAccessTrace;
        }

        [Fact]
        public void WhenVICDataAccessThenPredicatesForNameSpaceRight()
        {
            testData.Err = null;
            var command = provider.GetRequiredService<IDataCommand>();
            var result = command.ExecuteScalarListAsync<int>(null).Result;
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(3, result[0]);
            Assert.Null(testData.Err);
        }

        [Fact]
        public void WhenNoVICDataAccessExecuteMethodThenCanNotAop()
        {
            testData.Err = new Exception("r");
            var command = provider.GetRequiredService<IDataCommand>();
            var result = command.BeginTransaction(IsolationLevel.Chaos);
            Assert.Null(result);
            Assert.Null(testData.Err);
        }

        [Fact]
        public void WhenExecuteMethodThrowsExceptionThenAopCatchException()
        {
            testData.Err = null;
            var command = provider.GetRequiredService<IDataCommand>();
            try
            {
                var result = command.ExecuteScalarListAsync<int>(3).Result;
            }
            catch (Exception ex)
            {
                Assert.Contains("test", ex.Message);
            }

            Assert.NotNull(testData.Err);
            Assert.Contains("test", testData.Err.Message);
        }
    }

    [NonAspect]
    public class TestDataAccessTrace : IDataAccessTrace
    {
        public Exception Err { get; set; }

        public void Record(DateTime startDateTime, DateTime endDateTime, AspectContext context, Exception err)
        {
            Err = err;
        }
    }
}
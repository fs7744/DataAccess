using System;
using System.Collections.Generic;
using System.Text;
using Xunit;
using Microsoft.Extensions.DependencyInjection;
using VIC.DataAccess.Abstraction;
using VIC.DataAccess.Core;
using Moq;
using UT.VIC.DataAccess.Core;
using AspectCore.Extensions.DependencyInjection;
using VIC.DataAccess.Extensions;
using AspectCore.Injector;
using System.Threading.Tasks;

namespace UT.VIC.DataAccess.Aop
{
    public class ExtensionsTest
    {
        IServiceProvider provider;

        public ExtensionsTest()
        {
            var mockDataCommand = new Mock<IDataCommand>();
            mockDataCommand.Setup(i => i.ExecuteScalarListAsync<Student>(null))
                .ReturnsAsync(() => new List<Student>());

            provider = new ServiceCollection()
                .AddSingleton<IDataCommand>(mockDataCommand.Object)
                .ToServiceContainer()
                .AddDataAccessAop()
                .Build();
        }

        [Fact]
        public void WhenVICDataAccessThenPredicatesForNameSpaceRight()
        {
            var command = provider.GetRequiredService<IDataCommand>();
            var result = command.ExecuteScalarListAsync<Student>(null).Result;
        }
    }
}

using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VIC.DataAccess.MSSql.Core;
using Xunit;
using VIC.DataAccess.MSSql;
using VIC.DataAccess.Abstraction;

namespace UT.VIC.DataAccess.MSSql
{
    public class DynamicConditionTest
    {
        [Fact]
        public void TestDynamicCondition()
        {
            var sp = new ServiceCollection().UseDataAccess()
                .BuildServiceProvider();

           var command = sp.GetService<IDataCommand>();
            command.Text = "#where#";
            command.Text = 
                command.Text.Where("a".Equal("@d"))
                .And("a".Between("@d", "@D"))
                .And("a".Exists("SELECT TOP 1 1 #wh#".Where("a".Equal("d"),"#wh#").BuildToSubQuery()))
                .And("a".GreaterThan("@d"))
                .And("a".GreaterThanOrEqual("@d"))
                .And("a".In("@d", "@D"))
                .And("a".LessThan("@d"))
                .And("a".LessThanOrEqual("@d"))
                .And("a".Like("@d").Not())
                .Or("a".NotEqual("d".ToDbStr()))
                .Build();
            var sql = @"WHERE a = @d AND a BETWEEN @d AND @D AND a EXISTS (SELECT TOP 1 1 WHERE a = d) AND a > @d AND a >= @d AND a IN (@d,@D) AND a < @d AND a <= @d AND NOT a LIKE @d Or a <> 'd'";
            Assert.Equal(sql, command.Text);

        }
    }
}

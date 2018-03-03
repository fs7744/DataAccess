using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace UT.VIC.DataAccess.MSSql
{
    public class BatchTest
    {
        private static List<Student> _Students = new List<Student>()
        {
            new Student()
            {
                Age = 1,
                Name = "Victor1"
            },
            new Student() { Age = 2, Name = "Victor2" },
            new Student()
            {
                Age = 3,
                Name = "Victor3"
            },
        };

        //[Fact]
        //public void TestExecuteNonQueryList()
        //{
        //    using (var command = new TestDataCommand())
        //    {
        //        command.Text = "update top(1) Name = @Name where Age = @Age";
        //        command.ConnectionString = "sqlConnectionString";
        //        var num = command.ExecuteNonQuerys(_Students);
        //        var com = command.Connection.Command;
        //        Assert.Equal("update top(1) Name = @Name0 where Age = @Age0;update top(1) Name = @Name1 where Age = @Age1;update top(1) Name = @Name2 where Age = @Age2;", com.CommandText);
        //        for (int i = 0; i < _Students.Count; i++)
        //        {
        //            Assert.Equal(_Students[i].Name, com.Parameters[$"@Name{i}"].Value);
        //            Assert.Equal(_Students[i].Age, com.Parameters[$"@Age{i}"].Value);
        //        }
        //    }
        //}

        //[Fact]
        //public async void TestExecuteNonQueryAsyncList()
        //{
        //    using (var command = new TestDataCommand())
        //    {
        //        command.Text = "update top(1) Name = @Name where Age = @Age";
        //        command.ConnectionString = "sqlConnectionString";
        //        var num = await command.ExecuteNonQuerysAsync(_Students);
        //        var com = command.Connection.Command;
        //        Assert.Equal("update top(1) Name = @Name0 where Age = @Age0;update top(1) Name = @Name1 where Age = @Age1;update top(1) Name = @Name2 where Age = @Age2;", com.CommandText);
        //        for (int i = 0; i < _Students.Count; i++)
        //        {
        //            Assert.Equal(_Students[i].Name, com.Parameters[$"@Name{i}"].Value);
        //            Assert.Equal(_Students[i].Age, com.Parameters[$"@Age{i}"].Value);
        //        }
        //    }
        //}
    }
}

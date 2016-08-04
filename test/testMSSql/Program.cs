using System;
using System.Data;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using VIC.DataAccess;
using System.Diagnostics;
using System.Threading.Tasks;
using VIC.DataAccess.Abstraction;

namespace testMSSql
{
    public class SoInfo
    {
        public int TransactionNumber { get; set; }

        public int SoNumber { get; set; }

        public string ItemNumber { get; set; }

        public DateTime SoDate { get; set; }

        public int Quantity { get; set; }

        public decimal SoAmount { get; set; }

        public int ItemGroup { get; set; }

        public int SubCategory { get; set; }

        public int Manufacturer { get; set; }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var sc = new ServiceCollection();
            sc.UseDataAccess();
            Test(sc.BuildServiceProvider()).Wait();
        }

        public static async Task Test(IServiceProvider sc)
        {
            var cs = @"";
            var sql = @"select top 1
	TransactionNumber
	,SoNumber
	,ItemNumber
	,SoDate
	,Quantity
	,so_amount AS SoAmount
	,itemgroup AS ItemGroup
	,subcategory AS SubCategory
	,manufacturer AS Manufacturer
from TempTable.dbo.TEMP_Sales_20160205 with(nolock)
where ItemNumber = @ItemNumber";
            // select 
            // 	TransactionNumber
            // 	,SoNumber
            // 	,ItemNumber
            // 	,SoDate
            // 	,Quantity
            // 	,so_amount AS SoAmount
            // 	,itemgroup AS ItemGroup
            // 	,subcategory AS SubCategory
            // 	,manufacturer AS Manufacturer
            // from TempTable.dbo.TEMP_Sales_20160205 with(nolock)
            // where ItemNumber = @ItemNumber2";


            using (var command = sc.GetService<IDataCommand>())
            {
                command.Type = CommandType.Text;
                command.ConnectionString = cs;
                command.Text = sql;

                // var sw = new Stopwatch();
                // sw.Start();
                // using (var a = await command.ExecuteMultipleAsync(new { ItemNumber = "22-152-420", ItemNumber2 = "81-183-003" }))
                // {
                //     var result = await a.ExecuteEntityListAsync<SoInfo>();
                //     Console.WriteLine("data : {0}", result.Count);
                //     result = await a.ExecuteEntityListAsync<SoInfo>();
                //     Console.WriteLine("data : {0}", result.Count);
                // }
                // sw.Stop();
                // Console.WriteLine("no cache : {0}", sw.ElapsedMilliseconds);

                // sw.Restart();
                // using (var a = await command.ExecuteMultipleAsync(new { ItemNumber = "81-183-003", ItemNumber2 = "22-152-420" }))
                // {
                //     var result = await a.ExecuteEntityListAsync<SoInfo>();
                //     Console.WriteLine("data : {0}", result.Count);
                //     result = await a.ExecuteEntityListAsync<SoInfo>();
                //     Console.WriteLine("data : {0}", result.Count);
                // }
                // sw.Stop();
                // Console.WriteLine("has cache : {0}", sw.ElapsedMilliseconds);
                var sw = new Stopwatch();
                sw.Start();
                var result = await command.ExecuteEntityAsync<SoInfo>(new { ItemNumber = "22-152-420", SoNumber = 133 });
                sw.Stop();
                //Console.WriteLine("data : {0}", result.Count);
                Console.WriteLine("no cache : {0}", sw.ElapsedTicks);
                Console.WriteLine("no cache : {0} ms", sw.ElapsedMilliseconds);

                sw.Restart();
                result = await command.ExecuteEntityAsync<SoInfo>(new { ItemNumber = "81-183-003" });
                sw.Stop();
                //Console.WriteLine("data : {0}", result.Count);
                Console.WriteLine("has cache : {0}", sw.ElapsedTicks);
                Console.WriteLine("has cache : {0} ms", sw.ElapsedMilliseconds);
            }
        }
    }
}
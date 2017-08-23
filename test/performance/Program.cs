using BenchmarkDotNet.Running;

namespace performance
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //new DapperBenchmarks().Dapper();
            BenchmarkRunner.Run<DapperBenchmarks>();
        }
    }
}
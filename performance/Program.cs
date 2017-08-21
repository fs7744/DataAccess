using BenchmarkDotNet.Running;

namespace performance
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            BenchmarkRunner.Run<EntityConverterBenchmarks>();
        }
    }
}
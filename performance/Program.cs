using BenchmarkDotNet.Running;

namespace performance
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            //new EntityConverterBenchmarks().EmitEntityConverterIsDBNull();
            BenchmarkRunner.Run<EntityConverterBenchmarks>();
        }
    }
}
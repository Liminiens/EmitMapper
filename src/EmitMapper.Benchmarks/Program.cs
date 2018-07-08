using System;
using BenchmarkDotNet.Running;

namespace EmitMapper.Benchmarks
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<MapperBenchmark>();
        }
    }
}

using System;
using BenchmarkDotNet.Running;

namespace HLab.Notify.PropertyChanged.Benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<Bench>();
            Console.ReadKey();
        }
    }
}

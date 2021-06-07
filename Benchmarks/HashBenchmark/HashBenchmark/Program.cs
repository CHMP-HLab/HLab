using System;
using System.Runtime.Serialization;
using BenchmarkDotNet.Running;

namespace HashBenchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<BenchNew>();
            Console.ReadKey();
        }
    }
}

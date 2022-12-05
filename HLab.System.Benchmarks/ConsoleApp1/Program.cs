using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace HLab.System.Benchmark
{
    public class StringBenchmark
    {
        public string Result { get; set; }

        public string[] Param = new string[] { "chaine0", "Chaine1", "Chaine2", "Chaine3", "Chaine4" };

        [Benchmark]
        public void ConcatDollar()
        {
            Result = $"{{{Param[0]}:{Param[1]}:{Param[2]}:{Param[3]}:{Param[4]}}}";
        }

        [Benchmark]
        public void ConcatPlus()
        {
            Result = "{" + Param[0] + ":" + Param[1] + ":" + Param[2] + ":" + Param[3] + ":" + Param[4] + "}";
        }

        [Benchmark]
        public void ConcatPlusChar()
        {
            Result = '{' + Param[0] + ':' + Param[1] + ':' + Param[2] + ':' + Param[3] + ':' + Param[4] + '}';
        }

        [Benchmark]
        public void CancatStringBuilder()
        {
            var sb = new StringBuilder();
            sb.Append('{');
            sb.Append(Param[0]);
            sb.Append(':');
            sb.Append(Param[1]);
            sb.Append(':');
            sb.Append(Param[2]);
            sb.Append(':');
            sb.Append(Param[3]);
            sb.Append(':');
            sb.Append(Param[4]);
            sb.Append('}');

            Result = sb.ToString();
        }
    }


    internal class Program
    {
        static void Main(string[] args)
        {
            BenchmarkRunner.Run<StringBenchmark>();
        }
    }
}

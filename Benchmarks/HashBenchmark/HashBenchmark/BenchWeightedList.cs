using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BenchmarkDotNet.Attributes;
using HLab.Base;

namespace HashBenchmark
{

    [/*ClrJob(baseline: true),*/ CoreJob, /*MonoJob, CoreRtJob*/]
    [RPlotExporter, RankColumn]
    public class BenchWeightedList
    {
        private List<BasicClass> _listA = new List<BasicClass>();
        private WeightedList<BasicClass> _listB = new WeightedList<BasicClass>();
        private Random _rnd = new Random();

        [GlobalSetup]
        public void GlobalSetup()
        {
            for (int i = 0; i < 500; i++)
            {
                var a = new BasicClass(i.ToString());
                _listA.Add(a);
                _listB.Add(a);
            }
        }

        [Benchmark]
        public BasicClass List()
        {
            var n = _rnd.Next(0,100).ToString();
            return _listA.First(c => c.Name == n);
        }

        [Benchmark]
        public BasicClass WeightedList()
        {
            var n = _rnd.Next(0,100).ToString();
            return _listB.Get(c => c.Name == n);
        }
    }
}

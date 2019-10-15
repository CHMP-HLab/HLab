using System;
using System.Collections.Generic;
using System.Text;
using BenchmarkDotNet.Attributes;

namespace HLab.Notify.PropertyChanged.Benchmark
{
    [/*ClrJob(baseline: true),*/ CoreJob, /*MonoJob, CoreRtJob*/]
    [RPlotExporter, RankColumn]
    public class Bench
    {
        private NotifyTestClass0 _c0 = new NotifyTestClass0();
        private NotifyTestClass1 _c1 = new NotifyTestClass1();
        private NotifyTestClassA _cA = new NotifyTestClassA();
        private NotifyTestClassB _cB = new NotifyTestClassB();
        private NotifyTestClassC _cC = new NotifyTestClassC();
        private NotifyTestClassD _cD = new NotifyTestClassD();

        [GlobalSetup]
        public void GlobalSetup()
        {
            _c0.Value = 42;
            _c1.Value = 42;
            _cA.Value = 42;
            _cB.Value = 42;
            _cC.Value = 42;
            _cD.Value = 42;
        }
        //[Benchmark(Baseline = true)]
        //public int Get0()
        //{
        //    return _c0.Value;
        //}

        //[Benchmark]
        //public int Get1()
        //{
        //    return _c1.Value;
        //}

        //[Benchmark]
        //public int GetA()
        //{
        //    return _cA.Value;
        //}

        //[Benchmark]
        //public int GetB()
        //{
        //    return _cB.Value;
        //}

        //[Benchmark]
        //public int GetC()
        //{
        //    return _cC.Value;
        //}

        //[Benchmark]
        //public void Set0()
        //{
        //    _c0.Value = 46;
        //}

        [Benchmark(Baseline = true)]
        public void Set1()
        {
            _c1.Value = 46;
        }

        //[Benchmark]
        //public void SetA()
        //{
        //    _cA.Value = 46;
        //}

        //[Benchmark]
        //public void SetB()
        //{
        //    _cB.Value = 46;
        //}
        [Benchmark]
        public void SetC()
        {
            _cC.Value = 46;
            //_cD.Value = 46;
        }
        [Benchmark]
        public void SetB()
        {
            _cD.Value = 46;
        }

    }
}

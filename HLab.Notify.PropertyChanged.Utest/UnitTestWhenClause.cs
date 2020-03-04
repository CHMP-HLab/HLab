using System;
using System.Collections.Generic;
using System.Text;
using HLab.Notify.PropertyChanged.Utest.Annotations;
using Xunit;
using H = HLab.Notify.PropertyChanged.NotifyHelper<HLab.Notify.PropertyChanged.UTest.NotifyObjectWhen>;
namespace HLab.Notify.PropertyChanged.UTest
{
    class NotifyObjectWhen : N<NotifyObjectWhen>
    {
        public object Test
        {
            get => _test.Get();
            set => _test.Set(value);
        }
        private readonly IProperty<object> _test = H.Property<object>();

        public int Test1 => _test1.Get();

        private readonly IProperty<int> _test1 = H.Property<int>(c => c
            .Default(0)
            .On(e => e.Test)
            .NotNull(e => e.Test)
            .Set(e => e.Test1+1)
        );

    }
    class NotifyObjectWhenB : N<NotifyObjectWhenB>
    {
        public object TestA
        {
            get => _testA.Get();
            set => _testA.Set(value);
        }
        private readonly IProperty<object> _testA = H.Property<object>();
        public object TestB
        {
            get => _testB.Get();
            set => _testB.Set(value);
        }
        private readonly IProperty<object> _testB = H.Property<object>();

        public int Test => _test.Get();

        private readonly IProperty<int> _test = H.Property<int>(c => c
            .Default(0)
            .On(e => e.TestA)
            .On(e => e.TestB)
            .NotNull(e => e.TestA)
            .Set(e => e.Test+1)
        );

    }
    public class UnitTestWhenClause
    {
        [Fact]
        public void TestNoConfigurator()
        {

            var obj = new NotifyObjectWhen();

            Assert.Equal(0,obj.Test1);

            obj.Test = new object();

            Assert.Equal(1,obj.Test1);

            obj.Test = null;

            Assert.Equal(1,obj.Test1);
        }
    }

    public class UnitTestWhenClauseB
    {
        [Fact]
        public void TestNoConfigurator()
        {

            var obj = new NotifyObjectWhenB();

            Assert.Equal(0,obj.Test);

            obj.TestA = new object();

            Assert.Equal(1,obj.Test);

            obj.TestA = null;

            Assert.Equal(1,obj.Test);

            obj.TestB = new object();
            obj.TestB = new object();

            Assert.Equal(2,obj.Test);

        }
    }
}
using Xunit;
namespace HLab.Notify.PropertyChanged.UTest
{
    internal class NotifyObjectWhen : NotifierBase
    {
        public NotifyObjectWhen()
        {
            H<NotifyObjectWhen>.Initialize(this);
        }

        public object Test
        {
            get => _test.Get();
            set => _test.Set(value);
        }

        readonly IProperty<object> _test = H<NotifyObjectWhen>.Property<object>();

        public int Test1 => _test1.Get();

        readonly IProperty<int> _test1 = H<NotifyObjectWhen>.Property<int>(c => c
            .Default(0)
            .On(e => e.Test)
            .NotNull(e => e.Test)
            .Set(e => e.Test1+1)
        );

    }

    internal class NotifyObjectWhenB : NotifierBase
    {
        public NotifyObjectWhenB()
        {
            H<NotifyObjectWhenB>.Initialize(this);
        }

        public object TestA
        {
            get => _testA.Get();
            set => _testA.Set(value);
        }

        readonly IProperty<object> _testA = H<NotifyObjectWhenB>.Property<object>();
        public object TestB
        {
            get => _testB.Get();
            set => _testB.Set(value);
        }

        readonly IProperty<object> _testB = H<NotifyObjectWhenB>.Property<object>();

        public int Test => _test.Get();

        readonly IProperty<int> _test = H<NotifyObjectWhenB>.Property<int>(c => c
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
            obj.TestA = new object();

            Assert.Equal(2,obj.Test);

        }
    }
}
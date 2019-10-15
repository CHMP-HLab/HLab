using HLab.DependencyInjection.Annotations;
using HLab.Notify.Annotations;

namespace HLab.Notify.UTests
{
    public partial class UnitTestNotifier
    {
        class TestClass : NotifierObject
        {
//            public TestObjectLevel1 TestObjectLevel1 { get; } = new TestObjectLevel1(new TestNotifierLevel0());

            [Import]
            public TestObjectLevel1 TestObjectLevel1 => N.Get<TestObjectLevel1>();
            public int Result
            {
                get => N.Get<int>(() => 0);
                set => N.Set(value);
            }

            [TriggerOn(nameof(TestObjectLevel1),"TestNotifierLevel0","Value")]
            public void test()
            {
                Result = TestObjectLevel1.TestNotifierLevel0.Value;
            }
        }
    }
}

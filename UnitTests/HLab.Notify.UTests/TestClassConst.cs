using HLab.DependencyInjection.Annotations;
using HLab.Notify.Annotations;

namespace HLab.Notify.UTests
{
    public partial class UnitTestNotifier
    {
        class TestClassConst : NotifierObject
        {
            [Import]
             public TestObjectLevel1 TestObjectLevel1 { get; }

            public int Result {
                get => N.Get<int>(()=>0);
                set => N.Set(value);
            }

            [TriggerOn(nameof(TestObjectLevel1), "TestNotifierLevel0", "Value")]
            public void Trigger()
            {
                Result = TestObjectLevel1.TestNotifierLevel0.Value;
            }
        }
    }
}

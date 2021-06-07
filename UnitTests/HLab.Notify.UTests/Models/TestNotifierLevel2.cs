using HLab.Notify.Annotations;
using HLab.Notify.Triggers;

namespace HLab.Notify.UTests
{
    internal class TestNotifierLevel2 : NotifierObject
    {
        public TestNotifierLevel1 TestNotifierLevel1
        {
            get => this.Get<TestNotifierLevel1>();
            private set => N.Set(value);
        }

        public TestNotifierLevel2(TestNotifierLevel1 b) {  TestNotifierLevel1 = b; }

        public int Value
        {
            get => N.Get(() => -1);
            private set => N.Set(value);
        }

        [TriggerOn(nameof(TestNotifierLevel1), "TestNotifierLevel0", "Value")]
        public void Test()
        {
            Value = TestNotifierLevel1.TestNotifierLevel0.Value;
        }
    }
}
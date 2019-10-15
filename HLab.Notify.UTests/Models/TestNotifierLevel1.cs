using HLab.Notify.Annotations;

namespace HLab.Notify.UTests
{
    internal class TestGetter : NotifierObject
    {
        [TriggerOn(nameof(Source))]
        public int Value => N.Get(() => Source);
        public int Source
        {
            get => N.Get<int>();
            set => N.Set(value);
        } 
    }

    internal class TestNotifierLevel1 : NotifierObject
    {
        public TestNotifierLevel0 TestNotifierLevel0
        {
            get => this.Get<TestNotifierLevel0>();
            private set => N.Set(value);
        }

        public TestNotifierLevel1(TestNotifierLevel0 a) 
        {
            TestNotifierLevel0 = a;
        }

        public int Value
        {
            get => N.Get(() => -1);
            private set => N.Set(value);
        }


        [TriggerOn(nameof(TestNotifierLevel0), "Value")]
        public void Test()
        {
            Value = TestNotifierLevel0.Value;
        }
    }
}
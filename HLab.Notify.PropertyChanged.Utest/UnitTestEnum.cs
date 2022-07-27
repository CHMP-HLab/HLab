using Xunit;

namespace HLab.Notify.PropertyChanged.UTest
{
    internal enum TestEnum
    {
        foo,
        bar,
    }

    internal class TestEnumClass : NotifierTest<TestEnumClass>
    {
        readonly IProperty<TestEnum> _enumValue = H.Property<TestEnum>();

        public TestEnum EnumValue
        {
            get => _enumValue.Get();
            set => _enumValue.Set(value);
        }
    }

    public class UnitTestEnum
    {
        [Fact]
        public void TestEnumValue()
        {
            TestEnumClass c = new TestEnumClass();
            int count = 0;
            c.PropertyChanged += (s, a) => { count++; };

            c.EnumValue = TestEnum.bar;

            Assert.Equal(TestEnum.bar,c.EnumValue);

            c.EnumValue = TestEnum.foo;
            Assert.Equal(TestEnum.foo, c.EnumValue);
            Assert.Equal(2, count);
        }
    }
}

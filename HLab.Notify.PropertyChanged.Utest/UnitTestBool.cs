using Xunit;

namespace HLab.Notify.PropertyChanged.UTest
{
    internal class TestBoolClass : NotifierTest<TestBoolClass>
    {
        readonly IProperty<bool> _boolValue = H.Property<bool>();

        public bool BoolValue
        {
            get => _boolValue.Get();
            set => _boolValue.Set(value);
        }
    }
    public class UnitTestBool
    {
        [Fact]
        public void TestEnumValue()
        {
            TestBoolClass c = new TestBoolClass();
            int count = 0;
            c.PropertyChanged += (s, a) =>
            {
                Assert.Equal("BoolValue",a.PropertyName);
                count++;
            };

            c.BoolValue = true;

            Assert.True(c.BoolValue);

            c.BoolValue = false;
            Assert.False(c.BoolValue);
            Assert.Equal(2, count);
        }
    }
}

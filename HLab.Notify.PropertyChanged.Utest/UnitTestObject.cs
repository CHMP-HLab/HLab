using Xunit;

namespace HLab.Notify.PropertyChanged.UTest
{
    internal class DummyObject
    { }

    internal class TestObjectClass : NotifierTest<TestObjectClass>
    {
        readonly IProperty<DummyObject> _objectValue = H.Property<DummyObject>();
        public DummyObject ObjectValue
        {
            get => _objectValue.Get();
            set => _objectValue.Set(value);
        }
    }
    public class UnitTestObject
    {
        [Fact]
        public void TestObjectValue()
        {
            TestObjectClass c = new TestObjectClass();
            int count = 0;
            c.PropertyChanged += (s, a) =>
            {
                Assert.Equal("ObjectValue", a.PropertyName);
                count++;
            };

            var objA = new DummyObject();
            var objB = new DummyObject();

            c.ObjectValue = objA;

            Assert.Same(objA, c.ObjectValue);

            c.ObjectValue = objB;
            Assert.Same(objB, c.ObjectValue);
            Assert.Equal(2, count);
        }

    }
}

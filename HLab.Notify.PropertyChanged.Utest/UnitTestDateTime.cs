using System;
using Xunit;

namespace HLab.Notify.PropertyChanged.UTest
{
    internal class TestDateTimeClass : NotifierTest<TestDateTimeClass>
    {

        public DateTime DateTimeValue
        {
            get => _dateTimeValue.Get();
            set => _dateTimeValue.Set(value);
        }

        readonly IProperty<DateTime> _dateTimeValue = H.Property<DateTime>();
    }
    public class UnitTestDateTime
    {
        [Fact]
        public void TestDateTimeValue()
        {
            TestDateTimeClass c = new TestDateTimeClass();
            int count = 0;
            c.PropertyChanged += (s, a) =>
            {
                Assert.Equal("DateTimeValue", a.PropertyName);
                count++;
            };

            var dA = DateTime.Now;
            var dB = DateTime.Now;

            c.DateTimeValue = dA;

            Assert.Equal(dA, c.DateTimeValue);

            c.DateTimeValue = dB;
            
            Assert.Equal(dB, c.DateTimeValue);
            Assert.Equal(2, count);
        }
    }
}

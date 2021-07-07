using HLab.Notify.PropertyChanged.UTest.Classes;
using Xunit;

namespace HLab.Notify.PropertyChanged.UTest
{
    public class UnitTest_NotifyCount
    {
        [Fact]
        public void TestNotifyCount()
        {
            var count = 0;
            var fail = 0;
            var test = 0;

            var c = new ClassWithProperty();
            c.PropertyChanged += (sender, args) =>
            {
                switch (args.PropertyName)
                {
                    case "Value":
                        count++;
                        break;
                    default:
                        fail++;
                        break;
                }
                Assert.Equal(test,c.Value);
            };

            test = 42;
            c.Value = test;
            Assert.Equal(1, count);

            test = 43;
            c.Value = test;
            Assert.Equal(2, count);

            test++;
            c.Value++;
            Assert.Equal(3, count);

            Assert.Equal(0,fail);
        }

    }
}

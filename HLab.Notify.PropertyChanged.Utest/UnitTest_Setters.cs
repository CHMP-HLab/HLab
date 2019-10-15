using System;
using System.Collections.Generic;
using System.Text;
using HLab.Notify.PropertyChanged.Utest.Annotations;
using Xunit;
using H = HLab.Notify.PropertyChanged.NotifyHelper<HLab.Notify.PropertyChanged.UTest.NotifyObject>;
namespace HLab.Notify.PropertyChanged.UTest
{
    class NotifyObject
    {
        public NotifyObject()
        {
            H.Initialize(this);
        }

        public int Test
        {
            get => _test.Get();
            set => _test.Set(value);
        }
        private readonly IProperty<int> _test = H.Property<int>();

        public int Test1
        {
            get => _test1.Get();
            set => _test1.Set(value);
        }
        private readonly IProperty<int> _test1 = H.Property<int>(c => c.Default(1));

    }
    public class UnitTest_Setters
    {
        [Fact]
        public void TestNoConfigurator()
        {

            var obj = new NotifyObject();

            Assert.Equal(0,obj.Test);
            Assert.Equal(1,obj.Test1);
        }
    }
}

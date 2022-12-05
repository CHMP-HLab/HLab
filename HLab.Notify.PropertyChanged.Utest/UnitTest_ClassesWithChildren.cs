using System.Collections.ObjectModel;
using System.Linq;
using HLab.Notify.PropertyChanged.UTest.Classes;
using Xunit;

namespace HLab.Notify.PropertyChanged.UTest
{
    public class UnitTestClassesWithChildren
    {
        [Fact]
        public void TestClassWithChild()
        {
            var c = new ClassWithChild
            {
                Child = new ClassWithProperty{Value = 42}
            };

            Assert.Equal(42, c.Value);
        }

        class ClassWithChildren : NotifierTest<ClassWithChildren>
        {
            public ObservableCollection<ClassWithProperty> Children { get; } = new ObservableCollection<ClassWithProperty>();
            public int SumValue => _sumValue.Get();

            readonly IProperty<int> _sumValue = H.Property<int>(c => c
                .On(e => e.Children.Item().Value)
                .Set(e => e.Children.ToList().Sum(a => a.Value))
            );
        }

        [Fact]
        public void TestClassWithChildren()
        {
            var c = new ClassWithChildren();

            Assert.Equal(0, c.SumValue);

            var a = new ClassWithProperty {Value = 11};
            c.Children.Add(a);
            Assert.Equal(11, c.SumValue);


            var b = new ClassWithProperty {Value = 11};
            c.Children.Add(b);
            Assert.Equal(22, c.SumValue);

            a.Value = 21;
            Assert.Equal(32, c.SumValue);
            b.Value = 21;
            Assert.Equal(42, c.SumValue);
        }
    }
}

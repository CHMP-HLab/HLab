using Xunit;

namespace HLab.Notify.PropertyChanged.UTest
{
    public class UnitTest_WeakEvent
    {
        class Child : NotifierBase
        {
            public Child() => H<Child>.Initialize(this);
            ~Child(){}
            public string Value
            {
                get => _value.Get();
                set => _value.Set(value);
            }

            readonly IProperty<string> _value = H<Child>.Property<string>();
        }

        class Parent : NotifierBase
        {
            public Parent() => H<Parent>.Initialize(this);

            public string Value
            {
                get => _value.Get();
                set => _value.Set(value);
            }

            readonly IProperty<string> _value = H<Parent>.Property<string>();
            public Child Child => _child.Get();

            readonly IProperty<Child> _child = H<Parent>.Property<Child>(c => c
                .Set(e => new Child{Value = e.Value})
                .On(e => e.Value).Update()
            );
            public string ChildValue => _childValue.Get();

            readonly IProperty<string> _childValue = H<Parent>.Property<string>(c => c
                .Set(e => e.Child.Value)
                .On(e => e.Child.Value).Update()
            );
        }

        [Fact]
        public void TestWeakReference()
        {
            var parent = new Parent{Value = "42"};

            Assert.Equal("42",parent.Child.Value);
            Assert.Equal("42",parent.ChildValue);

            var child = parent.Child;

            parent.Value = "";
            Assert.Equal("",parent.Child.Value);
            Assert.Equal("",parent.ChildValue);

            child.Value = "10";

            parent.Value = "";
            Assert.Equal("",parent.Child.Value);
            Assert.Equal("",parent.ChildValue);

        }
    }
}

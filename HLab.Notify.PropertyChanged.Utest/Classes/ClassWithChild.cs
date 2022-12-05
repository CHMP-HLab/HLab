namespace HLab.Notify.PropertyChanged.UTest.Classes
{
    internal class ClassWithChild : NotifierTest<ClassWithChild>
    {
        readonly IProperty<ClassWithProperty> _child = H.Property<ClassWithProperty>();
        public ClassWithProperty Child
        {
            get => _child.Get();
            set => _child.Set(value);
        }

        public int Value => _value.Get();

        readonly IProperty<int> _value = H.Property<int>(c => c
                .On(e => e.Child.Value)
                .Set(e => e.Child.Value)
            );
    }
}
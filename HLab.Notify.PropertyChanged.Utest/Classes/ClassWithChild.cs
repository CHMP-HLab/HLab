namespace HLab.Notify.PropertyChanged.UTest.Classes
{
    class ClassWithChild : NotifierTest<ClassWithChild>
    {
        private readonly IProperty<ClassWithProperty> _child = H.Property<ClassWithProperty>();
        public ClassWithProperty Child
        {
            get => _child.Get();
            set => _child.Set(value);
        }

        public int Value => _value.Get();
        private readonly IProperty<int> _value = H.Property<int>(c => c
                .On(e => e.Child.Value)
                .Set(e => e.Child.Value)
            );
    }
}
namespace HLab.Notify.PropertyChanged.UTest.Classes
{
    class ClassWithProperty : NotifierTest<ClassWithProperty>
    {
        private readonly IProperty<int> _value = H.Property<int>();
        public int Value
        {
            get => _value.Get();
            set => _value.Set(value);
        }
    }
}
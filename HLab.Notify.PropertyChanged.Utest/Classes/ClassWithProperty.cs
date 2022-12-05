namespace HLab.Notify.PropertyChanged.UTest.Classes
{
    internal class ClassWithProperty : NotifierTest<ClassWithProperty>
    {
        readonly IProperty<int> _value = H.Property<int>();
        public int Value
        {
            get => _value.Get();
            set => _value.Set(value);
        }
    }
}
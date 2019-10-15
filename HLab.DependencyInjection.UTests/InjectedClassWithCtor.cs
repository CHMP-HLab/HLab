namespace HLab.DependencyInjection.UTests
{
    class InjectedClassWithCtor
    {
        public InjectedClassWithCtor(string value)
        {
            Value = value;
        }

        public string Value { get; }
    }
}
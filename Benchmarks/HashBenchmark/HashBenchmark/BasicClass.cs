namespace HashBenchmark
{
    public class BasicClass
    {
        public BasicClass(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public override string ToString() => Name;
    }
}

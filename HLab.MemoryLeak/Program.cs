using System;
using HLab.Notify.PropertyChanged;

namespace HLab.MemoryLeak
{
    class Program
    {
        static void Main(string[] args)
        {
            var dc = new DummyContainer() { };
            
            while (true)
            {
                Console.WriteLine("any key to create dummy container");
                Console.ReadKey();
                //Create();

                var d1 = new Dummy {Property = "a"};
                var d2 = new Dummy {Property = "b"};

                dc.Dummies.Add(d1);
                    Console.WriteLine("add a dummy : " + dc.Property);
                dc.Dummies.Add(d2);
                    Console.WriteLine("add b dummy : " + dc.Property);

                dc.Dummies.Remove(d1);
                    Console.WriteLine("remove a dummy : " + dc.Property);
                dc.Dummies.Remove(d2);
                    Console.WriteLine("remove b dummy : " + dc.Property);
            }


        }
        private static void Create()
        {
            var dc = new DummyContainer() { };
            var d = new Dummy {Property = "test"};
            dc.Dummies.Add(d);
            Console.WriteLine("there was a dummy : " + d.Property);
        }
    }
}

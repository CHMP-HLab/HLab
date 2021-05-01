using System;
using System.Reflection.Metadata.Ecma335;

namespace HLab.Fluent
{
    using static Extension;
    public interface Fluent<T>
    {
    }
    public interface FluentB<T> : Fluent<T>
    {
    }

    public static class Extension
    {
        public static T Null<T>() => (T)(object)null;


        public static FT Ext<T,FT>(this FT t, T dummy)
            where FT : Fluent<T>
        {
            return t;
        }
    }

    public class Test
    {
        public void A<T>()
        {
            Fluent<T> test;
            test = Null<Fluent<T>>();

            test.Ext(default(T));
        }
    }
}

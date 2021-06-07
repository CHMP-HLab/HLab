using System;
using System.ComponentModel;
using HLab.Notify.Annotations;

namespace HLab.Notify.UTests
{
    internal class TestNotifierLevel0 : NotifierObject
    {
        public int Value
        {
            get => N.Get<int>();
            set => N.Set(value);
        }
    }
}
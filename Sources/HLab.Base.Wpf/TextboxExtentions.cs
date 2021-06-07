using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

using HLab.Base;

namespace HLab.Base.Wpf
{
    public static class TextboxExtentions
    {
        public static void ApplySymbols(this TextBox tb)
        { 
            var pos = tb.CaretIndex;
            var v1 = tb.Text;
            var v2 = v1.ApplySymbols();

            tb.Text = v2;

            try
            {
                tb.CaretIndex = pos - (v1.Length - v2.Length);
            }
            catch(IndexOutOfRangeException){}
            catch(ArgumentOutOfRangeException){}
        }
    }
}

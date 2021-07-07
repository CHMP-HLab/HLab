using System;
using System.Windows.Controls;

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

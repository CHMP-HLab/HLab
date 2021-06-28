using System;
using System.Windows;
using System.Windows.Data;

namespace HLab.Base.Wpf
{
    public static class DependencyExtension
    {
        public static void SetBindingValue<T>(this T sender,DependencyProperty prop,Func<object,object> setter) 
            where T : DependencyObject
        {
            var binding = BindingOperations.GetBindingExpression(sender, prop);
            var inputVal = binding?.Target.GetValue(prop);

            var outputVal = setter(inputVal);

            binding?.Target.SetValue(prop, outputVal);
            binding?.UpdateSource();
        }
    }
}

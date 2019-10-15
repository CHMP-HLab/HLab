using System;
using System.Windows;

namespace HLab.Mvvm.Extentions
{
    public static class ViewModeExtentions
    {
        public static Type GetViewMode(UIElement element)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));
            return (Type)element.GetValue(ViewLocator.ViewModeProperty);
        }
        public static void SetViewMode(UIElement element, Type value)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));
            element.SetValue(ViewLocator.ViewModeProperty, value);
        }
        public static Type GetViewClass(UIElement element)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));
            return (Type)element.GetValue(ViewLocator.ViewClassProperty);
        }
        public static void SetViewClass(UIElement element, Type value)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));
            element.SetValue(ViewLocator.ViewModeProperty, value);
        }
    }
}

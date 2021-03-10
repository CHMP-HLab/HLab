using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace HLab.Base.Wpf
{
    public static class ControlExtensions
    {
        public static IEnumerable<T> FindVisualChildren<T>(this DependencyObject depObj) where T : DependencyObject
        {
            if (depObj == null) yield break;

            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                var child = VisualTreeHelper.GetChild(depObj, i);
                if (child is T childT)
                {
                    yield return childT;
                }

                foreach (var childOfChild in child.FindVisualChildren<T>())
                {
                    yield return childOfChild;
                }
            }
        }
    }
}

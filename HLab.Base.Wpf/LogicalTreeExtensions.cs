using System.Collections.Generic;
using System.Windows;

namespace HLab.Base.Wpf
{
    public static class LogicalTreeExtensions
    {
        public static IEnumerable<T> FindLogicalChildren<T>(this FrameworkElement fe) where T : FrameworkElement
        {
            if (fe == null) yield break;
            foreach (var child in LogicalTreeHelper.GetChildren(fe))
            {
                if (child is T c)
                {
                    yield return c;
                }

                if (child is not FrameworkElement e) continue;
                foreach (var childOfChild in FindLogicalChildren<T>(e))
                {
                    yield return childOfChild;
                }
            }
        }
    }
}

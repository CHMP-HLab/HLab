using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace HLab.Base.Wpf
{
    public static class ControlExtentions
    {
        public static void SetMouseWheel(this FrameworkElement fe)
        {
            foreach (var c in FindVisualChildren<ScrollViewer>(fe))
            {
                if (c is ScrollViewer sc)
                    sc.PreviewMouseWheel += Sc_PreviewMouseWheel;
            }
        }

        private static void Sc_PreviewMouseWheel(object sender, System.Windows.Input.MouseWheelEventArgs e)
        {
            if (sender is ScrollViewer sc)
            {
                sc.ScrollToVerticalOffset(sc.VerticalOffset - e.Delta);
                e.Handled = true;
            }
        }

        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }
    }
}

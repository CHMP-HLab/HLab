using System.Windows;
using HLab.Mvvm.Annotations;

namespace HLab.Mvvm.Wpf.Views
{
    public static class ViewWpfExtensions
    {
        public static Window AsWindow(this IView view)
        {
            if (view is Window win) return win;

            var w = new DefaultWindow
            {
                DataContext = (view as FrameworkElement)?.DataContext,
                Content = view,
                //SizeToContent = SizeToContent.WidthAndHeight,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
            };

            //if(view is FrameworkElement e)
            //{
            //    w.Height = e.Height;
            //    w.Width = e.Width;
            //}

            return w;
        }
        public static Window AsDialog(this IView view)
        {
            if (view is Window win) return win;

            var w = new DefaultWindow
            {
                DataContext = (view as FrameworkElement)?.DataContext,
                Content = view,

                SizeToContent = SizeToContent.WidthAndHeight,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                WindowStyle = WindowStyle.ToolWindow,
                ResizeMode = ResizeMode.NoResize,
            };

            return w;
        }
    }
}

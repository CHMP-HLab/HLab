using System.Windows;
using HLab.Mvvm.Annotations;

namespace HLab.Mvvm.Views
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
        public static TViewModel ViewModel<TViewMode,TViewModel>(this IView<TViewMode,TViewModel> view)
            where TViewMode : ViewMode
        {
            if (view is FrameworkElement fe)
            {
                if (fe.DataContext is TViewModel vm)
                {
                    return vm;
                }
            }

            return default;
        }

        public static bool TryGetViewModel<TViewMode,TViewModel>(this IView<TViewMode,TViewModel> view,out TViewModel viewModel)
            where TViewMode : ViewMode
        {
            if (view is FrameworkElement fe)
            {
                if (fe.DataContext is TViewModel vm)
                {
                    viewModel = vm;
                    return true;
                }
            }

            viewModel = default;
            return false;
        }
    }
}

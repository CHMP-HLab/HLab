using System;
using System.Windows;
using HLab.Mvvm.Annotations;

namespace HLab.Mvvm.Wpf.Views
{
    public static class ViewWpfExtensions
    {
        public static Window AsWindow<T>(this T view) where T : IView
        {
            if (view is FrameworkElement fe) return fe.AsWindow();

            throw new ArgumentException("view should be FrameworkElement");
        }

        public static Window AsWindow(this FrameworkElement view)
        {
            if (view is Window win) return win;

            var w = new DefaultWindow
            {
                DataContext = view?.DataContext,
                View = view,
                //SizeToContent = SizeToContent.WidthAndHeight,
                //WindowStartupLocation = WindowStartupLocation.CenterOwner,
            };

            return w;
        }

        public static Window AsDialog(this IView view)
        {
            if (view is FrameworkElement fe) return fe.AsDialog();

            throw new ArgumentException("view should be FrameworkElement");
        }

        public static Window AsDialog(this FrameworkElement view)
        {
            if (view is Window win) return win;

            var w = new DefaultWindow
            {
                DataContext = view?.DataContext,
                View = view,

                SizeToContent = SizeToContent.WidthAndHeight,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                WindowStyle = WindowStyle.None,
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

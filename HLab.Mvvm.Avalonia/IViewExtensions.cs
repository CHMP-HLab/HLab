using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using HLab.Mvvm.Annotations;

namespace HLab.Mvvm.Avalonia;

public static class ViewWpfExtensions
{
    public static Window AsWindow<T>(this T view) where T : IView
    {
        if (view is StyledElement fe) return fe.AsWindow();

        throw new ArgumentException("view should be FrameworkElement");
    }

    public static Window AsWindow(this StyledElement view)
    {
        if (view is Window win) return win;

        var w = new DefaultWindow
        {
            //Background = Brushes.Blue,
            DataContext = view?.DataContext,
            Content = view,
//
            
            //SizeToContent = SizeToContent.WidthAndHeight,
            //WindowStartupLocation = WindowStartupLocation.CenterOwner,
        };

        return w;
    }

    public static Window AsDialog(this IView view)
    {
        if (view is StyledElement fe) return fe.AsDialog();

        throw new ArgumentException("view should be FrameworkElement");
    }

    public static Window AsDialog(this StyledElement view)
    {
        if (view is not Window w)
        {
            w = new DefaultWindow
            {
                DataContext = view?.DataContext,
                Content = view,

                SizeToContent = SizeToContent.WidthAndHeight,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                //WindowStyle = WindowStyle.None,
                //ResizeMode = ResizeMode.NoResize,
            };
        }

        return w;
    }

    public static TViewModel? ViewModel<TViewMode,TViewModel>(this IView<TViewMode,TViewModel> view)
        where TViewMode : ViewMode =>
        view is StyledElement { DataContext: TViewModel vm } ? vm : default;

    public static bool TryGetViewModel<TViewMode,TViewModel>(this IView<TViewMode,TViewModel> view,out TViewModel? viewModel)
        where TViewMode : ViewMode
    {
        if (view is StyledElement { DataContext: TViewModel vm })
        {
            viewModel = vm;
            return true;
        }

        viewModel = default;
        return false;
    }
}
using Avalonia;
using HLab.Mvvm.Annotations;

namespace HLab.Mvvm.Avalonia;

public static class ViewExtensions
{
    public static TViewModel? GetViewModel<TViewModel>(this StyledElement @this, object? dataContext)
    {
        return dataContext is TViewModel vm  ? vm : default;
    }

    public static TViewModel? GetViewModel<TView,TViewModel>(this TView @this, TViewModel dummy)
    where TView : StyledElement, IView<TViewModel>
    {
        return @this.DataContext is TViewModel vm  ? vm : default;
    }

}


public class ViewHelperAvalonia : IViewHelper
{
    public ViewHelperAvalonia(StyledElement view)
    {
        View = view;
    }

    public StyledElement View { get; }
    public IMvvmContext? Context
    {
        get => View.GetValue(ViewLocator.MvvmContextProperty)!;
        set => View.SetValue(ViewLocator.MvvmContextProperty, value);
    }

    public object? Linked
    {
        get => View.DataContext;
        set => View.DataContext = value;
    }
}
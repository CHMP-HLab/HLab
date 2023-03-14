using Avalonia;
using HLab.Mvvm.Annotations;

namespace HLab.Mvvm.Avalonia
{
    public class ViewHelperAvalonia : IViewHelper
    {
        public ViewHelperAvalonia(StyledElement view)
        {
            View = view;
        }

        public StyledElement View { get; }
        public IMvvmContext? Context
        {
            get => (IMvvmContext)View.GetValue(ViewLocator.MvvmContextProperty)!;
            set => View.SetValue(ViewLocator.MvvmContextProperty, value);
        }

        public object? Linked
        {
            get => View.DataContext;
            set => View.DataContext = value;
        }
    }
}

using Avalonia;
using HLab.Mvvm.Annotations;

namespace HLab.Mvvm.Avalonia
{
    public class ViewHelperAvalonia : IViewHelper
    {
        public ViewHelperAvalonia(IStyledElement view)
        {
            View = view;
        }

        public IStyledElement View { get; }
        public IMvvmContext Context
        {
            get => (IMvvmContext)View.GetValue(ViewLocator.MvvmContextProperty);
            set => View.SetValue(ViewLocator.MvvmContextProperty, value);
        }

        public object? Linked
        {
            get => View.DataContext;
            set => View.DataContext = value;
        }
    }
}

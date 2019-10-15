using System.Windows;
using HLab.Mvvm.Annotations;

namespace HLab.Mvvm.Wpf
{
    public class ViewHelperWpf : IViewHelper
    {
        public ViewHelperWpf(FrameworkElement view)
        {
            View = view;
        }

        public FrameworkElement View { get; }
        public IMvvmContext Context
        {
            get => (MvvmContext) View.GetValue(ViewLocator.MvvmContextProperty);
            set => View.SetValue(ViewLocator.MvvmContextProperty, value);
        }

        public object Linked
        {
            get => View.DataContext;
            set => View.DataContext = value;
        }
    }
}

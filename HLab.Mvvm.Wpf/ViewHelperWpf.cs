using System.Windows;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Wpf;

namespace HLab.Mvvm
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
            get => (IMvvmContext) View.GetValue(ViewLocator.MvvmContextProperty);
            set => View.SetValue(ViewLocator.MvvmContextProperty, value);
        }

        public object Linked
        {
            get => View.DataContext;
            set => View.DataContext = value;
        }
    }
}

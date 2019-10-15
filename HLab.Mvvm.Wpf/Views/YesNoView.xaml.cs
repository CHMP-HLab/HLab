using System.Windows;
using System.Windows.Controls;
using HLab.Base;

namespace HLab.Mvvm.Views
{
    /// <summary>
    /// Logique d'interaction pour YesNoView.xaml
    /// </summary>
    public partial class YesNoView : UserControl
    {
        public YesNoView()
        {
            InitializeComponent();
        }

        class H : DependencyHelper<YesNoView> { }

        public static DependencyProperty ValueProperty = H.Property<bool?>()
            .OnChange((s, a) =>
            {
                    s.ButtonYes.IsChecked = a.NewValue;
                    s.ButtonNo.IsChecked = !a.NewValue;
            })
            .Register();

        public static readonly DependencyProperty IsReadOnlyProperty =
            H.Property<bool>().Default(false).OnChange((e, a) =>
            {
                e.SetReadOnly();
            }).Register();

        public bool? Value
        {
            get => (bool?)GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }
        public bool IsReadOnly
        {
            get => (bool)GetValue(IsReadOnlyProperty);
            set => SetValue(IsReadOnlyProperty, value);
        }

        private void Button_OnChecked(object sender, RoutedEventArgs e)
        {
            if (ReferenceEquals(sender, ButtonYes)) Value = true;
            if (ReferenceEquals(sender, ButtonNo)) Value = false;
        }
        private void SetReadOnly()
        {
            ButtonNo.IsEnabled = !IsReadOnly;
            ButtonYes.IsEnabled = !IsReadOnly;
        }
    }
}

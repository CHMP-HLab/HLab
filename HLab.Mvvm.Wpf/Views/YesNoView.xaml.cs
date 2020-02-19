using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using HLab.Base;

namespace HLab.Mvvm.Views
{
    using H = DependencyHelper<YesNoView>;

    /// <summary>
    /// Logique d'interaction pour YesNoView.xaml
    /// </summary>
    public partial class YesNoView : UserControl
    {

        public YesNoView()
        {
            InitializeComponent();
        }


        public static DependencyProperty ValueProperty = H.Property<bool?>()
            .OnChange((s, a) =>
            {
                s.SetValue(a.NewValue);

            }).BindsTwoWayByDefault
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

        private void SetValue(bool? value)
        {
            ButtonYes.IsChecked = value == true;
            ButtonNo.IsChecked = value == false;
        }

        private void Button_OnChecked(object sender, RoutedEventArgs e)
        {
            if (IsReadOnly)
            {
                SetValue(Value);
                return;
            }
            if (ReferenceEquals(sender, ButtonYes)) Value = true;
            if (ReferenceEquals(sender, ButtonNo)) Value = false;
        }
        private void SetReadOnly()
        {
            //ButtonNo.IsEnabled = !IsReadOnly;
            //ButtonYes.IsEnabled = !IsReadOnly;
        }

        private void Button_OnClick(object sender, RoutedEventArgs e)
        {
            if (IsReadOnly)
            {
                e.Handled = true;
            }
        }

        private void Button_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (IsReadOnly)
            {
                e.Handled = true;
            }
        }
    }
}

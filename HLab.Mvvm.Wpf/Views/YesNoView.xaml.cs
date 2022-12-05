using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using HLab.Base.Wpf;

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
                s.UpdateValue(a.NewValue, a.OldValue);

            }).BindsTwoWayByDefault
            .Register();

        public static readonly DependencyProperty IsReadOnlyProperty =
            H.Property<bool>().Default(false).OnChange((e, a) =>
            {
                e.SetReadOnly();
            }).Register();

        public static readonly DependencyProperty AllowNaProperty =
            H.Property<bool>().Default(false).OnChange((e, a) =>
            {
                e.SetNa(a.NewValue);
            }).Register();

        public static readonly DependencyProperty StringProperty =
            H.Property<string>().Default(null).OnChange((e, a) =>
            {
                e.UpdateString(a.NewValue);
            }).Register();

        void SetNa(bool na)
        {
            ButtonNa.Visibility =  na?Visibility.Visible:Visibility.Collapsed;
            Spacer.Visibility = na?Visibility.Visible:Visibility.Collapsed;
        }

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
        public bool AllowNa
        {
            get => (bool)GetValue(AllowNaProperty);
            set => SetValue(AllowNaProperty, value);
        }
        public string String
        {
            get => (string)GetValue(StringProperty);
            set => SetValue(StringProperty, value);
        }

        void UpdateString(string s)
        {
            ButtonYes.IsChecked = s == "true";
            ButtonNa.IsChecked = s == "na";
            ButtonNo.IsChecked = s == "false";

            if (s == "true") Value = true;
            if (s == "false") Value = false;
            if (s == "na") Value = null;
            if (s == "") Value = null;
        }

        void UpdateValue(bool? v, bool? old)
        {
            ButtonYes.IsChecked = v == true;
            ButtonNo.IsChecked = v == false;

            if (v == true) String = "true";
            if (v == false) String = "false";

            if (v == null)
            {
                if (old != null)
                {
                    String = "";
                }
            }
        }

        void Button_OnChecked(object sender, RoutedEventArgs e)
        {
            if (IsReadOnly)
            {
                UpdateString(String);
                return;
            }

            if (ReferenceEquals(sender, ButtonYes))
            {
                if(ButtonYes.IsChecked==true)
                    String = "true";
                else
                    String = "";
            }

            if (ReferenceEquals(sender, ButtonNo))
            {
                if(ButtonNo.IsChecked==true)
                    String = "false";
                else
                    String = "";
            }

            if (ReferenceEquals(sender, ButtonNa))
            {
                if(ButtonNa.IsChecked==true)
                    String = "na";
                else
                    String = "";
            }
        }

        void SetReadOnly()
        {
            //ButtonNo.IsEnabled = !IsReadOnly;
            //ButtonYes.IsEnabled = !IsReadOnly;
        }

        void Button_OnClick(object sender, RoutedEventArgs e)
        {
            if (IsReadOnly)
            {
                e.Handled = true;
            }
        }

        void Button_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (IsReadOnly)
            {
                e.Handled = true;
            }
        }

        void Button_OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (IsReadOnly)
            {
                e.Handled = true;
            }
        }
    }
}

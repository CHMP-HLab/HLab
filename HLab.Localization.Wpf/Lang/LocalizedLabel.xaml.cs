using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using HLab.Base.Wpf;

namespace HLab.Localization.Wpf.Lang
{
    using H = DependencyHelper<LocalizedLabel>;
    /// <summary>
    /// Logique d'interaction pour LocalizedLabel.xaml
    /// </summary>
    [ContentProperty(nameof(Text))]
    public partial class LocalizedLabel : Label
    {
        public LocalizedLabel()
        {
            InitializeComponent();
        }
        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }
        public static readonly DependencyProperty TextProperty =
            H.Property<string>()
            .OnChange((e,a)=> e.Localize.Id = a.NewValue)
                .Register();
    }
}

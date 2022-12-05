using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using HLab.Base.Extensions;
using HLab.Base.Wpf;
using HLab.Mvvm.Annotations;

namespace HLab.Localization.Wpf.Lang
{
    using H = DependencyHelper<LocalizeTextBox>;
    /// <summary>
    /// Logique d'interaction pour LocalizeTextBox.xaml
    /// </summary>
    ///
    [ContentProperty(nameof(Text))]
    public partial class LocalizeTextBox : UserControl
    {
        public LocalizeTextBox()
        {
            InitializeComponent();
        }

        void SetReadOnly(bool readOnly)
        {
            if (readOnly)
            {
                TextBoxEnabled.Visibility = Visibility.Collapsed;
                TextBoxDisabled.Visibility = Visibility.Visible;
                Button.Visibility = Visibility.Collapsed;
                LocalizationOpened = false;
            }
            else
            {
                TextBoxEnabled.Visibility = Visibility.Visible;
                TextBoxDisabled.Visibility = Visibility.Visible;
                Button.Visibility = Visibility.Visible;
            }
        }

        public static readonly DependencyProperty TextProperty =
            H.Property<string>()
                .BindsTwoWayByDefault
                .OnChange(async (s, e) =>
                {
                    var localize = (ILocalizationService) s.GetValue(Localize.LocalizationServiceProperty);
                    s.TextBoxDisabled.Text = await localize.LocalizeAsync(e.NewValue).ConfigureAwait(true);
                    await s.PopulateAsync(e.NewValue);
                })
                .Register();

        public static readonly DependencyProperty IsReadOnlyProperty =
            H.Property<bool>()
                .OnChange((s, e) =>
                {
                    s.SetReadOnly(e.NewValue);
                })
                .Register();

        public static readonly DependencyProperty LocalizationOpenedProperty =
            H.Property<bool>()
                .OnChange((s, e) =>
                {
                    s.SetLocalizationOpened(e.NewValue);
                })
                .Register();

        async void SetLocalizationOpened(bool opened)
        {
            if (opened)
            {
                DataGrid.Visibility = Visibility.Visible;
                await PopulateAsync(Text);
            }
            else
            {
                DataGrid.Visibility = Visibility.Collapsed;
                UnPopulate();
            }
        }

        public string Text
        {
            get => (string) GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public bool IsReadOnly
        {
            get => (bool) GetValue(IsReadOnlyProperty);
            set => SetValue(IsReadOnlyProperty, value);
        }

        public bool LocalizationOpened
        {
            get => (bool) GetValue(LocalizationOpenedProperty);
            set => SetValue(LocalizationOpenedProperty, value);
        }


        public ObservableCollection<ILocalizeEntry> Translations { get; } = new();

        async Task PopulateAsync(string source)
        {
            var service = (ILocalizationService)GetValue(Localize.LocalizationServiceProperty);
            var list = source.GetInside('{', '}').ToList();

            Translations.Clear();

            foreach (var s in list)
            {
                Translations.Add(await service.GetLocalizeEntryAsync("fr-fr",s));
            }
        }

        void UnPopulate()
        {
            Translations.Clear();
        }

        void Button_OnClick(object sender, RoutedEventArgs e)
        {
            LocalizationOpened = !LocalizationOpened;
        }
    }

}

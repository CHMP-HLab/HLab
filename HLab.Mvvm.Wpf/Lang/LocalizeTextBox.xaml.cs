using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using HLab.Base;
using HLab.Base.Extensions;
using HLab.Mvvm.Annotations;

namespace HLab.Mvvm.Lang
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

        private void SetReadOnly(bool readOnly)
        {
            if (!readOnly)
            {
                TextBoxEnabled.Visibility = Visibility.Visible;
                TextBoxDisabled.Visibility = Visibility.Visible;//Visibility.Collapsed;
            }
            else
            {
                TextBoxEnabled.Visibility = Visibility.Collapsed;
                TextBoxDisabled.Visibility = Visibility.Visible;
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



        public ObservableCollection<ILocalizeEntry> Translations { get; } = new();

        private async Task PopulateAsync(string source)
        {
            var service = (ILocalizationService)GetValue(Localize.LocalizationServiceProperty);
            var list = source.GetInside('{', '}').ToList();

            Translations.Clear();

            foreach (var s in list)
            {
                Translations.Add(await service.GetLocalizeEntryAsync("fr-fr",s));
            }
        }


    }

}

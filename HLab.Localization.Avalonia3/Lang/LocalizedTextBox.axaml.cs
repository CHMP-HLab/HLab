using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using HLab.Base.Extensions;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using HLab.Mvvm.Annotations;
using HLab.Base.Avalonia.DependencyHelpers;

namespace HLab.Localization.Avalonia.Lang;

using H = DependencyHelper<LocalizedTextBox>;
/// <summary>
/// Logique d'interaction pour LocalizeTextBox.xaml
/// </summary>
///
public partial class LocalizedTextBox : UserControl
{
    public LocalizedTextBox()
    {
        InitializeComponent();
    }

    void SetReadOnly(bool readOnly)
    {
        if (readOnly)
        {
            TextBoxEnabled.IsVisible = false;
            TextBoxDisabled.IsVisible = true;
            Button.IsVisible = false;
            LocalizationOpened = false;
        }
        else
        {
            TextBoxEnabled.IsVisible = true;
            TextBoxDisabled.IsVisible = true;
            Button.IsVisible = true;
        }
    }

    public static readonly StyledProperty<string> TextProperty =
        H.Property<string>()
            .BindsTwoWayByDefault
            .OnChanged(async (e,a) =>
            {
                var localize = e.GetValue(Localize.LocalizationServiceProperty);
                e.TextBoxDisabled.Text = await localize.LocalizeAsync(e.Text).ConfigureAwait(true);
                await e.PopulateAsync(e.Text);
            })
            .Register();

    public static readonly StyledProperty<bool> IsReadOnlyProperty =
        H.Property<bool>()
            .OnChanged((e,a) =>
            {
                e.SetReadOnly(e.IsReadOnly);
            })
            .Register();

    public static readonly StyledProperty<bool> LocalizationOpenedProperty =
        H.Property<bool>()
            .OnChanged((e,a) =>
            {
                e.SetLocalizationOpened(e.LocalizationOpened);
            })
            .Register();

    async void SetLocalizationOpened(bool opened)
    {
        if (opened)
        {
            DataGrid.IsVisible = true;
            await PopulateAsync(Text);
        }
        else
        {
            DataGrid.IsVisible = false;
            UnPopulate();
        }
    }

    //[Content]
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
        var service = GetValue(Localize.LocalizationServiceProperty);
        var list = source.GetInside('{', '}').ToList();

        Translations.Clear();

        foreach (var s in list)
        {
            Translations.Add(await service?.GetLocalizeEntryAsync("fr-fr",s)!);
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
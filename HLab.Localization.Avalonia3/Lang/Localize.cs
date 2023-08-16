using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Metadata;
using Avalonia.Threading;
using HLab.Base.Avalonia;
using HLab.Mvvm.Annotations;

namespace HLab.Localization.Avalonia.Lang;

using H = DependencyHelper<Localize>;

public class Localize : TextBlock
{
    public Localize()
    {
        if (Design.IsDesignMode)
        {
            _updateAsync = UpdateDesignModeAsync;
            return;
        }
        
        _updateAsync = InitAsync;
    }

    [Content]
    public string Id
    {
        get => GetValue(IdProperty);
        set => SetValue(IdProperty, value);
    }
    public static readonly StyledProperty<string> IdProperty =
        H.Property<string>()
            .OnChanged(async (e, a) =>
            {
                await e._updateAsync();
            })
            .Register();

    public string? StringFormat
    {
        get => GetValue(StringFormatProperty);
        set => SetValue(StringFormatProperty, value);
    }
    public static readonly StyledProperty<string?> StringFormatProperty =
        H.Property<string?>()
            .OnChanged(async (e,a) =>
            {
                if (e.StringFormat == null)
                {
                    e._updateAsync = e.InitAsync;
                }
                await e._updateAsync();
            })
            .Register();

    public ILocalizationService? LocalizationService
    {
        get => GetValue(LocalizationServiceProperty);
        set => SetValue(LocalizationServiceProperty, value);
    } 
    public static ILocalizationService GetLocalizationService(AvaloniaObject obj) => obj.GetValue(LocalizationServiceProperty);
    public static void SetLocalizationService(AvaloniaObject obj, ILocalizationService value) => obj.SetValue(LocalizationServiceProperty, value);

    public static readonly AttachedProperty<ILocalizationService?> LocalizationServiceProperty =
        H.Property<ILocalizationService?>()
            .OnChanged(async (e, a) =>
            {
                if (e.LocalizationService == null)
                {
                    e._updateAsync = e.InitAsync;
                }
                await e._updateAsync();
            })
            .Inherits
            .Attached.Register();

    public string? Language
    {
        get => GetValue(LanguageProperty);
        set => SetValue(LanguageProperty, value);
    }

    public static readonly AttachedProperty<string?> LanguageProperty =
        H.Property<string?>()
            .OnChanged(async (e, a) =>
            {
                if (e.Language == null)
                {
                    e._updateAsync = e.InitAsync;
                }
                await e._updateAsync();
            })
            .Inherits
            .Attached.Register();
    

    Func<Task> _updateAsync;

    public async Task InitAsync()
    {
        if (LocalizationService == null) return;
        if (Language == null) return;
        if (Id == null) return;
        _updateAsync = UpdateAsync;
        await UpdateAsync();
    }

    public async Task UpdateAsync()
    {
        var localized = Id;
        try
        {
            localized = await LocalizationService.LocalizeAsync(Language, localized).ConfigureAwait(false);
        }
        catch (Exception)
        {
        }

        await Dispatcher.UIThread.InvokeAsync(() => Text = localized);
    }
    public async Task UpdateDesignModeAsync()
    {
        await Dispatcher.UIThread.InvokeAsync(() => Text = Id);
    }

    public static string GetLanguage(AvaloniaObject obj) => obj?.GetValue(LanguageProperty)??"";

    public static void SetLanguage(AvaloniaObject obj, string value) => obj.SetValue(LanguageProperty, value);
}
using System;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Metadata;
using Avalonia.Threading;
using HLab.Base.Avalonia.DependencyHelpers;
using HLab.Mvvm.Annotations;

namespace HLab.Localization.Avalonia.Lang;

using H = DependencyHelper<Localize>;

public class Localize : TextBlock
{
    public Localize()
    {
        if (Design.IsDesignMode)
        {
            _update = UpdateDesignMode;
            return;
        }
        
        _update = Init;
    }

    [Content]
    public string Id
    {
        get => GetValue(IdProperty);
        set => SetValue(IdProperty, value);
    }
    public static readonly StyledProperty<string> IdProperty =
        H.Property<string>()
            .OnChanged((e, a) =>
            {
                e._update();
            })
            .Register();

    public string? StringFormat
    {
        get => GetValue(StringFormatProperty);
        set => SetValue(StringFormatProperty, value);
    }
    public static readonly StyledProperty<string?> StringFormatProperty =
        H.Property<string?>()
            .OnChanged((e,a) =>
            {
                if (e.StringFormat == null)
                {
                    e._update = e.Init;
                }
                e._update();
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
            .OnChanged((e, a) =>
            {
                if (e.LocalizationService == null)
                {
                    e._update = e.Init;
                }
                e._update();
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
            .OnChanged((e, a) =>
            {
                if (e.Language == null)
                {
                    e._update = e.Init;
                }
                e._update();
            })
            .Inherits
            .Attached.Register();
    

    Action _update;

    public void Init()
    {
        if (LocalizationService == null) return;
        if (Language == null) return;
        if (Id == null) return;
        _update = Update;
        Update();
    }

    void Update()
    {
        var localized = Id;
        try
        {
            var token = new CancellationToken();
            Task.Run(async ()=>
            {
                await LocalizationService.LocalizeAsync(Language, localized, token).ConfigureAwait(false);
                if(token.IsCancellationRequested) return;
                await Dispatcher.UIThread.InvokeAsync(() => Text = localized);
            }, token);
        }
        catch (Exception)
        {
        }
    }
    public void UpdateDesignMode()
    {
        if(!Design.IsDesignMode) throw new InvalidOperationException("Only for design mode");

        Dispatcher.UIThread.InvokeAsync(() => Text = Id);
    }

    public static string GetLanguage(AvaloniaObject obj) => obj?.GetValue(LanguageProperty)??"";

    public static void SetLanguage(AvaloniaObject obj, string value) => obj.SetValue(LanguageProperty, value);
}
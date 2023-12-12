#nullable enable
using System.Management;
using System.Security.Principal;
using Avalonia.Controls;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Media;
using HLab.ColorTools.Avalonia;
using Microsoft.Win32;

namespace HLab.Base.Avalonia.Themes;

public class ThemeService
{
    const string REGISTRY_KEY_PATH = @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";
    const string REGISTRY_VALUE_NAME = "AppsUseLightTheme";

    readonly IResourceProvider _themeDark;
    readonly IResourceProvider _themeLight;


    public enum WindowsTheme
    {
        Light,
        Dark,
        Auto
    }


    readonly IResourceDictionary _dictionary;
    public ThemeService(IResourceDictionary dictionary)
    {

        var assemblyName = typeof(ThemeService).Assembly.GetName().Name;

        _themeDark = new ResourceInclude(new Uri($"avares://{assemblyName}")) {Source = new Uri(
            "/Themes/HLab.Theme.Dark.axaml",
            UriKind.RelativeOrAbsolute)};

        _themeLight = new ResourceInclude(new Uri($"avares://{assemblyName}")) {Source = new Uri(
            "/Themes/HLab.Theme.Light.axaml", 
            UriKind.RelativeOrAbsolute)};

        _dictionary = dictionary;
    }

    ManagementEventWatcher? _watcher = null;
    void SetAuto()
    {
        UnsetAuto();

        var currentUser = WindowsIdentity.GetCurrent();
        if (currentUser.User == null) return;

        var query = @$"SELECT * FROM RegistryValueChangeEvent WHERE Hive = 'HKEY_USERS' AND KeyPath = '{currentUser.User.Value}\\{REGISTRY_KEY_PATH.Replace(@"\", @"\\")}' AND ValueName = '{REGISTRY_VALUE_NAME}'";

        try
        {
            _watcher = new ManagementEventWatcher(query);
            _watcher.EventArrived += _watcher_EventArrived;

            // Start listening for events
            _watcher.Start();
        }
        catch (Exception)
        {
            // This can fail on Windows 7
        }

        SetTheme(GetWindowsTheme());
    }

    void _watcher_EventArrived(object sender, EventArrivedEventArgs e)
    {
        SetTheme(GetWindowsTheme());
    }

    void UnsetAuto()
    {
        if(_watcher==null) return;
        _watcher.Stop();
        _watcher.EventArrived -= _watcher_EventArrived;
        _watcher.Dispose();
        _watcher = null;
    }

    public void SetTheme(string name)
    {
        SetTheme(name switch
        {
            "{Dark}" => WindowsTheme.Dark,
            "{Light}" => WindowsTheme.Light,
            _ => WindowsTheme.Auto
        });
    }


    public void SetTheme(WindowsTheme theme)
    {
        //ThemeManager.Current.SyncTheme(ThemeSyncMode.SyncAll);

        // TODO Avalonia
        switch (theme)
        {
            case WindowsTheme.Light:
                //ThemeManager.Current.ChangeTheme(this, "Light.Blue");
                UnsetAuto();
                if (_dictionary.MergedDictionaries.Contains(_themeDark)) _dictionary.MergedDictionaries.Remove(_themeDark);
                if (!_dictionary.MergedDictionaries.Contains(_themeLight))
                    _dictionary.MergedDictionaries.Add(_themeLight);
                break;

            case WindowsTheme.Dark:
                //ThemeManager.Current.ChangeTheme(this, "Dark.Blue");
                UnsetAuto();
                if (_dictionary.MergedDictionaries.Contains(_themeLight)) _dictionary.MergedDictionaries.Remove(_themeLight);
                if (!_dictionary.MergedDictionaries.Contains(_themeDark))
                    _dictionary.MergedDictionaries.Add(_themeDark);
                break;
            case WindowsTheme.Auto:
                SetAuto();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(theme), theme, null);
        }
    }
    static WindowsTheme GetWindowsTheme()
    {
        using var key = Registry.CurrentUser.OpenSubKey(REGISTRY_KEY_PATH);

        var registryValueObject = key?.GetValue(REGISTRY_VALUE_NAME);
        if (registryValueObject is int registryValue)
        {
            return registryValue > 0 ? WindowsTheme.Light : WindowsTheme.Dark;
        }
        return WindowsTheme.Light;
    }

    static Color GetAccentColor()
    {
        using var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\DWM");

        var value = key?.GetValue("AccentColor");

        if (value is string s && int.TryParse(s, out var result))
        {
            return result.ToAvaloniaColor();
        }
        return Colors.Blue;
    }

}
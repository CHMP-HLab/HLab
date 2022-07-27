using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Management;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using ControlzEx.Theming;
using HLab.ColorTools.Wpf;
using Microsoft.Win32;

namespace HLab.Base.Wpf.Themes
{
    public class ThemeWatcher
    {
        const string RegistryKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";
        const string RegistryValueName = "AppsUseLightTheme";

        readonly ResourceDictionary _themeDark = new() { Source = new Uri("/HLab.Base.Wpf;component/Themes/HLab.Theme.Dark.xaml", UriKind.RelativeOrAbsolute) };
        readonly ResourceDictionary _themeLight = new() { Source = new Uri("/HLab.Base.Wpf;component/Themes/HLab.Theme.Light.xaml", UriKind.RelativeOrAbsolute) };

        enum WindowsTheme
        {
            Light,
            Dark
        }


        readonly ResourceDictionary _dictionary;
        public ThemeWatcher(ResourceDictionary dictionary)
        {
            _dictionary = dictionary;

            var currentUser = WindowsIdentity.GetCurrent();
            if (currentUser.User == null) return;

            var query = @$"SELECT * FROM RegistryValueChangeEvent WHERE Hive = 'HKEY_USERS' AND KeyPath = '{currentUser.User.Value}\\{RegistryKeyPath.Replace(@"\", @"\\")}' AND ValueName = '{RegistryValueName}'";

            try
            {
                var watcher = new ManagementEventWatcher(query);
                watcher.EventArrived += (sender, args) =>
                {
                    SetTheme(GetWindowsTheme());
                };

                // Start listening for events
                watcher.Start();
            }
            catch (Exception)
            {
                // This can fail on Windows 7
            }

            SetTheme(GetWindowsTheme());
        }

        void SetTheme(WindowsTheme theme)
        {
            ThemeManager.Current.SyncTheme(ThemeSyncMode.SyncAll);

            switch (theme)
            {
                case WindowsTheme.Light:
                    //ThemeManager.Current.ChangeTheme(this, "Light.Blue");
                    if (_dictionary.MergedDictionaries.Contains(_themeDark)) _dictionary.MergedDictionaries.Remove(_themeDark);
                    if (!_dictionary.MergedDictionaries.Contains(_themeLight))
                        _dictionary.MergedDictionaries.Add(_themeLight);
                    break;

                case WindowsTheme.Dark:
                    //ThemeManager.Current.ChangeTheme(this, "Dark.Blue");

                    if (_dictionary.MergedDictionaries.Contains(_themeLight)) _dictionary.MergedDictionaries.Remove(_themeLight);
                    if (!_dictionary.MergedDictionaries.Contains(_themeDark))
                        _dictionary.MergedDictionaries.Add(_themeDark);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(theme), theme, null);
            }
        }
        static WindowsTheme GetWindowsTheme()
        {
            using var key = Registry.CurrentUser.OpenSubKey(RegistryKeyPath);

            var registryValueObject = key?.GetValue(RegistryValueName);
            if (registryValueObject is int registryValue)
            {
                return registryValue > 0 ? WindowsTheme.Light : WindowsTheme.Dark;
            }
            return WindowsTheme.Light;
        }

        static Color GetAccentColor()
        {
            using var key = Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\DWM");

            var value = key.GetValue("AccentColor");

            if (value is string s && int.TryParse(s, out var result))
            {
                return result.ToColor();
            }
            return Colors.Blue;
        }

    }
}

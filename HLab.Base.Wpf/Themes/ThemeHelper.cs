#nullable enable
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
using HLab.ColorTools.Wpf;
using Microsoft.Win32;

namespace HLab.Base.Wpf.Themes
{
    public class ThemeService
    {
        const string RegistryKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize";
        const string RegistryValueName = "AppsUseLightTheme";

        readonly ResourceDictionary _themeDark = new() { Source = new Uri("/HLab.Base.Wpf;component/Themes/HLab.Theme.Dark.xaml", UriKind.RelativeOrAbsolute) };
        readonly ResourceDictionary _themeLight = new() { Source = new Uri("/HLab.Base.Wpf;component/Themes/HLab.Theme.Light.xaml", UriKind.RelativeOrAbsolute) };

        public enum WindowsTheme
        {
            Light,
            Dark,
            Auto
        }


        readonly ResourceDictionary _dictionary;
        public ThemeService(ResourceDictionary dictionary)
        {
            _dictionary = dictionary;
        }

        ManagementEventWatcher? _watcher = null;
        void SetAuto()
        {
            UnsetAuto();

            var currentUser = WindowsIdentity.GetCurrent();
            if (currentUser.User == null) return;

            var query = @$"SELECT * FROM RegistryValueChangeEvent WHERE Hive = 'HKEY_USERS' AND KeyPath = '{currentUser.User.Value}\\{RegistryKeyPath.Replace(@"\", @"\\")}' AND ValueName = '{RegistryValueName}'";

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

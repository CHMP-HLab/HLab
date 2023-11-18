using System.Runtime.InteropServices;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media.Imaging;
using HLab.Icons.Avalonia;
using HLab.Icons.Avalonia.Icons;
using HLab.Mvvm.Annotations;

namespace HLab.UserNotification.Avalonia;

public class UserNotificationServiceAvalonia : IUserNotificationService
{
    // TODO : Toast implementation
    //readonly INotificationManager _manager;
    readonly TrayIcons _trayIcons = new();
    readonly TrayIcon _trayIcon = new();
    readonly IIconService _icons;

    public UserNotificationServiceAvalonia(IIconService icons)
    {
        _icons = icons;
        _trayIcons.Add(_trayIcon);
        TrayIcon.SetIcons(Application.Current, _trayIcons);
        //_manager = CreateManager();
    }

    //private static INotificationManager CreateManager()
    //{
    //    if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
    //    {
    //        return new FreeDesktopNotificationManager();
    //    }

    //    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
    //    {
    //        return new WindowsNotificationManager();
    //    }

    //    if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
    //    {
    //        //return new AppleNotificationManager();
    //    }

    //    throw new PlatformNotSupportedException();
    //}
    public void AddMenu(int pos, NativeMenuItem item)
    {
        _trayIcon.Menu ??= new NativeMenu();

        if (pos < 0 || pos >= _trayIcon.Menu.Items.Count)
            _trayIcon.Menu.Items.Add(item);
        else
            _trayIcon.Menu.Items.Insert(pos, item);
    }

    public void AddMenu(int pos, string header, string iconPath, Func<Task> action)
    {
        NativeMenuItem item = new(header.ToString())
        {
            Icon = GetImage(iconPath, 16),
            //IsChecked = chk,
        };

        item.Click += async (o,a) => await action();

        AddMenu(pos, item);
    }

    public void AddMenu(int pos, string header, string iconPath, ICommand command)
    {
        NativeMenuItem item = new(header.ToString())
        {
            Icon = GetImage(iconPath, 16),
            Command = command,
            //IsChecked = chk,
        };

        AddMenu(pos, item);
    }

    Bitmap GetImage(string path, int size)
    {
        var icon = new IconView {Path = path};// _iconService.GetIcon(path);}

        IconView.SetIconService(icon,_icons);

        return XamlTools.GetBitmap(icon, new(size, size));
    }

    public event Action<object, object>? Click;

    public void SetIcon(string icon, int i)
    {
    }

    public string ToolTipText
    {
        get => _toolTipText;
        set
        {
            _toolTipText = value;
            if (_trayIcon != null)
                _trayIcon.ToolTipText = value;
        }
    }

    public WindowIcon Icon
    {
        get => _icon;
        set
        {
            _icon = value;
            if(_trayIcon!=null)
                _trayIcon.Icon = value;
        }
    }

    WindowIcon _icon;
    string _toolTipText;

    public void Show()
    {
        /*
        _trayIcon = new TrayIcon
        {
            Icon = Icon,
            ToolTipText = ToolTipText,
                
        };
        */

    }

    public void Notify(string title, string message)
    {
        //_manager.ShowNotification(new Notification { Title = title, Body = message});
        //_taskbarIcon.ShowNotification(title, message, NotificationIcon.None);
    }
    public void NotifyInfo(string title, string message)
    {
        //_manager.ShowNotification(new Notification { Title = title, Body = message});
        //_taskbarIcon.ShowNotification(title, message, NotificationIcon.Info);
    }
    public void NotifyWarning(string title, string message)
    {
        //_manager.ShowNotification(new Notification { Title = title, Body = message});
        //_taskbarIcon.ShowNotification(title, message, NotificationIcon.Warning);
    }
    public void NotifyError(string title, string message)
    {
        //_manager.ShowNotification(new Notification { Title = title, Body = message});
        //_taskbarIcon.ShowNotification(title, message, NotificationIcon.Error);
    }

}
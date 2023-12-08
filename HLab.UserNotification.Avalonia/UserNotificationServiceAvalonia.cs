using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Threading;
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

        _trayIcon.Clicked += (o, a) => Click?.Invoke(o, a);
    }

    public void AddMenu(int pos, NativeMenuItem item)
    {
        _trayIcon.Menu ??= new NativeMenu();

        if (pos < 0 || pos >= _trayIcon.Menu.Items.Count)
            _trayIcon.Menu.Items.Add(item);
        else
            _trayIcon.Menu.Items.Insert(pos, item);
    }

    public async Task AddMenuAsync(int pos, string header, string iconPath, Func<Task> action)
    {
        var icon = await GetImageAsync(iconPath, 32);

        NativeMenuItem item = new(header.ToString())
        {
            Icon = icon,
            //IsChecked = chk,
        };

        item.Click += async (o,a) => await action();

        AddMenu(pos, item);
    }

    public async Task AddMenuAsync(int pos, string header, string iconPath, ICommand command)
    {
        var icon = await GetImageAsync(iconPath, 256);

        NativeMenuItem item = new(header.ToString())
        {
            Icon = icon,
            Command = command,
            //IsChecked = chk,
        };

        AddMenu(pos, item);
    }

    async Task<Bitmap?> GetImageAsync(string path, int size)
    {
        if (await _icons.GetIconAsync(path) is not Control c) return null;

        if (Dispatcher.UIThread.CheckAccess())
            return XamlTools.GetBitmap(c, new(size, size));

        return await Dispatcher.UIThread.InvokeAsync(() => XamlTools.GetBitmap(c, new(size, size))) ;

    }

    public event Action<object?, object>? Click;

    public async Task SetIconAsync(string iconPath, int i)
    {
        var icon = await GetImageAsync(iconPath, i);

        using var memory = new MemoryStream();
        icon.Save(memory);

        memory.Position = 0;

        Icon = new WindowIcon(memory);
    }

    public string ToolTipText
    {
        get => _toolTipText;
        set
        {
            _toolTipText = value;
            _trayIcon.ToolTipText = value;
        }
    }

    public WindowIcon? Icon
    {
        get => _icon;
        set
        {
            _icon = value;

            Dispatcher.UIThread.InvokeAsync(() => _trayIcon.Icon = value);
        }
    }
    WindowIcon? _icon;

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
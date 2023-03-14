/*
  LittleBigMouse.Daemon
  Copyright (c) 2021 Mathieu GRENET.  All right reserved.

  This file is part of LittleBigMouse.Daemon.

    LittleBigMouse.Daemon is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    LittleBigMouse.Daemon is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with MouseControl.  If not, see <http://www.gnu.org/licenses/>.

	  mailto:mathieu@mgth.fr
	  http://www.mgth.fr
*/

using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using H.NotifyIcon;
using HLab.Icons.Annotations.Icons;
using HLab.Icons.Wpf.Icons;

namespace HLab.UserNotification.Wpf;

public class UserNotify //: IDisposable
{
    private readonly TaskbarIcon _notify;
    private readonly IIconService _iconService;

    public UserNotify(IIconService iconService)
    {
        _iconService = iconService;
        _notify = new TaskbarIcon()
        {
            Visibility = Visibility.Visible,
            ContextMenu = new ContextMenu(),
        };

        var rvc = IconView.GetIconService(_notify);

        IconView.SetIconService(_notify, iconService);

        _notify.TrayLeftMouseUp += _notify_TrayLeftMouseUp; ;
        _notify.TrayRightMouseUp += _notify_TrayRightMouseUp;
    }

    public void SetIcon(Icon icon)
    {
        _notify.Icon = icon;
        //_notify.ForceCreate();
    }

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    static extern bool DestroyIcon(IntPtr handle);

    public void SetIcon(string path, int size)
    {
        _notify.Dispatcher.Invoke(() => SetIcon(GetIcon(path, size)));
    }

    private Icon GetIcon(string path, int size)
    {
        var bitmap = GetBitmap(path, size);

        var h = bitmap.GetHicon();

        return Icon.FromHandle(h);
    }

    private Bitmap GetBitmap(string path, int size)
    {
        var wpfIcon = new IconView {Path = path};// _iconService.GetIcon(path);}

        IconView.SetIconService(wpfIcon,_iconService);

        return XamlTools.GetBitmap(wpfIcon, new(size, size));
    }

    System.Windows.Controls.Image GetImage(string path, int size)
    {
        var wpfIcon = new IconView {Path = path};// _iconService.GetIcon(path);}

        IconView.SetIconService(wpfIcon,_iconService);

        return XamlTools.GetImage(wpfIcon, new(size, size));
    }


    public void Show()
    {
        _notify.Visibility = Visibility.Visible;
        _notify.ForceCreate();
    }

    public void Hide()
    {
        _notify.Visibility = Visibility.Hidden;
    }

    private void _notify_TrayRightMouseUp(object sender, RoutedEventArgs e)
    {
        //_notify.ContextMenuStrip.Show( Control.MousePosition);
    }

    private void _notify_TrayLeftMouseUp(object sender, RoutedEventArgs e)
    {
        Click?.Invoke(sender, e);
    }


    public event EventHandler Click;
    //public void Dispose()
    //{
    //    Dispose(true);
    //    GC.SuppressFinalize(this);
    //}
    public delegate void Func();
    public void AddMenu(int pos, object header, string iconPath, Action action, string tag = null, bool chk = false)
        => AddMenu(pos, header, iconPath, (s, e) => action(), tag, chk);

    public void AddMenu(int pos, object header, string iconPath, RoutedEventHandler handler, string tag = null, bool chk = false)
    {
        var contextMenu = _notify?.ContextMenu;
        if (contextMenu == null) return;

        MenuItem item = new()
        {
            Header = header,
            IsChecked = chk,
            Icon = GetImage(iconPath, 16),
            Tag = tag,
        };

        item.Click += handler;

        if (pos < 0 || pos >= contextMenu.Items.Count)
            contextMenu.Items.Add(item);
        else
            contextMenu.Items.Insert(pos, item);
    }

    public void RemoveMenu(string tag)
    {
        var contextMenu = _notify.ContextMenu;

        if (contextMenu == null) return;

        var done = false;
        while (!done)
        {
            var items = new MenuItem[contextMenu.Items.Count];
            contextMenu.Items.CopyTo(items, 0);
            done = true;
            foreach (var i in items)
            {
                if (i.Tag as string != tag) continue;

                contextMenu.Items.Remove(i);
                done = false;
            }
        }
    }

}
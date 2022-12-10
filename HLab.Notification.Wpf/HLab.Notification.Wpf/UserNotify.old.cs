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
using System.Windows;
using System.Windows.Controls;
using H.NotifyIcon;
using HLab.Icons.Annotations.Icons;
using HLab.Icons.Wpf.Icons;

namespace HLab.UserNotification.Wpf
{
    public class UserNotifyOld //: IDisposable
    {
        readonly TaskbarIcon _notify;

        public UserNotifyOld(IIconService iconService)
        {
            _notify = new TaskbarIcon()
            {
                Visibility = Visibility.Visible,
                ContextMenu = new ContextMenu(),
            };

            var rvc = IconView.GetIconService(_notify);

            IconView.SetIconService(_notify, iconService);

            SetOff();

            _notify.TrayLeftMouseUp += _notify_TrayLeftMouseUp; ;
            _notify.TrayRightMouseUp += _notify_TrayRightMouseUp;
        }

        public void SetOn()
        {
            //_notify.Icon = Resources.lbm_on;
        }

        public void SetOff()
        {
            //_notify.Icon = Resources.lbm_off;
        }

        public void Show()
        {
            _notify.Visibility = Visibility.Visible;
        }

        public void Hide()
        {
            _notify.Visibility = Visibility.Hidden;
        }

        void _notify_TrayRightMouseUp(object sender, RoutedEventArgs e)
        {
            //_notify.ContextMenuStrip.Show( Control.MousePosition);
        }

        void _notify_TrayLeftMouseUp(object sender, RoutedEventArgs e)
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
                Icon = new IconView { Path = iconPath },
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
}

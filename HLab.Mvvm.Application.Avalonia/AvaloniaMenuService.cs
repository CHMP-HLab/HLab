using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Media;
using HLab.Icons.Avalonia.Icons;
using HLab.Localization.Avalonia.Lang;
using HLab.Mvvm.Application.Menus;

namespace HLab.Mvvm.Application.Avalonia;

public class AvaloniaMenuService : IMenuService
{
    public Menu MainMenu {get;} = new();
    object IMenuService.MainMenu => MainMenu;

    public bool RegisterMenu(string path, object header, ICommand command, string iconPath) 
        => RegisterMenu( MenuPath.Get(path),MainMenu.Items, header, command, iconPath);

    static bool RegisterMenu(MenuPath path, ItemCollection items, object header, ICommand command, string iconPath)
    {
        while (true)
        {
            if (path.Next == null)
            {
                if (header is string s)
                {
                   header = new Localize { Id = s };
                }

                var m = new MenuItem {
                    Name = path.Name, 
                    Header = header, 
                    Command = command, 
                    Background = Brushes.Transparent,
                    Icon = new IconView 
                    {
                        Height = 25, 
                        Path = iconPath
                    }
                };

                var old = items.Cast<MenuItem>().FirstOrDefault(menu => menu.Name == path.Name);
                if (old != null)
                {
                    items.Remove(old);
                    while (old.Items.Count>0)
                    {
                        var item = old.Items[0];
                        old.Items.Remove(item);
                        m.Items.Add(item);
                    }
                }

                items.Add(m);
                return true;
            }

            var child = items.Cast<MenuItem>().FirstOrDefault(menu => menu.Name == path.Name);

            if (child == null)
            {
                child = new MenuItem{Name = path.Name};
                items.Add(child);
            }
            path = path.Next;
            items = child.Items;
        }
    }
}
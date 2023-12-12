using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using HLab.Erp.Core;
using HLab.Icons.Wpf.Icons;
using HLab.Localization.Wpf.Lang;

namespace HLab.Mvvm.Application.Wpf
{
    internal class MenuPath
    {
        public string Name { get; }
        public MenuPath Next {get; private set; } = null;

        MenuPath(string name)
        {
            Name = name;
        }

        public static MenuPath Get(string path) => Get(path.Split('/'));
        public static MenuPath Get(IEnumerable<string> path)
        {
            using var e = path.GetEnumerator();
            
            if (!e.MoveNext()) return null;

            var result = new MenuPath(e.Current);
            var current = result;
            while (e.MoveNext())
            {
                current = current.Next = new MenuPath(e.Current);
            }
            return result;
        }
    }


    public class MenuService : IMenuService
    {
        readonly MainWpfViewModel _viewModel;

        public MenuService(MainWpfViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        public bool RegisterMenu(string path, object header, ICommand command, string iconPath) 
            => RegisterMenu( MenuPath.Get(path),_viewModel.Menu.Items, header, command, iconPath);

        static bool RegisterMenu(MenuPath path, ItemCollection items, object header, ICommand command, string iconPath)
        {
            while (true)
            {
                if (path.Next == null)
                {
                    if (header is string s)
                    {
                        header = new Localize {Id = s};
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
                        while (old.HasItems)
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
}

using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using HLab.DependencyInjection.Annotations;
using HLab.Erp.Core;
using HLab.Mvvm.Icons;
using HLab.Mvvm.Lang;

namespace HLab.Mvvm.Application.Wpf
{
    class MenuPath
    {
        public string Name {get;} = "";
        public MenuPath Next {get;} = null;
        public MenuPath(string path):this(path.Split('/')) {  }
        public MenuPath(IEnumerable<string> path)
        {
            Name = path.First();
            if(path.Count()>1)
                Next = new MenuPath(path.Skip(1));
        }
    }


    [Export(typeof(IMenuService)), Singleton]
    public class MenuService : IMenuService
    {
        [Import]
        private MainWpfViewModel _viewModel;

        public bool RegisterMenu(string path, object header, ICommand command, string iconPath) 
            => RegisterMenu(new MenuPath(path),_viewModel.Menu.Items, header, command, iconPath);

        private static bool RegisterMenu(MenuPath path, ItemCollection items, object header, ICommand command, string iconPath)
        {
            while (true)
            {
                if (path.Next == null)
                {
                    if (header is string s)
                    {
                        header = new Localize {Id = s};
                    }

                    var m = new MenuItem {Name = path.Name, Header = header, Command = command, Icon = new IconView {Height = 25, Path = iconPath}};

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

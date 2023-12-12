using System.Collections.Generic;

namespace HLab.Mvvm.Application.Menus;

public class MenuPath
{
    public string Name { get; }
    public MenuPath Next { get; private set; } = null;

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
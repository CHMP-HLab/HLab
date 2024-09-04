using System;
using System.ComponentModel;
using System.Reflection;

namespace HLab.Notify.PropertyChanged;

public class NotifyHelper
{
    public static IEventHandlerService EventHandlerService { get; set; } = new EventHandlerService();

    protected static readonly MethodInfo SetParent = typeof(IChildObject).GetMethod("SetParent", new Type[] { typeof(object),typeof(Action<PropertyChangedEventArgs>) });

    public static string GetNameFromCallerName(string name)
    {
        if (name.Length < 2) return name;
        if (name.StartsWith("_")) name = name.Substring(1); 
        name = Char.ToUpper(name[0]) + name.Substring(1) ;
        return name;
    }
}
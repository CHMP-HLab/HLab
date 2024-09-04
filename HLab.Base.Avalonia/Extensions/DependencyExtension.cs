using Avalonia;

namespace HLab.Base.Avalonia.Extensions;

public static class DependencyExtension
{



    public static void SetBindingValue<T>(this T sender, AvaloniaProperty prop, Func<object, object> setter)
        where T : AvaloniaObject
    {

        sender.SetValue(prop, setter(sender.GetValue(prop)));

    }
}
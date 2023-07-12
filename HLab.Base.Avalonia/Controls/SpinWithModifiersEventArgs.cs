using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace HLab.Base.Avalonia.Controls;

public class SpinWithModifiersEventArgs : SpinEventArgs
{
    ButtonSpinner _button;
    public KeyModifiers KeyModifiers { get; }
    public SpinWithModifiersEventArgs(RoutedEvent routedEvent, SpinDirection direction, KeyModifiers keyModifiers) : base(routedEvent, direction)
    {
        KeyModifiers = keyModifiers;
    }
    public SpinWithModifiersEventArgs(RoutedEvent routedEvent, SpinDirection direction, KeyModifiers keyModifiers, bool usingMouseWheel) : base(routedEvent, direction, usingMouseWheel)
    {
        KeyModifiers = keyModifiers;
    }
}
using Avalonia.Input;
using Avalonia.Interactivity;

namespace HLab.Base.Avalonia.Controls;

public class ButtonEventArgs : RoutedEventArgs
{
    public ButtonEventArgs(RoutedEvent routedEvent, IInputElement source, KeyModifiers keyModifiers) 
        : base(routedEvent, source)
    {
        KeyModifiers = keyModifiers;
    }
    public KeyModifiers KeyModifiers { get; }
}
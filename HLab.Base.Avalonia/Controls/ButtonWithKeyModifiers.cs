using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.VisualTree;

namespace HLab.Base.Avalonia.Controls;

/// <summary>
/// A standard button control.
/// </summary>
public class ButtonWithKeyModifiers : Button
{
    /// <summary>
    /// Defines the <see cref="ClickWithKeyModifiers"/> event.
    /// </summary>
    public static readonly RoutedEvent<ButtonEventArgs> ClickWithKeyModifiersEvent =
        RoutedEvent.Register<Button, ButtonEventArgs>(nameof(ClickWithKeyModifiers), RoutingStrategies.Bubble);

    /// <summary>
    /// Raised when the user clicks the button.
    /// </summary>
    public event EventHandler<ButtonEventArgs>? ClickWithKeyModifiers
    {
        add => AddHandler(ClickWithKeyModifiersEvent, value);
        remove => RemoveHandler(ClickWithKeyModifiersEvent, value);
    }

    //protected virtual void OnAccessKey(RoutedEventArgs e) => OnClick(KeyModifiers.None);

    /// <inheritdoc/>
    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);

        switch (e.Key)
        {
            case Key.Enter:
                OnClick(e.KeyModifiers);
                e.Handled = true;
                break;

            case Key.Space:
            {
                if (ClickMode == ClickMode.Press)
                {
                    OnClick(e.KeyModifiers);
                }
                e.Handled = true;
                break;
            }
        }
    }

    /// <inheritdoc/>
    protected override void OnKeyUp(KeyEventArgs e)
    {
        base.OnKeyDown(e);

        if (e.Key == Key.Space)
        {
            if (ClickMode == ClickMode.Release)
            {
                OnClick(e.KeyModifiers);
            }
            e.Handled = true;
        }
    }

    /// <summary>
    /// Invokes the <see cref="ClickWithKeyModifiers"/> event.
    /// </summary>
    protected override void OnClick()
    {
        OnClick(KeyModifiers.None);
    }

    protected virtual void OnClick(KeyModifiers keyModifiers)
    {
        if (!IsEffectivelyEnabled) return;

        if (_isFlyoutOpen)
        {
            CloseFlyout();
        }
        else
        {
            OpenFlyout();
        }

        var e = new RoutedEventArgs(ClickEvent, this);
        RaiseEvent(e);
        var be = new ButtonEventArgs(ClickWithKeyModifiersEvent, this, keyModifiers);
        RaiseEvent(be);

        if (e.Handled || be.Handled || Command?.CanExecute(CommandParameter) != true) return;

        Command.Execute(CommandParameter);
        e.Handled = true;
        be.Handled = true;
    }

    private bool _isFlyoutOpen = false ;

    /// <summary>
    /// Invoked when the button's flyout is opened.
    /// </summary>
    protected override void OnFlyoutOpened()
    {
        _isFlyoutOpen = true ;
    }

    /// <summary>
    /// Invoked when the button's flyout is closed.
    /// </summary>
    protected override void OnFlyoutClosed()
    {
        _isFlyoutOpen = false ;
    }

    /// <inheritdoc/>
    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);

        if (e.Handled && ClickMode == ClickMode.Press)
        {
            OnClick(e.KeyModifiers);
        }
    }

    /// <inheritdoc/>
    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);

        if (e.Handled && ClickMode == ClickMode.Release &&
            this.GetVisualsAt(e.GetPosition(this)).Any(c => this == c || this.IsVisualAncestorOf(c)))
        {
            OnClick(e.KeyModifiers);
        }
    }
}
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;

namespace HLab.Base.Avalonia.Controls;

/// <summary>
/// Represents a spinner control that includes two Buttons.
/// </summary>
[TemplatePart("PART_DecreaseButton", typeof(ButtonWithKeyModifiers))]
[TemplatePart("PART_IncreaseButton", typeof(ButtonWithKeyModifiers))]
[PseudoClasses(":left", ":right")]
public class ButtonSpinnerWithModifiers : ButtonSpinner
{
    public ButtonSpinnerWithModifiers()
    {

    }

    RepeatButton? _decreaseButton;
    /// <summary>
    /// Gets or sets the DecreaseButton template part.
    /// </summary>
    RepeatButton? DecreaseButton
    {
        get => _decreaseButton;
        set
        {
            if (_decreaseButton != null)
            {
                //_decreaseButton.Click  -= OnButtonClick;
                _decreaseButton.PointerPressed -= OnButtonPointerPressed;
                _decreaseButton.PointerReleased -= OnButtonPointerReleased;

            }
            _decreaseButton = value;
            if (_decreaseButton != null)
            {
                //_decreaseButton.Click += OnButtonClick;
                _decreaseButton.PointerPressed += OnButtonPointerPressed;
                _decreaseButton.PointerReleased += OnButtonPointerReleased;
            }
        }
    }

    RepeatButton? _increaseButton;
    /// <summary>
    /// Gets or sets the IncreaseButton template part.
    /// </summary>
    RepeatButton? IncreaseButton
    {
        get => _increaseButton;
        set
        {
            if (_increaseButton != null)
            {
                //_decreaseButton.Click  -= OnButtonClick;
                _increaseButton.PointerPressed -= OnButtonPointerPressed;
                _increaseButton.PointerReleased -= OnButtonPointerReleased;
            }
            _increaseButton = value;
            if (_increaseButton != null)
            {
                //_decreaseButton.Click += OnButtonClick;
                _increaseButton.PointerPressed += OnButtonPointerPressed;
                _increaseButton.PointerReleased += OnButtonPointerReleased;
                _increaseButton.PointerMoved += _increaseButton_PointerMoved;
            }
        }
    }

    private void _increaseButton_PointerMoved(object? sender, PointerEventArgs e)
    {
    }


    /// <inheritdoc />
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        IncreaseButton = e.NameScope.Find<RepeatButton>("PART_IncreaseButton");
        DecreaseButton = e.NameScope.Find<RepeatButton>("PART_DecreaseButton");
        base.OnApplyTemplate(e);
    }

    /// <inheritdoc />
    protected override void OnKeyDown(KeyEventArgs e)
    {
        switch (e.Key)
        {
            case Key.Up:
            {
                if (AllowSpin)
                {
                    OnSpin(new SpinWithModifiersEventArgs(SpinEvent, SpinDirection.Increase, e.KeyModifiers));
                    e.Handled = true;
                }
                break;
            }
            case Key.Down:
            {
                if (AllowSpin)
                {
                    OnSpin(new SpinWithModifiersEventArgs(SpinEvent, SpinDirection.Decrease, e.KeyModifiers));
                    e.Handled = true;
                }
                break;
            }
            case Key.Enter:
            {
                //Do not Spin on enter Key when spinners have focus
                if (((IncreaseButton != null) && (IncreaseButton.IsFocused))
                    || ((DecreaseButton != null) && DecreaseButton.IsFocused))
                {
                    e.Handled = true;
                }
                break;
            }
        }
    }

    /// <inheritdoc />
    protected override void OnPointerWheelChanged(PointerWheelEventArgs e)
    {
        base.OnPointerWheelChanged(e);

        if (AllowSpin && IsKeyboardFocusWithin)
        {
            if (e.Delta.Y != 0)
            {
                var spinnerEventArgs = new SpinWithModifiersEventArgs(SpinEvent, (e.Delta.Y < 0) ? SpinDirection.Decrease : SpinDirection.Increase, e.KeyModifiers ,true);
                OnSpin(spinnerEventArgs);
                e.Handled = true;
            }
        }
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == ButtonSpinnerLocationProperty)
        {
        }
    }

    /// <summary>
    /// Called when user clicks one of the spin buttons.
    /// </summary>
    /// <param name="sender">The event sender.</param>
    /// <param name="e">The event args.</param>
    void OnButtonPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (AllowSpin)
        {
            var direction = sender == IncreaseButton ? SpinDirection.Increase : SpinDirection.Decrease;
            OnSpin(new SpinWithModifiersEventArgs(SpinEvent, direction, e.KeyModifiers));
        }
    }

    private void OnButtonPointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        throw new NotImplementedException();
    }



}
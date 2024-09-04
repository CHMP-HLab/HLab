using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Input;


namespace HLab.Base.Avalonia.Controls;

enum ActualKey
{
    None,
    Backspace,
    Delete,
    Left,
    Right,
    Up,
    Down,
    Home,
    End,
    Enter,
    Tab,
    ShiftTab,
    Insert,
    Escape,
    PageUp,
    PageDown,
    Space,
    DecimalSeparator,
    D0,
    D1,
    D2,
    D3,
    D4,
    D5,
    D6,
    D7,
    D8,
    D9,
    Minus,
    Plus,
}


/// <summary>
/// Control that represents a TextBox with button spinners that allow incrementing and decrementing numeric values.
/// </summary>
[TemplatePart("PART_Spinner", typeof(Spinner))]
[TemplatePart("PART_TextBox", typeof(TextBox))]
public class DoubleBox : TemplatedControl
{
    /// <summary>
    /// Defines the <see cref="Value"/> property.
    /// </summary>
    public static readonly StyledProperty<double> ValueProperty =
        AvaloniaProperty.Register<DoubleBox, double>(nameof(Value),
            defaultBindingMode: BindingMode.TwoWay, enableDataValidation: true);

    /// <summary>
    /// Defines the <see cref="TrailingZeros"/> property.
    /// </summary>
    public static readonly StyledProperty<bool> TrailingZerosProperty =
        AvaloniaProperty.Register<DoubleBox, bool>(nameof(TrailingZeros), true);

    /// <summary>
    /// Defines the <see cref="Decimals"/> property.
    /// </summary>
    public static readonly StyledProperty<int> DecimalsProperty = 
        AvaloniaProperty.Register<DoubleBox, int>(nameof(Decimals), 2);


    /// <summary>
    /// Gets or sets the value.
    /// </summary>
    public double Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    /// <summary>
    /// ?
    /// </summary>
    public bool TrailingZeros
    {
        get => GetValue(TrailingZerosProperty);
        set => SetValue(TrailingZerosProperty, value);
    }

    /// <summary>
    /// ?
    /// </summary>
    public int Decimals
    {
        get => GetValue(DecimalsProperty);
        set => SetValue(DecimalsProperty, value);
    }

    /// <summary>
    /// Gets the Spinner template part.
    /// </summary>
    Spinner? Spinner { get; set; }

    /// <summary>
    /// Gets the TextBox template part.
    /// </summary>
    TextBox? TextBox { get; set; }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == ValueProperty)
        {
            FormatValue();
        }
        else if (change.Property == TrailingZerosProperty)
        {
            FormatValue();
        }
        else if (change.Property == DecimalsProperty)
        {
            FormatValue();
        }
    }


    void FormatValue()
    {
        if(TextBox == null) return;
        // 
        var format = TrailingZeros ? "F" + Decimals : "G" + Decimals;

        // set text from value
        TextBox.Text = Value.ToString(format, CultureInfo.CurrentCulture);
    }

    /// <inheritdoc />
    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        if (TextBox != null)
        {
            TextBox.KeyDown -= TextBox_KeyDown;
            TextBox.GotFocus -= TextBox_GotFocus;
        }

        TextBox = e.NameScope.Find<TextBox>("PART_TextBox");
        if (TextBox != null)
        {
            TextBox.KeyDown += TextBox_KeyDown;
            TextBox.GotFocus += TextBox_GotFocus;
        }

        if (Spinner != null)
        {
        }
        Spinner = e.NameScope.Find<Spinner>("PART_Spinner");
        if (Spinner != null)
        {
        }

        FormatValue();
    }

    void TextBox_GotFocus(object? sender, GotFocusEventArgs e)
    {
        if (TextBox?.Text == null) return;
        TextBox.SelectionStart = 0;
        TextBox.SelectionEnd = TextBox.Text.Length;
    }

    void TextBox_KeyDown(object? sender, KeyEventArgs e)
    {
        e.Handled = HandleChar(GetActualKey(e.Key));
    }

    bool HandleChar(ActualKey k)
    {
        if (k == ActualKey.None) return true;
        if (TextBox == null) return false;

        TextBox.Text ??= "";


        // handle +/- keys
        switch (k)
        {
            case ActualKey.Minus when Value < 0: return true;

            case ActualKey.Minus:

                TextBox.SelectionStart++;
                TextBox.SelectionEnd++;
                Value = -Value;

                return true;

            case ActualKey.Plus when Value > 0: return true;

            case ActualKey.Plus:

                TextBox.SelectionStart--;
                TextBox.SelectionEnd--;

                Value = -Value;

                return true;
        }

        // handle navigation keys
        switch (k)
        {
            case ActualKey.Enter or ActualKey.Tab:
                
                var next = KeyboardNavigationHandler.GetNext(this, NavigationDirection.Next);
                if (next == null) return true;

                next.Focus(NavigationMethod.Directional);

                return true;
                
            case ActualKey.ShiftTab:
                
                var previous = KeyboardNavigationHandler.GetNext(this, NavigationDirection.Previous);
                if (previous == null) return true;

                previous.Focus(NavigationMethod.Directional);
                return true;
                
            case ActualKey.Left:
                
                if (TextBox.SelectionStart <= 0) return true;
                TextBox.SelectionStart--;
                TextBox.SelectionEnd = TextBox.SelectionStart;
                return true;
                
            case ActualKey.Right:
                
                if (TextBox.SelectionStart >= TextBox.Text.Length) return true;
                TextBox.SelectionStart++;
                TextBox.SelectionEnd = TextBox.SelectionStart;
                return true;
                
        }

        var selectionLength = TextBox.SelectionEnd - TextBox.SelectionStart;

        // if any text is selected, delete it
        if (selectionLength > 0)
        {
            TextBox.Text = TextBox.Text.Remove(TextBox.SelectionStart, selectionLength);
            TextBox.SelectionEnd = TextBox.SelectionStart;

            // if backspace or delete was pressed, we're done
            if (k is ActualKey.Backspace or ActualKey.Delete) return true;
        }

        // handle suppression keys
        switch (k)
        {
            // handle the backspace key
            case ActualKey.Backspace when TextBox.SelectionStart <= 0:
                return true;
            case ActualKey.Backspace:
                TextBox.Text = TextBox.Text.Remove(TextBox.SelectionStart - 1, 1);
                TextBox.SelectionStart--;
                return true;
            // handle the delete key
            case ActualKey.Delete when TextBox.SelectionStart >= TextBox.Text.Length:
                return true;
            case ActualKey.Delete:
                TextBox.Text = TextBox.Text.Remove(TextBox.SelectionStart, 1);
                return true;
        }

        var c = GetChar(k);

        // handle decimal separator
        var decimalSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator[0];
        if (k is ActualKey.DecimalSeparator)
        {
            if (TextBox.Text.Contains(decimalSeparator))
            {
                TextBox.SelectionStart = TextBox.Text.IndexOf(decimalSeparator) + 1;
                return true;
            }

            c = decimalSeparator;
        }

        if (c is '\0') return true;

        var content = (TextBox.Text??"").Insert(TextBox.SelectionStart, c.ToString());

        var start = TextBox.SelectionStart;

        Value = double.Parse(content, CultureInfo.CurrentCulture);

        TextBox.SelectionEnd = TextBox.SelectionStart = start+1;

        return true;
    }

    static char GetChar(ActualKey key)
    {
        return key switch
        {
            ActualKey.D0 => '0',
            ActualKey.D1 => '1',
            ActualKey.D2 => '2',
            ActualKey.D3 => '3',
            ActualKey.D4 => '4',
            ActualKey.D5 => '5',
            ActualKey.D6 => '6',
            ActualKey.D7 => '7',
            ActualKey.D8 => '8',
            ActualKey.D9 => '9',
            _ => '\0'
        };
    }

    static ActualKey GetActualKey(Key key)
    {
        switch (key)
        {
            case Key.D0: return ActualKey.D0;
            case Key.D1: return ActualKey.D1;
            case Key.D2: return ActualKey.D2;
            case Key.D3: return ActualKey.D3;
            case Key.D4: return ActualKey.D4;
            case Key.D5: return ActualKey.D5;
            case Key.D6: return ActualKey.D6;
            case Key.D7: return ActualKey.D7;
            case Key.D8: return ActualKey.D8;
            case Key.D9: return ActualKey.D9;

            case Key.NumPad0: return ActualKey.D0;
            case Key.NumPad1: return ActualKey.D1;
            case Key.NumPad2: return ActualKey.D2;
            case Key.NumPad3: return ActualKey.D3;
            case Key.NumPad4: return ActualKey.D4;
            case Key.NumPad5: return ActualKey.D5;
            case Key.NumPad6: return ActualKey.D6;
            case Key.NumPad7: return ActualKey.D7;
            case Key.NumPad8: return ActualKey.D8;
            case Key.NumPad9: return ActualKey.D9;

            case Key.OemMinus: return ActualKey.Minus;
            case Key.OemPlus: return  ActualKey.Plus;

            case Key.Subtract: return ActualKey.Minus;
            case Key.Add: return ActualKey.Plus;

            case Key.OemComma: return ActualKey.DecimalSeparator;
            case Key.OemPeriod: return  ActualKey.DecimalSeparator;
            case Key.Decimal: return ActualKey.DecimalSeparator;

            case Key.Back: return ActualKey.Backspace;
            case Key.Delete: return ActualKey.Delete;
            case Key.Enter: return  ActualKey.Enter;
            case Key.Tab: return ActualKey.Tab;
            case Key.Space: return ActualKey.Space;

            case Key.Up: return ActualKey.Up;
            case Key.Down: return ActualKey.Down;
            case Key.Left: return ActualKey.Left;
            case Key.Right: return ActualKey.Right;

            default: return ActualKey.None;
        }
    }

}
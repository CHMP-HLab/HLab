using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using HLab.Base.Extensions;

namespace HLab.Base.Wpf
{
    using H = DependencyHelper<NumTextBox>;

    public class ValueChangedEventArg : EventArgs
    {
        public int NewValue { get; }

        public ValueChangedEventArg(int value)
        {
            NewValue = value;
        }
    }

    public class NumTextBox : TextBox, IMandatoryNotFilled
    {
        public DependencyProperty MandatoryProperty => ValueProperty;

        public event EventHandler<ValueChangedEventArg> ValueChanged;
        public static readonly DependencyProperty ValueProperty =
            H.Property<int>()
                .Default(0)
                .OnChange((e, a) => e.OnValueChanged(new ValueChangedEventArg(a.NewValue)))
                .Register();

        public static readonly DependencyProperty MinValueProperty =
            H.Property<int>()
                .Default(0)
                .OnChange((e, a) =>
                {
                    if (e.MaxValue < a.NewValue)
                        e.MaxValue = a.NewValue;

                    if (e.Value < a.NewValue)
                        e.Value = a.NewValue;
                }).Register();

        public static readonly DependencyProperty MaxValueProperty =
            H.Property<int>()
                .Default(99)
                .OnChange((e, a) =>
                {
                    if (e.MinValue > a.NewValue)
                        e.MinValue = a.NewValue;

                    if (e.Value > a.NewValue)
                        e.Value = a.NewValue;
                }).Register();

        public static readonly DependencyProperty MandatoryNotFilledProperty = H.Property<bool>()
            .OnChange( (s,a) => s.SetMandatoryNotFilled() )
            .Register();

        public static readonly DependencyProperty ZerosProperty = H.Property<int>()
            .OnChange( (s,a) => s.UpdateText() )
            .Register();

        public static readonly DependencyProperty HideZeroValueProperty = H.Property<bool>()
            .OnChange( (s,a) => s.UpdateText() )
            .Register();

        public int Value
        {
            get => (int) GetValue(ValueProperty);
            set => SetValue(ValueProperty,value);
        }
        public int MinValue
        {
            get => (int) GetValue(MinValueProperty);
            set => SetValue(MinValueProperty,value);
        }
        public int MaxValue
        {
            get => (int) GetValue(MaxValueProperty);
            set => SetValue(MaxValueProperty,value);
        }
        public bool MandatoryNotFilled
        {
            get => (bool)GetValue(MandatoryNotFilledProperty);
            set => SetValue(MandatoryNotFilledProperty, value);
        }
        public int Zeros
        {
            get => (int)GetValue(ZerosProperty);
            set => SetValue(ZerosProperty, value);
        }
        public bool HideZeroValue
        {
            get => (bool)GetValue(HideZeroValueProperty);
            set => SetValue(HideZeroValueProperty, value);
        }

        static string FormatZeros(int value, int zeros, bool hideZeroValue)
        {
            if (hideZeroValue && value == 0) return "";

            var text = $"{value}";

            if (text.Length < zeros)
            {
                text = $"{new string('0', zeros)}{text}".Right(zeros);
            }

            return text;
        }

        static int ParseTextToValue(string text)
        {
            if(int.TryParse(text,out var value))
            {
                return value;
            }

            return 0;
        }

        protected virtual void OnValueChanged(ValueChangedEventArg arg)
        {
            UpdateText();
            ValueChanged?.Invoke(this,arg);
        }

        protected override void OnPreviewTextInput(TextCompositionEventArgs e)
        {
            base.OnPreviewTextInput(e);

            if ("/:".Contains(e.Text) && !string.IsNullOrWhiteSpace(Text))
            {
                e.Handled = true;
                MoveNext();
            }

            var regex = new Regex(@"^[-+]?\d*$");
            e.Handled = !regex.IsMatch(e.Text);
        }

        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            base.OnTextChanged(e);

            var text = Text;
            var i = SelectionStart;
            var l = SelectionLength;

            var left = text[..i];
            var right = text[(i + l)..];

            if (ParseTextToValue($"{left}0{right}") > MaxValue) 
                e.Handled = MoveNext();

            if (ParseTextToValue($"1{text}") > MaxValue) 
                e.Handled = MoveNext();
        }

        void SetMandatoryNotFilled()
        {
            if (MandatoryNotFilled)
            {
                BorderThickness = new Thickness(1);
                BorderBrush = new SolidColorBrush(Colors.DarkRed);
            }
            else
            {
                BorderThickness = new Thickness(0);
                BorderBrush = new SolidColorBrush(Colors.Transparent);
            }
        }

        void UpdateText()
        {
            Text = FormatZeros(Value, Zeros, HideZeroValue);
        }

        protected override void OnMouseDoubleClick(MouseButtonEventArgs e)
        {
            base.OnMouseDoubleClick(e);
            base.OnGotMouseCapture(e);
            if(e.Source is NumTextBox n)
                n.SelectAll();
        }


        protected override void OnGotKeyboardFocus(KeyboardFocusChangedEventArgs e)
        {
            base.OnGotKeyboardFocus(e);
            if(e.Source is NumTextBox n)
                n.SelectAll();
        }

        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseLeftButtonDown(e);
            SelectivelyIgnoreMouseButton(e);
        }

        bool MoveNext()
        {
            if(IsFocused)
                return MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            else
                return true;
        }

        static void SelectAllText(RoutedEventArgs e)
        {
            if (e.OriginalSource is NumTextBox textBox)
                textBox.SelectAll();
        }

        int ConstrainedValue(int value)
        {
            if (value > MaxValue) value = MaxValue;
            if (value < MinValue) value = MinValue;
            return value;
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);
            Value = ConstrainedValue(ParseTextToValue(Text));
            UpdateText();
        }

        static void SelectivelyIgnoreMouseButton( MouseButtonEventArgs e)
        {
            // Find the TextBox
            DependencyObject parent = e.OriginalSource as UIElement;
            while (parent != null && !(parent is TextBox))
                parent = VisualTreeHelper.GetParent(parent);

            if (parent != null)
            {
                var textBox = (TextBox)parent;
                if (!textBox.IsKeyboardFocusWithin)
                {
                    // If the text box is not yet focussed, give it the focus and
                    // stop further processing of this click event.
                    textBox.Focus();
                    e.Handled = true;
                }
            }
        }
    }
}

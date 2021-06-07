using System;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using HLab.Base.Wpf;

namespace HLab.Base
{
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
        private class H : DependencyHelper<NumTextBox> { }

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
            .OnChange( (s,a) => s.SetMandatoryNotFilled(a.NewValue) )
            .Register();

        public static readonly DependencyProperty ShowZeroProperty = H.Property<bool>()
            .OnChange( (s,a) => s.SetShowZero(a.NewValue) )
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
        public bool ShowZero
        {
            get => (bool)GetValue(ShowZeroProperty);
            set => SetValue(ShowZeroProperty, value);
        }

        protected virtual void OnValueChanged(ValueChangedEventArg arg)
        {

            if(ShowZero)
                Text = arg.NewValue.ToString();
            else
                Text = arg.NewValue == 0 ? "" : arg.NewValue.ToString();

            ValueChanged?.Invoke(this,arg);

        }

        protected override void OnPreviewTextInput(TextCompositionEventArgs e)
        {
            base.OnPreviewTextInput(e);

            if (e.Text.Contains('/') && !string.IsNullOrWhiteSpace(Text))
            {
                MoveNext();
            }

            var regex = new Regex(@"^[-+]?\d*$");
            e.Handled = !regex.IsMatch(e.Text);

        }

        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            base.OnTextChanged(e);

            var text = Text;
            var s = SelectionStart;
            var removed = 0;

            while (text.StartsWith('0') && text != "0")
            {
                text = text.Substring(1);
                if(s>0) s--;
                removed++;
            }

            while (true)
            {

                if (!int.TryParse(text, out var i))
                {
                    if(!string.IsNullOrWhiteSpace(text)) return;
                    i = 0;
                }

                if (i > MaxValue) 
                {
                    if (s == text.Length)
                    {
                        Value = MaxValue;
                        e.Handled = MoveNext();
                        return;
                    }

                    text = text.Substring(0, s) + text.Substring(s + 1);
                    SelectionStart = s;
                    e.Handled = true;
                    continue;
                }

                if (i*10 > MaxValue && s == text.Length)
                {
                    Value = i;
                    e.Handled = MoveNext();
                    return;
                }

                if (removed > 0 && Math.Pow(10 , Text.Length) > MaxValue)
                {
                    Value = i;
                    e.Handled = MoveNext();
                    return;
                }

                Value = i;
                SelectionStart = s;
                e.Handled = true;
                return;
            }

        }
        private void SetMandatoryNotFilled(bool mnf)
        {
            if (mnf)
            {
                BorderThickness = new Thickness(1);
                BorderBrush = new SolidColorBrush(Colors.DarkRed);
            }
            else
            {
                BorderThickness = new Thickness(0);
                BorderBrush = new SolidColorBrush(Colors.Transparent);
            }
//            Mandatory.Visibility = mnf ? Visibility.Visible : Visibility.Collapsed;
        }

        private void SetShowZero(bool value)
        {
            if (value)
            {
                if (Value == 0) Text = "0";
            }
            else
            {
                if (Value == 0) Text = "";
            }
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

        private static void SelectAllText(RoutedEventArgs e)
        {
            if (e.OriginalSource is NumTextBox textBox)
                textBox.SelectAll();
        }

        protected override void OnLostFocus(RoutedEventArgs e)
        {
            base.OnLostFocus(e);
            if (ShowZero)
            {
                if (string.IsNullOrWhiteSpace(Text))
                {
                    Value = 0;
                }
            }
            else
            {
                if (int.TryParse(Text, out var i))
                {
                    if (i == 0) Text = "";
                }
            }
        }

        private static void SelectivelyIgnoreMouseButton( MouseButtonEventArgs e)
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

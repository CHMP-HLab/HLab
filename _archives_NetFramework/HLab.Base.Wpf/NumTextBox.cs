using System;
using System.Diagnostics.Eventing.Reader;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace HLab.Base.Wpf
{
    public class NumTextBox : TextBox
    {
        private class H : DependencyHelper<NumTextBox> { }

        public event EventHandler ValueChanged;
        public static readonly DependencyProperty ValueProperty =
            H.Property<int>()
                .Default(0)
                .OnChange((e, a) =>
            {
                e.Text = a.NewValue.ToString();
                e.ValueChanged?.Invoke(e,new EventArgs());
            }).Register();

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

        protected override void OnPreviewTextInput(TextCompositionEventArgs e)
        {
            base.OnPreviewTextInput(e);
            Regex regex = new Regex(@"^[-+]?\d*$");
            e.Handled = regex.IsMatch(e.Text);

        }

        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            base.OnTextChanged(e);
            while(true)
            if (int.TryParse(Text, out var i))
            {
                var s = SelectionStart;
                if (i > MaxValue) 
                {
                    if (s == Text.Length)
                    {
                        Value = MaxValue;
                        e.Handled = MoveNext();
                        return;
                    }

                    Text = Text.Substring(0, s) + Text.Substring(s + 1);
                    SelectionStart = s;
                    e.Handled = true;
                    continue;
                }
                else if (i*10 > MaxValue && s == Text.Length)
                {
                    e.Handled = MoveNext();
                    return;
                }

                e.Handled = true;
                return;
            }
            else return;

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

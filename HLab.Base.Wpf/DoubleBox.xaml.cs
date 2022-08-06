using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;

namespace HLab.Base.Wpf
{
    using H = DependencyHelper<DoubleBox>;

    /// <summary>
    /// Logique d'interaction pour DoubleBox.xaml
    /// </summary>
    public partial class DoubleBox : UserControl, IDoubleProvider
    {
        public DoubleBox()
        {
            InitializeComponent();

            FormatDouble();
        }





        //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        // Double

        public static readonly DependencyProperty DoubleProperty =
            H.Property<double>()
                .Default(0.0)
                .OnChange((e, a) => e.OnDoubleChanged(a.OldValue, a.NewValue))
                .BindsTwoWayByDefault
                .Register();

        public double Double
        {
            set => SetValue(DoubleProperty, value);
            get => (double)GetValue(DoubleProperty);
        }

        readonly bool _preventFormat = false;

        void OnDoubleChanged(double oldValue, double value)
        {
            if (!_preventFormat)
            {
                FormatDouble();
            }
            RaiseEvent(new RoutedEventArgs(DoubleChangedEvent, this));
        }

        public static readonly RoutedEvent DoubleChangedEvent = H.Event<RoutedEvent>().Bubble.Register();
        public event RoutedEventHandler DoubleChanged
        {
            add => AddHandler(DoubleChangedEvent, value);
            remove => RemoveHandler(DoubleChangedEvent, value);
        }

        public static implicit operator double(DoubleBox doubleBox) => doubleBox?.Double ?? default;

        //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        // Decimals

        public static readonly DependencyProperty DecimalsProperty =
            H.Property<int>()
                .Default(2)
                .OnChange((e, a) => e.OnDecimalsChanged(a.NewValue))
                .Register();

        public int Decimals
        {
            set => SetValue(DecimalsProperty, value);
            get => (int)GetValue(DecimalsProperty);
        }

        void OnDecimalsChanged(int value)
        {
            if (double.IsFinite(Double))
            {
                FormatDouble();
            }
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        // DisplayZeros

        public static readonly DependencyProperty DisplayZerosProperty =
            H.Property<DisplayZeros>()
                .Default(DisplayZeros.Always)
                .Register();

        public DisplayZeros DisplayZeros
        {
            set => SetValue(DisplayZerosProperty, value);
            get => (DisplayZeros)GetValue(DisplayZerosProperty);
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        // IsReadOnly

        public static readonly DependencyProperty IsReadOnlyProperty =
            H.Property<bool>()
                .Default(false)
                .Register();

        public bool IsReadOnly
        {
            set => SetValue(IsReadOnlyProperty, value);
            get => (bool)GetValue(IsReadOnlyProperty);
        }

        //Privates
        void FormatDouble()
        {
            if (double.IsNaN(Double))
            {
                TextBox.Text = "-";
                return;
            }
            if (double.IsFinite(Double))
            {
                TextBox.Text = Double.ToString($"#,0.{new string('0', Decimals)}");
            }
        }

        void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (TextBox.SelectionLength == 0)
                TextBox.SelectAll();
        }

        void TextBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            TextBox.LostMouseCapture += TextBox_LostMouseCapture;
        }

        void TextBox_LostMouseCapture(object sender, MouseEventArgs e)
        {
            // If user highlights some text, don't override it
            if (TextBox.SelectionLength == 0)
                TextBox.SelectAll();

            // further clicks will not select all
            TextBox.LostMouseCapture -= TextBox_LostMouseCapture;
        }

        void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            foreach (var c in e.Text)
            {
                var text = TextBox.Text;
                var selectionStart = TextBox.SelectionStart;
                var separatorPos = text.IndexOf(Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator);

                switch (c)
                {
                    case '.':
                    case ',':
                    {
                        if (separatorPos >= 0)
                        {
                            if (separatorPos >= TextBox.SelectionStart)
                            {
                                TextBox.SelectionStart = separatorPos + 1;
                                TextBox.SelectionLength = TextBox.Text.Length - TextBox.SelectionStart;
                                e.Handled = true;
                                return;
                            }

                            e.Handled = true;
                            return;
                        }

                        break;
                    }
                    case '-' when Double >= 0:
                        TextBox.Text = $"-{text.Trim()}";
                        TextBox.SelectionStart = selectionStart + 1;
                        e.Handled = true;
                        return;

                    case '+' when TextBox.Text.Contains("-"):
                        TextBox.Text = text.Replace("-", "");
                        TextBox.SelectionStart = Math.Max(selectionStart + 1, 0);
                        e.Handled = true;
                        return;
                }

                if ("0123456789.,".Contains(c)) continue;

                e.Handled = true;
                return;
            }
        }


        string _oldText = "";

        void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (IsReadOnly) return;

            var offset = e.Changes.First().Offset;
            var text = TextBox.Text;

            var selection = TextBox.SelectionStart;
            var selectionLength = TextBox.SelectionLength;

            var separator = Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator;

            if (
                e.Changes.Count > 0
                && e.Changes.First().RemovedLength == 1
                && _oldText.Length > offset
                && _oldText[offset] == ',')
            {
                text = _oldText[..(offset - 1)] + _oldText[offset..];
                selection--;
            }

            _oldText = TextBox.Text;

            // if french separator is used but numpad has point
            if (separator == ",")
            {
                text = text.Replace(".", separator);
            }

            // put [] markers for actual selection
            text = text[..selection] + "[" + text[selection..(selection + selectionLength)] + "]" + text[(selection + selectionLength)..];

            // Add front zero when missing (starts with separator)
            if (text.StartsWith(separator)) text = '0' + text;

            // Add tailling zeros
            if (text.EndsWith(separator)) text += new string('0', Decimals);
            if (text.EndsWith(separator + "[]")) text += new string('0', Decimals);

            //Find separator
            var separatorPos = text.IndexOf(separator, StringComparison.Ordinal);

            //If no separator lets add one
            if (separatorPos < 0)
            {
                separatorPos = text.Length;
                text += separator + new string('0', Decimals);
            }

            // Cut or add missing exeeding decimals
            var q = text.Substring(separatorPos + 1).Replace("[", "").Replace("]", "");
            var n = Decimals - q.Length;

            switch (n)
            {
                case > 0:
                    text += new string('0', Decimals - q.Length);
                    break;

                case < 0:
                    text = text[..(text.Length + n)];
                    break;
            }

            var textOut = "";
            n = 0;

            var neg = false;
            var prefix = "";
            var atSelection = false;

            // Format integer part
            for (var i = separatorPos - 1; i >= 0; i--)
            {
                var c = text[i];

                if (c == '-') neg = true;

                switch (c)
                {
                    case '-':
                        neg = true;
                        break;
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        if (atSelection && prefix == " " && e.Changes.First().RemovedLength == 1)
                        { }
                        else
                        {
                            textOut = c + prefix + textOut;

                            n++;
                            if (n == 3)
                            {
                                n = 0;
                                prefix = " ";
                            }
                            else
                                prefix = "";
                        }
                        atSelection = false;
                        break;

                    case '[':
                        textOut = c + textOut;
                        atSelection = true;
                        break;
                    case ']':
                        textOut = c + textOut;
                        atSelection = false;
                        break;
                    case ' ':
                        atSelection = false;
                        break;
                    default:
                        atSelection = false;
                        break;
                }
            }

            // Format decimal part
            bool needSeparator = true;
            n = 0;

            for (int i = separatorPos + 1; i < text.Length; i++)
            {
                if (needSeparator)
                {
                    textOut += separator;
                    needSeparator = false;
                }

                var c = text[i];

                switch (c)
                {
                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':
                        if (n < Decimals) textOut += c;
                        n++;
                        break;
                    case '[':
                    case ']':
                        textOut += c;
                        break;
                }
            }

            while ((textOut.StartsWith('0') || textOut.StartsWith(' ')) && !textOut.StartsWith("0" + separator))
            {
                textOut = textOut[1..];
            }

            textOut = textOut.Trim();
            if (neg) textOut = "-" + textOut.Trim();
            textOut += "[]";

            selection = textOut.IndexOf('[');
            textOut = textOut.Replace("[", "");

            selectionLength = textOut.IndexOf(']') - selection;
            textOut = textOut.Replace("]", "");

            textOut = textOut.Trim();


            TextBox.Text = textOut;
            TextBox.SelectionStart = selection;
            TextBox.SelectionLength = selectionLength;

        }


        double DeltaWithModifierKey(double delta)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Control) != 0) return delta / 10;
            if ((Keyboard.Modifiers & ModifierKeys.Shift) != 0) return delta * 10;
            return delta;
        }

        [DllImport("User32.dll")]
        static extern bool SetCursorPos(int X, int Y);

        static void SetCursorPos(Point p) => SetCursorPos((int)Math.Round(p.X,0), (int)Math.Round(p.Y,0));

        void Shift(double delta, object sender,  MouseEventArgs args)
        {
            Double += delta;

            if (sender is FrameworkElement fe)
            {
                var rx = 0.5;
                var ry = 0.5;

                if(args!=null)
                {
                    var p = args.GetPosition(fe);
                    rx = p.X / fe.ActualWidth;
                    ry = p.Y / fe.ActualHeight;
                }

                Dispatcher.BeginInvoke(() =>
                {
                    var p2 = new Point(rx * fe.ActualWidth, ry * fe.ActualHeight);
                    SetCursorPos(fe.PointToScreen(p2));

                }, DispatcherPriority.Loaded);
            }
            
        }

        void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            Shift(DeltaWithModifierKey((e.Delta > 0) ? 1.0 : -1.0),sender,e);
        }

        void Click_Up(object sender, RoutedEventArgs e)
        {
            Shift(DeltaWithModifierKey(1.0),sender,null);
        }

        void Click_Down(object sender, RoutedEventArgs e)
        {
            Shift(DeltaWithModifierKey(-1.0),sender,null);

        }

        void UpdateDoubleFromText()
        {
            Double = double.TryParse(TextBox.Text, out var value) ? value : double.NaN;
        }

        void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if(IsReadOnly) return;

            UpdateDoubleFromText();
        }

        void OnKeyEnterUpdate(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                UpdateDoubleFromText();
            }
        }
    }
}

using System;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

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

            TextBox.TextChanged += TextBox_TextChanged;
            TextBox.PreviewTextInput += TextBox_PreviewTextInput;
            TextBox.TextInput += TextBox_TextInput;
            TextBox.LostMouseCapture += TextBox_LostMouseCapture;
            TextBox.LostKeyboardFocus += TextBox_LostKeyboardFocus;
            TextBox.GotFocus += TextBox_GotFocus;
            TextBox.LostFocus += TextBox_LostFocus;

            FormatDouble();
        }

        private void FormatDouble()
        {
            if(double.IsNaN(Double))
            {
                TextBox.Text = "-";
                return;
            }
            if(double.IsFinite(Double))
            {
                TextBox.Text = Double.ToString($"#,0.{new string('0', Decimals)}");
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (TextBox.SelectionLength == 0)
                TextBox.SelectAll();
        }

        private void TextBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            TextBox.LostMouseCapture += TextBox_LostMouseCapture;
        }

        private void TextBox_LostMouseCapture(object sender, MouseEventArgs e)
        {
            // If user highlights some text, don't override it
            if (TextBox.SelectionLength == 0)
                TextBox.SelectAll();

            // further clicks will not select all
            TextBox.LostMouseCapture -= TextBox_LostMouseCapture;
        }

        private void TextBox_TextInput(object sender, TextCompositionEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            foreach(char c in e.Text)
            {
                var text = TextBox.Text;
                var selectionStart = TextBox.SelectionStart;
                var separatorPos = text.IndexOf(Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator);

                if(c=='.' || c ==',')
                {
                    if (separatorPos >= 0)
                    {
                        if(separatorPos>=TextBox.SelectionStart)
                        {
                            TextBox.SelectionStart = separatorPos+1;
                            TextBox.SelectionLength = TextBox.Text.Length - TextBox.SelectionStart;
                            e.Handled = true;
                            return;
                        }
                        else
                        {
                                e.Handled = true;
                                return;
                        }
                    }
                }

                if(c=='-')
                {
                    if(Double>0)
                    {
                        TextBox.Text = $"-{text.Trim()}";
                        TextBox.SelectionStart = selectionStart+1;
                        e.Handled = true;
                        return;
                    }                        
                }

                if(c=='+')
                {
                    if(TextBox.Text.Contains("-"))
                    {
                        TextBox.Text = text.Replace("-","");
                        TextBox.SelectionStart = Math.Max(selectionStart+1,0);
                        e.Handled = true;
                        return;
                    }
                }

                if(!"0123456789.,".Contains(c))
                {
                    e.Handled = true;
                    return;
                }
            }
        }


        string oldText = "";
        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(IsReadOnly) return;

            var offset = e.Changes.First().Offset;
            var text = TextBox.Text;

            var selection = TextBox.SelectionStart;
            var selectionLength =  TextBox.SelectionLength;

            var separator = Thread.CurrentThread.CurrentCulture.NumberFormat.NumberDecimalSeparator;

            if(
                e.Changes.Count > 0 
                && e.Changes.First().RemovedLength == 1 
                && oldText.Length > offset 
                && oldText[offset] == ',' )
            {
                text = oldText[0..(offset-1)] + oldText[offset..];
                selection--;
            }

            oldText = TextBox.Text;

            // if french separator is used but numpad has point
            if(separator == ",")
            {
                text = text.Replace(".",separator);
            }

            // put [] markers for actual selection
            text = text[..selection] + "[" + text[selection..(selection+selectionLength)] + "]" + text[(selection + selectionLength)..];

            // Add front zero when missing (starts with separator)
            if(text.StartsWith(separator)) text = '0' + text;

            // Add tailling zeros
            if(text.EndsWith(separator))  text += new string('0',Decimals);
            if(text.EndsWith(separator+"[]"))  text += new string('0',Decimals);

            //Find separator
            var separatorPos = text.IndexOf(separator);

            //If no separator lets add one
            if(separatorPos<0)
            {
                separatorPos = text.Length;
                text += separator + new string('0',Decimals);
            }

            // Cut or add missing exeeding decimals
            var q = text.Substring(separatorPos+1).Replace("[","").Replace("]","");
            int n = Decimals-q.Length;

            if(n>0)
                text += new string('0',Decimals-q.Length);
            if(n<0)
                text = text.Substring(0,text.Length+n);

            var textOut ="";
            n = 0;

            var neg = false;
            var prefix = "";
            bool atSelection = false;

            // Format integer part
            for(int i = separatorPos-1; i>=0; i--)
            {
                var c = text[i];

                if(c=='-') neg = true;
                
                if("0123456789".Contains(c))
                {
                    if(atSelection &&  prefix==" " && e.Changes.First().RemovedLength==1 )
                    { }
                    else
                    {
                        textOut = c + prefix + textOut;

                        n++;
                        if(n==3)
                        {
                            n=0;
                            prefix = " ";
                        }
                        else 
                            prefix = "";
                    }
                    atSelection=false;
                }
                else if(c=='[')
                {
                    textOut = c + textOut;
                    atSelection = true;
                }
                else if(c==']')
                {
                    textOut = c + textOut;
                    atSelection=false;
                }
                else if(c==' ')
                {
                    atSelection = false;
                }
                else atSelection = false;
            }

            // Format decimal part
            bool needSeparator = true;
            n = 0;

            for(int i = separatorPos+1; i<text.Length; i++)
            {
                if(needSeparator)
                {
                    textOut += separator; 
                    needSeparator = false;
                }

                var c = text[i];

                if("0123456789".Contains(c))
                {
                    if(n<Decimals) textOut += c;
                    n++;
                }
                else if(c=='[' || c==']')
                    textOut += c;
            }

            while((textOut.StartsWith('0') || textOut.StartsWith(' ')) && !textOut.StartsWith("0"+separator))
            {
                textOut = textOut[1..];
            }

            textOut = textOut.Trim();
            if(neg) textOut = "-" + textOut.Trim();
            textOut+="[]";

            selection = textOut.IndexOf('[');
            textOut = textOut.Replace("[", "");

            selectionLength = textOut.IndexOf(']') - selection;
            textOut = textOut.Replace("]", "");

            textOut = textOut.Trim();


            TextBox.Text = textOut;
            TextBox.SelectionStart = selection;
            TextBox.SelectionLength = selectionLength;

            var value = 0.0;
            _preventFormat = true;
            if(double.TryParse(TextBox.Text, out value) && !IsReadOnly) Double = value;
            else Double = Double.NaN;
            _preventFormat = false;
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        // Double

        public static readonly DependencyProperty DoubleProperty =
            H.Property<double>()
                .Default(0.0)
                .OnChange((e, a) => e.OnDoubleChanged(a.OldValue,a.NewValue))
                .BindsTwoWayByDefault
                .Register();

        public double Double
        {
            set => SetValue(DoubleProperty, value);
            get => (double)GetValue(DoubleProperty);
        }

        private bool _preventFormat = false;
        private void OnDoubleChanged(double oldValue,double value)
        {
            if(!_preventFormat)
            {
                FormatDouble();
            }
            RaiseEvent(new RoutedEventArgs(DoubleChangedEvent, this));
        }

        public static readonly RoutedEvent DoubleChangedEvent = H.Event<RoutedEvent>().Bubble.Register();
        public event RoutedEventHandler DoubleChanged
        {
            add { AddHandler(DoubleChangedEvent, value); }
            remove { RemoveHandler(DoubleChangedEvent, value); }
        }

        public static implicit operator double(DoubleBox doubleBox) => doubleBox?.Double??default;

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

        private void OnDecimalsChanged(int value)
        {
            if(double.IsFinite(Double))
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



    }
}

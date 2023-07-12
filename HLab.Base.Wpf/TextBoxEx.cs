/*
 * TextBoxEx
 * 
 * Date : 21/05/2013
 * 
 * *********************************************************************************************************************************************************************************************************************************************************************/

using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

// A FAIRE : Téléphone, email, RCS, Siret, Siren... etc (masque ?)

namespace HLab.Base.Wpf
{
    using H = DependencyHelper<TextBoxEx>;
    //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
    // AffichageZero

    public enum TextBoxMode
    {
        /// <summary>
        /// Mode normal du TextBox
        /// </summary>
        Text = 0,
        Texte = 0,
        /// <summary>
        /// N'accepte que des chiffres 
        /// </summary>
        Digit = 1,
        Chiffre = 1,
        /// <summary>
        /// Un nombre entier
        /// </summary>
        Integer = 2,
        Entier = 2,
        /// <summary>
        /// Un nombre décimal avec un nombre de chiffres après la virgule fixe
        /// </summary>
        Double = 3,

        /// <summary>
        /// Un nombre décimal qui n'affiche les chiffres après la virgule (nombre fixe) que si la valeur n'est pas entière
        /// </summary>
        Number = 4,
        Nombre = 4,
        /// <summary>
        /// Pour une date
        /// </summary>
        Date = 5,

        /// <summary>
        /// Pour une heure
        /// </summary>
        Time = 6,
        Heure = 6,
        /// <summary>
        /// Adresse e-mail
        /// </summary>
        Email = 7
    }

    public class TextBoxEx : TextBox, IDoubleProvider
    {
        //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        // Modes






        //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        // Mode

        public static implicit operator double(TextBoxEx tbe) => tbe.Double;

        public static readonly DependencyProperty ModeProperty = H
            .Property<TextBoxMode>()
            .Default(TextBoxMode.Text)
            .OnChange((e, a) => e.OnModeChanged(a.NewValue))
            .Register();

        public TextBoxMode Mode
        {
            set => SetValue(ModeProperty, value);
            get => (TextBoxMode)GetValue(ModeProperty);
        }

        public void OnModeChanged(TextBoxMode mode)
        {
            if (mode == TextBoxMode.Email)
            {
                TextChanged += Ex_TextChanged;
            }
            else if (mode != TextBoxMode.Text)
            {
                var decimals = Decimals;
                var displayZeros = DisplayZeros;
                var doubleValue = Double;
                var dateTimeValue = Date;
                var valid = true;
                var selectionStart = SelectionStart;
                var selectionLength = SelectionLength;

                var txt = Cleanup(Text, mode, ref selectionStart, ref selectionLength);
                Text = Format(mode, decimals, displayZeros, txt, ref selectionStart, ref doubleValue, ref dateTimeValue, ref valid);
            }
        }

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
        // Decimals

        public static readonly DependencyProperty DecimalsProperty =
            H.Property<int>()
                .Default(2)
                .Register();

        public int Decimals
        {
            set => SetValue(DecimalsProperty, value);
            get => (int)GetValue(DecimalsProperty);
        }

        //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        // Date

        public static readonly DependencyProperty DateProperty =
            H.Property<DateTime>()
                .Default(DateTime.MinValue)
                .OnChange((e, a) => e.OnDateChanged(a.NewValue))
                .Register();

        public DateTime Date
        {
            set => SetValue(DateProperty, value);
            get => (DateTime)GetValue(DateProperty);
        }

        void OnDateChanged(DateTime date)
        {
            if (_preventChange) return;

            Text = date == DateTime.MinValue ? "__ / __ / ____" : date.ToString("dd / MM / yyyy");
        }


        public static readonly RoutedEvent DateChangeEvent = H.Event().Bubble.Register();

        public event RoutedEventHandler DateChange
        {
            add => AddHandler(DateChangeEvent, value);
            remove => RemoveHandler(DateChangeEvent, value);
        }

        public void SetNow()
        {
            Date = DateTime.Now;
        }


        //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        // Double

        //[BindableAttribute(true, BindingDirection.TwoWay)]
        //public static readonly DependencyProperty DoubleProperty = DependencyProperty.RegisterAttached("Double", typeof(double), typeof(TextBoxEx), new UIPropertyMetadata(0.0, DoubleChanged));
        public static readonly DependencyProperty DoubleProperty =
            H.Property<double>()
                .Default(0.0)
                .OnChange((e, a) => e.OnDoubleChanged(a.NewValue))
                .BindsTwoWayByDefault
                .Register();
        //DependencyProperty.Register("Double", typeof(double), typeof(TextBoxEx), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, DoubleChanged));

        public double Double
        {
            set => SetValue(DoubleProperty, value);
            get => (double)GetValue(DoubleProperty);
        }

        void OnDoubleChanged(double value)
        {
            if (_preventChange) return;

            var mode = Mode;
            var decimals = Decimals;
            var displayZeros = DisplayZeros;

            var selectionStart = 0;

            var valueDouble = 0.0;
            var dateTimeValue = DateTime.MinValue;
            var valid = true;

            var text = Format(mode, decimals, displayZeros, Math.Round(value,Decimals).ToString(CultureInfo.CurrentCulture), ref selectionStart, ref valueDouble, ref dateTimeValue, ref valid);

            if(Text != text) Text = text; 

            if(!IsReadOnly) Double = valueDouble;
        }

        public static readonly RoutedEvent DoubleChangeEvent = EventManager.RegisterRoutedEvent("DoubleChange", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(TextBoxEx));

        public event RoutedEventHandler DoubleChange
        {
            add { AddHandler(DoubleChangeEvent, value); }
            remove { RemoveHandler(DoubleChangeEvent, value); }
        }


        //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        // IsValid

        public static readonly DependencyProperty IsValidProperty = H.Property<bool>().Default(true).Register();

        public bool IsValid
        {
            set
            {
                if (IsValid == value) return;
                SetValue(IsValidProperty, value);
                RaiseEvent(new RoutedEventArgs(IsValidChangedEvent, this));
            }
            get => (bool)GetValue(IsValidProperty);
        }

        public static readonly RoutedEvent IsValidChangedEvent = H.Event().Bubble.Register();

        public event RoutedEventHandler IsValidChanged
        {
            add => AddHandler(IsValidChangedEvent, value);
            remove => RemoveHandler(IsValidChangedEvent, value);
        }

        bool _preventChange = false;


        //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        // Eclaire

        public static readonly DependencyProperty EnlightenedProperty = H.Property<bool>().Default(false).Register();
        //DependencyProperty.RegisterAttached("Eclaire", typeof(bool), typeof(TextBoxEx), new UIPropertyMetadata(false));

        public static bool GetEnlightened(DependencyObject d)
        {
            return (bool)d.GetValue(EnlightenedProperty);
        }

        public static void SetEnlightened(DependencyObject d, bool value)
        {
            d.SetValue(EnlightenedProperty, value);
        }


        /********************************************************************************************************************************************************************************************************************************************************************************
        * 
        * Texte qui change
        * 
        ***********************************************************************************************************************************************************************************************************************************************************************************/

        static void Ex_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBoxEx tb)
            {
                // Vérifie l'adresse e-mail
                if (tb.Mode == TextBoxMode.Email)
                {
                    var valid = ValidateEmail(tb.Text);

                    if (tb.IsValid == valid) return;

                    tb.IsValid = valid;
                }
            }
        }


        /********************************************************************************************************************************************************************************************************************************************************************************
        * 
        * Touches
        * 
        ***********************************************************************************************************************************************************************************************************************************************************************************/

        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            if (Mode == TextBoxMode.Email || Mode == TextBoxMode.Text) return;

            e.Handled = true;

            if (IsReadOnly) return;

            //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            // Convertie la touche en lettre

            char letter;
            switch (e.Key)
            {
                // 0
                case Key.NumPad0:
                case Key.D0: letter = '0'; break;

                // 1
                case Key.NumPad1:
                case Key.D1: letter = '1'; break;

                // 2
                case Key.NumPad2:
                case Key.D2: letter = '2'; break;

                // 3
                case Key.NumPad3:
                case Key.D3: letter = '3'; break;

                // 4
                case Key.NumPad4:
                case Key.D4: letter = '4'; break;

                // 5
                case Key.NumPad5:
                case Key.D5: letter = '5'; break;

                // 6
                case Key.NumPad6:
                case Key.D6: letter = '6'; break;

                // 7
                case Key.NumPad7:
                case Key.D7: letter = '7'; break;

                // 8
                case Key.NumPad8:
                case Key.D8: letter = '8'; break;

                // 9
                case Key.NumPad9:
                case Key.D9: letter = '9'; break;

                // Virgule
                case Key.Decimal:
                case Key.OemComma:
                case Key.OemPeriod: letter = Separator; break;

                // Retour
                case Key.Back: letter = 'R'; break;

                // Supprimer
                case Key.Delete: letter = 'S'; break;

                // Droite
                case Key.Right: letter = 'D'; break;

                // Gauche
                case Key.Left: letter = 'G'; break;

                // Touches a laisser passer
                case Key.Up:
                case Key.Down:
                case Key.Enter:
                case Key.Tab:
                case Key.PageUp:
                case Key.PageDown:
                case Key.Home:
                case Key.End: e.Handled = false; return;

                // Les autres touches à interdire
                default: return;
            }


            //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            // Paramètres existants

            var selectionStart = SelectionStart;
            var selectionLength = SelectionLength;


            //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            // Traite les cas particuliers 

            switch (Mode)
            {
                case TextBoxMode.Integer when Text == "0" && selectionStart == 0 && selectionLength == 0:
                case TextBoxMode.Double or TextBoxMode.Number when selectionStart == 0 && selectionLength == 0 && Text.Length > 0 && Text[0] == '0':
                    selectionStart = 1;
                    break;
            }


            //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            // Nettoye la chaine de caractère en fonction du mode

            var txt = Cleanup(Text, Mode, ref selectionStart, ref selectionLength);


            //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            // Traite la chaine nettoyée

            if (letter == 'D')
            {
                if (selectionStart < txt.Length)
                    selectionStart++;
            }
            else if (letter == 'G')
            {
                if (selectionStart > 0)
                    selectionStart--;
            }
            else if (letter == 'R')
            {
                // Remplace par des '_'
                if (Mode == TextBoxMode.Date || Mode == TextBoxMode.Time)
                {
                    if (selectionLength == 0)
                    {
                        if (selectionStart > 0)
                        {
                            selectionStart--;
                            txt = txt.Substring(0, selectionStart) + '_' + txt.Substring(selectionStart + 1, txt.Length - selectionStart - 1);
                        }
                    }
                    else
                        txt = txt.Substring(0, selectionStart) + String.Empty.PadRight(selectionLength, '_') + txt.Substring(selectionStart + selectionLength, txt.Length - selectionStart - selectionLength);
                }

                // Les autres cas
                else
                {
                    if (selectionLength == 0)
                        if (selectionStart > 0)
                        {
                            selectionStart--;
                            if ((Mode != TextBoxMode.Double && Mode != TextBoxMode.Number) || txt[selectionStart] != ',') // Ne supprime pas la virgule pour les doubles
                                selectionLength = 1;
                        }

                    txt = txt.Substring(0, selectionStart) + txt.Substring(selectionStart + selectionLength, txt.Length - selectionStart - selectionLength);
                }
            }
            else if (letter == 'S')
            {
                // Remplace par des '_'
                if (Mode == TextBoxMode.Date || Mode == TextBoxMode.Time)
                {
                    if (selectionLength == 0)
                    {
                        if (selectionStart < txt.Length)
                        {
                            txt = txt.Substring(0, selectionStart) + '_' + txt.Substring(selectionStart + 1, txt.Length - selectionStart - 1);
                            selectionStart++;
                        }
                    }
                    else
                        txt = txt.Substring(0, selectionStart) + String.Empty.PadRight(selectionLength, '_') + txt.Substring(selectionStart + selectionLength, txt.Length - selectionStart - selectionLength);
                }

                // Les autres cas
                else
                {
                    if (selectionLength == 0)
                        if (selectionStart < txt.Length)
                        {
                            if ((Mode == TextBoxMode.Double || Mode == TextBoxMode.Number) && txt[selectionStart] == ',') // Ne supprime pas la virgule pour les doubles
                                selectionStart++;
                            else
                                selectionLength = 1;
                        }

                    txt = txt.Substring(0, selectionStart) + txt.Substring(selectionStart + selectionLength, txt.Length - selectionStart - selectionLength);
                }
            }
            else if (letter == Separator)
            {
                // Ignore la virgule
                if (Mode == TextBoxMode.Digit || Mode == TextBoxMode.Integer || Mode == TextBoxMode.Date || Mode == TextBoxMode.Time)
                    return;

                // Ajoute la virgule
                txt = txt.Substring(0, selectionStart) + Separator + txt.Substring(selectionStart + selectionLength, txt.Length - selectionStart - selectionLength);
                selectionStart++;
            }
            else
            {
                var insert = letter.ToString();
                // Remplace les caractères tapés
                if (Mode == TextBoxMode.Date || Mode == TextBoxMode.Time)
                    if (selectionStart < txt.Length)
                    {
                        if (selectionLength == 0)
                            selectionLength = 1;
                        else
                            insert = insert.PadRight(selectionLength, '_');
                    }

                // Insert les caractères
                txt = txt.Substring(0, selectionStart) + insert + txt.Substring(selectionStart + selectionLength, txt.Length - selectionStart - selectionLength);
                selectionStart++;
            }

            // Supprime les éventuelles autres virgules
            txt = Cleanup(txt, Mode, ref selectionStart, ref selectionLength);


            //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            // Reformate la chaine résultante

            var doubleValue = 0.0;
            var dateTimeValue = DateTime.MinValue;
            var valid = true;
            Text = Format(Mode, Decimals, DisplayZeros, txt, ref selectionStart, ref doubleValue, ref dateTimeValue, ref valid);
            SelectionStart = selectionStart;
            SelectionLength = 0;

            _preventChange = true;

            if (Mode == TextBoxMode.Date || Mode == TextBoxMode.Time)
            {
                if (Date != dateTimeValue)
                {
                    SetValue(DateProperty, dateTimeValue);
                    RaiseEvent(new RoutedEventArgs(DateChangeEvent, this));
                }
            }
            else
            {
                if (Double != doubleValue)
                {
                    SetValue(DoubleProperty, doubleValue);
                    RaiseEvent(new RoutedEventArgs(DoubleChangeEvent, this));
                }
            }

            if (IsValid != valid)
            {
                SetValue(IsValidProperty, valid);
                RaiseEvent(new RoutedEventArgs(IsValidChangedEvent, this));
            }

            _preventChange = false;
        }


        /********************************************************************************************************************************************************************************************************************************************************************************
        * 
        * Cleanup original string
        * 
        ***********************************************************************************************************************************************************************************************************************************************************************************/

        static string Cleanup(string text, TextBoxMode mode, ref int selectionStart, ref int selectionLength)
        {
            var result = "";
            var resultStart = selectionStart;
            var resultLength = selectionLength;

            // Replace l'eventuelle point par une virgule
            text = text.Replace(".", ",");

            // remove all numbers and first coma
            var chars = text.ToCharArray();
            var textPosition = 0;
            var comaPosition = -1;
            foreach (var c in chars)
            {
                if ((c >= '0' && c <= '9') || c == '_')
                    result += c;
                else if ((mode == TextBoxMode.Double || mode == TextBoxMode.Number) && c == ',' && comaPosition < 0)
                {
                    result += c;
                    comaPosition = textPosition;
                }
                else
                {
                    if (textPosition < selectionStart)
                        resultStart--;
                    else if (textPosition - selectionStart < selectionLength)
                        resultLength--;
                }

                textPosition++;
            }

            selectionStart = resultStart;
            selectionLength = resultLength;
            return result;
        }

        static readonly char Separator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator[0];

        /********************************************************************************************************************************************************************************************************************************************************************************
        * 
        * Formate la chaine de caractère intermédiaire pour l'affichage final et détermination des valeurs Double et DateTime
        * 
        ***********************************************************************************************************************************************************************************************************************************************************************************/

        static string Format(TextBoxMode mode, int decimals, DisplayZeros displayZeros, string text, ref int cursorPos, ref double doubleValue, ref DateTime dateTimeValue, ref bool valid)
        {
            var result = "";
            valid = true;

            //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            // Chiffres

            if (mode == TextBoxMode.Digit)
            {
                result = text;
                double.TryParse(result, out doubleValue);
            }


            //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            // Entier

            else if (mode == TextBoxMode.Integer)
            {
                var temp = text.TrimStart(new char[] { '0' });
                cursorPos -= text.Length - temp.Length;
                double.TryParse(temp, out doubleValue);

                if (cursorPos < 0)
                    cursorPos = 0;

                // Partie entière
                var digit = 0;
                var i = temp.Length - 1;
                while (i >= 0)
                {
                    if (digit != 0 && digit % 3 == 0)
                    {
                        result = " " + result;
                        if (cursorPos > i)
                            cursorPos++;
                    }
                    result = temp[i] + result;
                    i--;
                    digit++;
                }

                if (result != "") return result;

                if (displayZeros == DisplayZeros.Always)
                {
                    result = "0";
                    cursorPos = 1;
                }
                else if (displayZeros == DisplayZeros.Free)
                {
                    if (text != "")
                        cursorPos = 1;
                    else
                        cursorPos = 0;
                }
            }


            //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            // Double

            else if (mode == TextBoxMode.Double)
            {
                doubleValue = DoubleFormat(text, decimals, ref cursorPos, ref result);
            }


            //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            // Nombre

            else if (mode == TextBoxMode.Number)
            {
                doubleValue = DoubleFormat(text, decimals, ref cursorPos, ref result);

                if (doubleValue == Math.Truncate(doubleValue))
                {
                    int positionVirgule = result.IndexOf(',');
                    if (cursorPos <= positionVirgule)
                        result = result.Substring(0, positionVirgule);
                }

            }


            //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            // Date

            else if (mode == TextBoxMode.Date)
            {
                // Prépare le découpage de la date            
                text = text.PadRight(8, '_');

                // Jour
                var day = text.Substring(0, 2);

                if (day[0] != '_' && day[1] == '_')
                {
                    if (day[0] > '3')
                    {
                        day = "0" + day[0];
                        if (cursorPos == 1)
                            cursorPos = 2;
                    }
                    else
                        day = day[0] + "0";
                }

                day = day.Replace('_', '0');
                int iDay = int.Parse(day);

                if (iDay > 31)
                {
                    iDay = 31;
                    day = "31";
                }

                if (iDay < 1)
                {
                    day = "__";
                    valid = false;
                }


                // Mois
                var month = text.Substring(2, 2);

                if (month[0] != '_' && month[1] == '_')
                {
                    if (month[0] > '1')
                    {
                        month = "0" + month[0];
                        if (cursorPos == 3)
                            cursorPos = 4;
                    }
                    else
                        month = month[0] + "0";
                }

                month = month.Replace('_', '0');
                int iMois = int.Parse(month);

                if (iMois > 12)
                {
                    iMois = 12;
                    month = "12";
                }

                if (iMois < 1)
                {
                    month = "__";
                    valid = false;
                }


                //Année
                var year = text.Substring(4, 4);

                // Que le premier chiffre
                if (year[0] != '_' && year[1] == '_' && year[2] == '_' && year[3] == '_')
                {
                    if (year[0] == '0') // Années >= 2000
                    {
                        year = "200_";
                        if (cursorPos == 5)
                            cursorPos = 7;
                    }
                }

                // Les deux premiers chiffres entrés
                if (year[0] != '_' && year[1] != '_' && year[2] == '_' && year[3] == '_')
                {
                    // Cas ou la date ne sera qu'à partir de 2010
                    if (year[0] == '1')
                    {
                        if (year[1] < '9')
                        {
                            year = "20" + year[0] + year[1];
                            if (cursorPos == 6)
                                cursorPos = 8;
                        }
                    }

                    else if (year[0] > '2')
                    {
                        year = "19" + year[0] + year[1];
                        if (cursorPos == 6)
                            cursorPos = 8;
                    }
                }

                int iAnnee = int.Parse(year.Replace('_', '0'));

                if (iAnnee > 2050)
                    valid = false;

                if (iAnnee < 1900)
                    valid = false;

                if (iAnnee < 1)
                    year = "____";


                // Validation de la date
                result = day + " / " + month + " / " + year;

                if (result.Contains("_"))
                    valid = false;

                dateTimeValue = DateTime.MinValue;
                doubleValue = 0.0;
                if (valid)
                {
                    try
                    {
                        dateTimeValue = new DateTime(iAnnee, iMois, iDay);
                        doubleValue = dateTimeValue.Year * 10000.0 + dateTimeValue.Month * 100.0 + dateTimeValue.Day;
                    }
                    catch (Exception)
                    {
                        valid = false;
                    }
                }

                //if(valeurDateTime.Day != iJour || valeurDateTime.Month != iMois)
                //   valide = false;

                if (result == "__ / __ / ____")
                    valid = true;

                // Modifie la position du curseur
                if (cursorPos >= 4)
                    cursorPos += 6;
                else if (cursorPos >= 2)
                    cursorPos += 3;
            }


            //------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            // Heure

            else if (mode == TextBoxMode.Time)
            {
                // Prépare le découpage de la date            
                text = text.PadRight(6, '_');

                // Heure
                String heure = text.Substring(0, 2);
                int iHeure = int.Parse(heure.Replace('_', '0'));


                // Minutes
                String minute = text.Substring(2, 2);

                if (minute[0] != '_' && minute[1] == '_')
                {
                    if (minute[0] > '5')
                        minute = "5" + minute[1];
                }

                int iMinute = int.Parse(minute.Replace('_', '0'));

                if (iMinute > 59)
                {
                    iMinute = 59;
                    minute = "59";
                }

                // Secondes
                String seconde = text.Substring(4, 2);

                if (seconde[0] != '_' && seconde[1] == '_')
                {
                    if (seconde[0] > '5')
                        seconde = "5" + seconde[1];
                }

                int iSeconde = int.Parse(seconde.Replace('_', '0'));

                if (iSeconde > 59)
                {
                    iSeconde = 59;
                    seconde = "59";
                }

                // Validation de la date
                dateTimeValue = DateTime.MinValue;
                doubleValue = 0.0;
                if (valid)
                {
                    try
                    {
                        dateTimeValue = new DateTime(1, 1, 1, iHeure, iMinute, iSeconde);
                        doubleValue = dateTimeValue.Hour * 10000.0 + dateTimeValue.Minute * 100.0 + dateTimeValue.Second;
                    }
                    catch (Exception)
                    {
                        valid = false;
                    }
                }

                //if(valeurDateTime.Day != iJour || valeurDateTime.Month != iMois)
                //   valide = false;

                result = heure + " : " + minute + " : " + seconde;
                if (result == "__ : __ : __")
                    valid = true;

                // Modifie la position du curseur
                if (cursorPos >= 4)
                    cursorPos += 6;
                else if (cursorPos >= 2)
                    cursorPos += 3;
            }

            return result;
        }


        /********************************************************************************************************************************************************************************************************************************************************************************
        * 
        * Formate la chaine de caractère intermédiaire en Double
        * 
        ***********************************************************************************************************************************************************************************************************************************************************************************/

        static double DoubleFormat(string text, int decimals, ref int cursorPos, ref string result)
        {
            var temp = text.TrimStart(new char[] { '0' });
            var tempLength = temp.Length;
            cursorPos -= text.Length - tempLength;

            if (cursorPos < 0) cursorPos = 0;

            var separatorPos = temp.IndexOf(Separator);

            if (separatorPos == -1)
                separatorPos = tempLength;

            // Integer part
            var digit = 0;
            var i = separatorPos - 1;
            while (i >= 0)
            {
                if (digit != 0 && digit % 3 == 0)
                {
                    result = " " + result;
                    if (cursorPos > i)
                        cursorPos++;
                }
                result = temp[i] + result;
                i--;
                digit++;
            }

            // Decimal part
            digit = 0;
            i = separatorPos;
            while (i < tempLength && digit <= decimals)
            {
                result += temp[i];
                i++;
                digit++;
            }

            if (cursorPos > result.Length)
                cursorPos = result.Length;

            // Filling after separator
            separatorPos = result.IndexOf(Separator);
            if (separatorPos == -1)
            {
                result += Separator;
                separatorPos = result.Length - 1;
            }
            result = result.PadRight(separatorPos + 1 + decimals, '0');

            // if no zero in front
            if (result[0] == Separator)
            {
                result = "0" + result;
                cursorPos++;
            }

            // Valeur double
            if(double.TryParse(result.Replace(" ", ""), out var value))
            {
                return value;
            }
            return double.NaN;
        }


        /********************************************************************************************************************************************************************************************************************************************************************************
        * 
        * Vérifie si l'adresse email est valide
        * 
        ***********************************************************************************************************************************************************************************************************************************************************************************/

        /* private static bool ValideEmail(String email)
         {
            return IsValidEmail(email);

            if(email.Length <= 0)
               return true;

            String[] parties = email.Split(new char[] { '@' });

            if(parties.GetLength(0) != 2)
               return false;

            if(parties[0].Length < 1)
               return false;

            if(parties[1].Length < 3)
               return false;

            String[] partiesDomaine = parties[1].Split(new char[] { '.' });

            if(partiesDomaine.GetLength(0) < 2)
               return false;

            if(partiesDomaine[0].Length < 1)
               return false;

            if(partiesDomaine[partiesDomaine.GetLength(0) - 1].Length < 1)
               return false;

            // Vérifie qu'il n'y a que des caractères autorisés
            // Partie utilisateur
            foreach(char c in parties[0])
            {
               if( c 

               }
           // Partie domaine

            return true;
         }*/

        //static bool invalid = false;
        /*
        private static bool ValideEmail(String email)
        {
           //return Regex.IsMatch(email, @"(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*|""(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21\x23-\x5b\x5d-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])*"")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\[(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?|[a-z0-9-]*[a-z0-9]:(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21-\x5a\x53-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])+)\])");
           /*
           return Regex.IsMatch(email, @"^(?'localPart'((((\((((?'paren'\()|(?'-paren'\))|([\u0021-\u0027\u002a-\u005b\u005d-\u007e]|[\u0001-\u0008\u000b\u000c\u000e-\u001f\u007f])|([ \t]+((\r\n)[ \t]+)?|((\r\n)[ \t]+)+)|\\([\u0021-\u007e]|[ \t]|[\r\n\0]|[\u0001-\u0008\u000b\u000c"
  +@"\u000e-\u001f\u007f]))*(?(paren)(?!)))\))|([ \t]+((\r\n)[ \t]+)?|((\r\n)[ \t]+)+))*?(([a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+)|(""(([ \t]+((\r\n)[ \t]+)?|((\r\n)[ \t]+)+)?(([\u0021\u0023-\u005b\u005d-\u007e]|[\u0001-\u0008\u000b\u000c\u000e-\u001f\u"
  +@"007f])|\\([\u0021-\u007e]|[ \t]|[\r\n\0]|[\u0001-\u0008\u000b\u000c\u000e-\u001f\u007f])))*([ \t]+((\r\n)[ \t]+)?|((\r\n)[ \t]+)+)?""))((\((((?'paren'\()|(?'-paren'\))|([\u0021-\u0027\u002a-\u005b\u005d-\u007e]|[\u0001-\u0008\u000b\u000c\u00"
  +@"0e-\u001f\u007f])|([ \t]+((\r\n)[ \t]+)?|((\r\n)[ \t]+)+)|\\([\u0021-\u007e]|[ \t]|[\r\n\0]|[\u0001-\u0008\u000b\u000c\u000e-\u001f\u007f]))*(?(paren)(?!)))\))|([ \t]+((\r\n)[ \t]+)?|((\r\n)[ \t]+)+))*?)(\.(((\((((?'paren'\()|(?'-paren'\))|"
  +@"([\u0021-\u0027\u002a-\u005b\u005d-\u007e]|[\u0001-\u0008\u000b\u000c\u000e-\u001f\u007f])|([ \t]+((\r\n)[ \t]+)?|((\r\n)[ \t]+)+)|\\([\u0021-\u007e]|[ \t]|[\r\n\0]|[\u0001-\u0008\u000b\u000c\u000e-\u001f\u007f]))*(?(paren)(?!)))\))|([ \t]+"
  +@"((\r\n)[ \t]+)?|((\r\n)[ \t]+)+))*?(([a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+)|(""(([ \t]+((\r\n)[ \t]+)?|((\r\n)[ \t]+)+)?(([\u0021\u0023-\u005b\u005d-\u007e]|[\u0001-\u0008\u000b\u000c\u000e-\u001f\u007f])|\\([\u0021-\u007e]|[ \t]|[\r\n\0]|[\u0001-"
  +@"\u0008\u000b\u000c\u000e-\u001f\u007f])))*([ \t]+((\r\n)[ \t]+)?|((\r\n)[ \t]+)+)?""))((\((((?'paren'\()|(?'-paren'\))|([\u0021-\u0027\u002a-\u005b\u005d-\u007e]|[\u0001-\u0008\u000b\u000c\u000e-\u001f\u007f])|([ \t]+((\r\n)[ \t]+)?|((\r\n)["
  +@"\t]+)+)|\\([\u0021-\u007e]|[ \t]|[\r\n\0]|[\u0001-\u0008\u000b\u000c\u000e-\u001f\u007f]))*(?(paren)(?!)))\))|([ \t]+((\r\n)[ \t]+)?|((\r\n)[ \t]+)+))*?))*))@(?'domain'((((\((((?'paren'\()|(?'-paren'\))|([\u0021-\u0027\u002a-\u005b\u005d-\"
  +@"u007e]|[\u0001-\u0008\u000b\u000c\u000e-\u001f\u007f])|([ \t]+((\r\n)[ \t]+)?|((\r\n)[ \t]+)+)|\\([\u0021-\u007e]|[ \t]|[\r\n\0]|[\u0001-\u0008\u000b\u000c\u000e-\u001f\u007f]))*(?(paren)(?!)))\))|([ \t]+((\r\n)[ \t]+)?|((\r\n)[ \t]+)+))*?("
  +@"([a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+)|(""(([ \t]+((\r\n)[ \t]+)?|((\r\n)[ \t]+)+)?(([\u0021\u0023-\u005b\u005d-\u007e]|[\u0001-\u0008\u000b\u000c\u000e-\u001f\u007f])|\\([\u0021-\u007e]|[ \t]|[\r\n\0]|[\u0001-\u0008\u000b\u000c\u000e-\u001f\u007"
  +@"f])))*([ \t]+((\r\n)[ \t]+)?|((\r\n)[ \t]+)+)?""))((\((((?'paren'\()|(?'-paren'\))|([\u0021-\u0027\u002a-\u005b\u005d-\u007e]|[\u0001-\u0008\u000b\u000c\u000e-\u001f\u007f])|([ \t]+((\r\n)[ \t]+)?|((\r\n)[ \t]+)+)|\\([\u0021-\u007e]|[ \t]|[\"
  +@"r\n\0]|[\u0001-\u0008\u000b\u000c\u000e-\u001f\u007f]))*(?(paren)(?!)))\))|([ \t]+((\r\n)[ \t]+)?|((\r\n)[ \t]+)+))*?)(\.(((\((((?'paren'\()|(?'-paren'\))|([\u0021-\u0027\u002a-\u005b\u005d-\u007e]|[\u0001-\u0008\u000b\u000c\u000e-\u001f\u0"
  +@"07f])|([ \t]+((\r\n)[ \t]+)?|((\r\n)[ \t]+)+)|\\([\u0021-\u007e]|[ \t]|[\r\n\0]|[\u0001-\u0008\u000b\u000c\u000e-\u001f\u007f]))*(?(paren)(?!)))\))|([ \t]+((\r\n)[ \t]+)?|((\r\n)[\t]+)+))*?(([a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+)|(""(([ \t]+((\r\"
  +@"n)[ \t]+)?|((\r\n)[ \t]+)+)?(([\u0021\u0023-\u005b\u005d-\u007e]|[\u0001-\u0008\u000b\u000c\u000e-\u001f\u007f])|\\([\u0021-\u007e]|[ \t]|[\r\n\0]|[\u0001-\u0008\u000b\u000c\u000e-\u001f\u007f])))*([ \t]+((\r\n)[ \t]+)?|((\r\n)[ \t]+)+)?""))"
  +@"((\((((?'paren'\()|(?'-paren'\))|([\u0021-\u0027\u002a-\u005b\u005d-\u007e]|[\u0001-\u0008\u000b\u000c\u000e-\u001f\u007f])|([ \t]+((\r\n)[ \t]+)?|((\r\n)[ \t]+)+)|\\([\u0021-\u007e]|[ \t]|[\r\n\0]|[\u0001-\u0008\u000b\u000c\u000e-\u001f\u0"
  +@"07f]))*(?(paren)(?!)))\))|([ \t]+((\r\n)[ \t]+)?|((\r\n)[ \t]+)+))*?))*)|(((\((((?'paren'\()|(?'-paren'\))|([\u0021-\u0027\u002a-\u005b\u005d-\u007e]|[\u0001-\u0008\u000b\u000c\u000e-\u001f\u007f])|([ \t]+((\r\n)[ \t]+)?|((\r\n)[ \t]+)+)|\\"
  +@"([\u0021-\u007e]|[ \t]|[\r\n\0]|[\u0001-\u0008\u000b\u000c\u000e-\u001f\u007f]))*(?(paren)(?!)))\))|([ \t]+((\r\n)[ \t]+)?|((\r\n)[ \t]+)+))*?\[(([ \t]+((\r\n)[ \t]+)?|((\r\n)[ \t]+)+)?([!-Z^-~]|[\u0001-\u0008\u000b\u000c\u000e-\u001f\u007f"
  +@"]))*([ \t]+((\r\n)[ \t]+)?|((\r\n)[ \t]+)+)?\]((\((((?'paren'\()|(?'-paren'\))|([\u0021-\u0027\u002a-\u005b\u005d-\u007e]|[\u0001-\u0008\u000b\u000c\u000e-\u001f\u007f])|([ \t]+((\r\n)[ \t]+)?|((\r\n)[ \t]+)+)|\\([\u0021-\u007e]|[ \t]|[\r\n"
  +@"\0]|[\u0001-\u0008\u000b\u000c\u000e-\u001f\u007f]))*(?(paren)(?!)))\))|([ \t]+((\r\n)[ \t]+)?|((\r\n)[ \t]+)+))*?))$", RegexOptions.IgnoreCase);
        //return Regex.IsMatch(email, "(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*|\"(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21\x23-\x5b\x5d-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])*\")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\\[(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?|[a-z0-9-]*[a-z0-9]:(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21-\x5a\x53-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])+)\\])");
           */
        //return Regex.IsMatch(email, "(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*|\"(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21\x23-\x5b\x5d-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])*\")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\\[(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?|[a-z0-9-]*[a-z0-9]:(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21-\x5a\x53-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])+)\\])", RegexOptions.IgnoreCase);


        public static bool ValidateEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
                return true;

            // Use IdnMapping class to convert Unicode domain names.
            email = Regex.Replace(email, @"(@)(.+)$", DomainMapper);
            return email != "" && Regex.IsMatch(email, @"^(?("")(""[^""]+?""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9]{2,17}))$", RegexOptions.IgnoreCase);
        }

        static string DomainMapper(Match match)
        {
            // IdnMapping class with default property values.
            IdnMapping idn = new();

            var domainName = match.Groups[2].Value;
            try
            {
                domainName = idn.GetAscii(domainName);
            }
            catch (ArgumentException)
            {
                return "";
            }
            return match.Groups[1].Value + domainName;
        }

        public void Evaluate(string formula)
        {
            try
            {
                var engine = new Mages.Core.Engine();
                var result = engine.Interpret(formula);

                if (result != null)
                    Compute((double)result);
            }
            catch { }

            //Double = double.NaN;
            //IsValid = false;
        }

        public void Compute(double value)
        {
            if (double.IsInfinity(value) || double.IsNaN(value))
            {
//                Background = InvalidBrush;
                Text = "!";
                Double = double.NaN;
                IsValid = false;
            }

//            Background = ValidBrush;
            Double = Math.Round(value, Decimals);
            IsValid = true;
        }

    }
}
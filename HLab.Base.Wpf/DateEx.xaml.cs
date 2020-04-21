using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace HLab.Base
{
    using H = DependencyHelper<DateEx>;
    /// <summary>
    /// Logique d'interaction pour DateEx.xaml
    /// </summary>
    public interface IMandatoryNotFilled
    {
        DependencyProperty MandatoryProperty { get; }
        bool MandatoryNotFilled { get; set; }
    }

    public partial class DateEx : UserControl, IMandatoryNotFilled
    {
        private static readonly int[] _daysToMonth365 = {
            0, 31, 59, 90, 120, 151, 181, 212, 243, 273, 304, 334, 365};
        private static readonly int[] _daysToMonth366 = {
            0, 31, 60, 91, 121, 152, 182, 213, 244, 274, 305, 335, 366};
        private static int DaysInMonth(int year, int month)
        {
            if (month < 1) return _daysToMonth366.Max();
            int[] days = year<1 ? _daysToMonth366 : DateTime.IsLeapYear(year) ? _daysToMonth366 : _daysToMonth365;
            return days[month] - days[month - 1];
        }

        public DateEx()
        {
            InitializeComponent();
            Set(Date,DayValid);
        }
        public DependencyProperty MandatoryProperty => DateProperty;

        public static readonly DependencyProperty DateProperty =
            H.Property<DateTime?>()
            .BindsTwoWayByDefault
            .OnChange((e, a) =>
            {
                e.Set(a.NewValue, e.DayValid);
                e.Calendar.DisplayDate = a.NewValue??DateTime.Now;
            }).Register();

        public static readonly DependencyProperty DayValidProperty =
            H.Property<bool>()
            .BindsTwoWayByDefault
            .OnChange((e, a) =>
            {
                e.Set(e.Date, a.NewValue);
            }).Register();

        public static readonly DependencyProperty IsReadOnlyProperty =
            H.Property<bool>().Default(false).OnChange((e, a) =>
            {
                e.SetReadOnly();
            }).Register();

        public static readonly DependencyProperty EmptyDayAllowedProperty =
            H.Property<bool>().OnChange((e, a) =>
            {
                //e.Set(e.Date, a.NewValue);
            }).Register();

        public static readonly DependencyProperty MandatoryNotFilledProperty = H.Property<bool>()
            .OnChange( (s,a) => s.SetMandatoryNotFilled(a.NewValue) )
            .Register();

        public static readonly DependencyProperty ContentBackgroundProperty = H.Property<Brush>()
            .OnChange( (s,a) => s.SetContentBackground(a.NewValue) )
            .Default(new SolidColorBrush(Colors.Transparent))
            .Register();

        private void SetContentBackground(Brush brush)
        {
            TextDay.Background = brush;
            TextMonth.Background = brush;
            TextYear.Background = brush;
        }

        private void SetReadOnly()
        {
            TextDay.IsReadOnly = IsReadOnly;
            TextMonth.IsReadOnly = IsReadOnly;
            TextYear.IsReadOnly = IsReadOnly;
            Button.Visibility = IsReadOnly ? Visibility.Collapsed : Visibility.Visible;
        }

        private void Set(DateTime? date, bool dayValid)
        {
            if (date != null)
            {
                TextDay.Value = dayValid?date.Value.Day:0;
                TextMonth.Value = date.Value.Month;
                TextYear.Value = date.Value.Year;
            }
        }
        private void SetMandatoryNotFilled(bool mnf)
        {
            Mandatory.Visibility = mnf ? Visibility.Visible : Visibility.Collapsed;
            IconMandatory.Visibility = mnf ? Visibility.Visible : Visibility.Collapsed;
        }

        public DateTime? Date
        {
            get => (DateTime?)GetValue(DateProperty);
            set => SetValue(DateProperty, value);
        }
        public bool DayValid
        {
            get => (bool)GetValue(DayValidProperty);
            set => SetValue(DayValidProperty, value);
        }
        public bool EmptyDayAllowed
        {
            get => (bool)GetValue(EmptyDayAllowedProperty);
            set => SetValue(EmptyDayAllowedProperty, value);
        }
        public bool IsReadOnly
        {
            get => (bool)GetValue(IsReadOnlyProperty);
            set => SetValue(IsReadOnlyProperty, value);
        }
        public bool MandatoryNotFilled
        {
            get => (bool)GetValue(MandatoryNotFilledProperty);
            set => SetValue(MandatoryNotFilledProperty, value);
        }
        public Brush ContentBackground
        {
            get => (Brush)GetValue(ContentBackgroundProperty);
            set => SetValue(ContentBackgroundProperty, value);
        }

        protected virtual void OnValueChange(object source, ValueChangedEventArg e)
        {
            var year = TextYear.Value;
            var month = TextMonth.Value;
            var day = TextDay.Value;
            var dayValid = true;

            if (month > 12) month = 12;
            if (month < 1 && year>0) month = 1;
            //if (year < 1) year = 1;
            if (year > 9999) year = 9999;

            if (EmptyDayAllowed)
            {
                if (day < 1 || day > DaysInMonth(year, month))
                {
                    dayValid = false;
                    day = DaysInMonth(year, month);
                }
            }
            else
            {
                if (day < 1 && month>0) day = 1;
                if (day > DaysInMonth(year, month))
                {
                    day = DaysInMonth(year, month);
                }
            }


            TextYear.Value = year;
            TextMonth.Value = month;

            TextDay.Value = dayValid ? day : 0;
            if (year > 0)
            {
                DayValid = dayValid;
                Date = new DateTime(year,month,day);
            }
        }

        private void Button_OnClick(object sender, RoutedEventArgs e)
        {
            Popup.IsOpen = true;
            Calendar.Focus();
        }

        private void Calendar_OnSelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            DayValid = true;
            Date = Calendar.SelectedDate;
            Popup.IsOpen = false;
        }
    }
}

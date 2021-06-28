using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace HLab.Base.Wpf
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
            int[] days = year < 1 ? _daysToMonth366 : DateTime.IsLeapYear(year) ? _daysToMonth366 : _daysToMonth365;
            return days[month] - days[month - 1];
        }

        public DateEx()
        {
            InitializeComponent();
            Set(Date, DayValid);
        }
        public DependencyProperty MandatoryProperty => DateProperty;

        public static readonly DependencyProperty DateProperty =
            H.Property<DateTime?>()
            .BindsTwoWayByDefault
            .OnChange((e, a) =>
            {
                e.Set(a.NewValue, e.DayValid);
                e.Calendar.DisplayDate = a.NewValue ?? DateTime.Now;
            }).Register();

        public static readonly DependencyProperty DayValidProperty =
            H.Property<bool>()
            .BindsTwoWayByDefault
            .OnChange((e, a) =>
            {
                e.Set(e.Date, a.NewValue);
            }).Register();

        public static readonly DependencyProperty HasTimeProperty =
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

        public static readonly DependencyProperty ShowTimeProperty =
            H.Property<bool>().OnChange((e, a) =>
            {
                e.SetShowTime();
            }).Default(false).Register();

        public static readonly DependencyProperty MandatoryNotFilledProperty = H.Property<bool>()
            .OnChange((s, a) => s.SetMandatoryNotFilled(a.NewValue))
            .Register();

        public static readonly DependencyProperty ContentBackgroundProperty = H.Property<Brush>()
            .OnChange((s, a) => s.SetContentBackground(a.NewValue))
            .Default(new SolidColorBrush(Colors.Transparent))
            .Register();

        private void SetContentBackground(Brush brush)
        {
            TextDay.Background = brush;
            TextMonth.Background = brush;
            TextYear.Background = brush;
            TextHour.Background = brush;
            TextMin.Background = brush;
        }

        private void SetReadOnly()
        {
            TextDay.IsReadOnly = IsReadOnly;
            TextMonth.IsReadOnly = IsReadOnly;
            TextYear.IsReadOnly = IsReadOnly;
            TextHour.IsReadOnly = IsReadOnly;
            TextMin.IsReadOnly = IsReadOnly;
            Button.Visibility = IsReadOnly ? Visibility.Collapsed : Visibility.Visible;
            if (ShowTime)
            {
                Spacer.Visibility = IsReadOnly ? Visibility.Visible : Visibility.Collapsed;
            }
            SetTimeButton();
        }

        private void Set(DateTime? date, bool dayValid)
        {
            if (date == null)
            {
                TextDay.Text = "";
                TextMonth.Text = "";
                TextYear.Text = "";

                TextHour.Value = 0;
                TextMin.Value = 0;
            }
            else
            {
                TextDay.Value = dayValid ? date.Value.Day : 0;
                TextMonth.Value = date.Value.Month;
                TextYear.Value = date.Value.Year;

                TextHour.Value = date.Value.Hour;
                TextMin.Value = date.Value.Minute;

                TimePicker.Time = date;
            }
        }

        private void SetMandatoryNotFilled(bool mnf)
        {
            Mandatory.Visibility = mnf ? Visibility.Visible : Visibility.Collapsed;
            IconMandatory.Visibility = mnf ? Visibility.Visible : Visibility.Collapsed;
        }


        private void SetShowTime()
        {
            SetTimeButton();
            if (ShowTime)
            {
                SetShowTime(Visibility.Visible,new GridLength(1, GridUnitType.Star));
            }
            else
            {
                SetShowTime(Visibility.Collapsed,GridLength.Auto);
            }
        }


        private void SetShowTime(Visibility visibility, GridLength width)
        {
            TextHour.Visibility = visibility;
            TextMin.Visibility = visibility;
            TimeDots.Visibility = visibility;
            TimeDots.Visibility = visibility;
            HourColumn.Width = width;
            MinColumn.Width = width;
        }

        private void SetTimeButton()
        {
            if (ShowTime && !IsReadOnly)
            {
                TimeButton.Visibility = Visibility.Visible;
            }
            else
            {
                TimeButton.Visibility = Visibility.Collapsed;
            }
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
        public bool ShowTime
        {
            get => (bool)GetValue(ShowTimeProperty);
            set => SetValue(ShowTimeProperty, value);
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
            var hour = TextHour.Value;
            var minute = TextMin.Value;
            var dayValid = true;

            if (month > 12) month = 12;
            if (month < 1 && year > 0) month = 1;
            //if (year < 1) year = 1;
            if (year > 9999) year = 9999;

            if (hour > 24) hour = 24;
            if (minute > 59) minute = 59;
            if (hour < 0) hour = 0;
            if (minute < 0) minute = 0;

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
                if (day < 1)
                {
                    if (month > 0) day = 1;
                }
                else
                {
                    var daysInMonth = DaysInMonth(year, month);
                    if (day > daysInMonth)
                        day = daysInMonth;
                }
            }

            TextYear.Value = year;
            TextMonth.Value = month;
            TextDay.Value = dayValid ? day : 0;
            TextHour.Value = hour;
            TextMin.Value = minute;

            if (year > 0)
            {
                DayValid = dayValid;
                Date = new DateTime(year, month, day, hour,minute,0);
            }
        }

        private void Button_OnClick(object sender, RoutedEventArgs e)
        {
            Popup.IsOpen = true;
            Calendar.Focus();
        }
        private void TimeButton_OnClick(object sender, RoutedEventArgs e)
        {
            TimePopup.IsOpen = true;
            TimePicker.Focus();
        }

        private void NowButton_OnClick(object sender, RoutedEventArgs e)
        {
            DayValid = true;
            Date = DateTime.Now;
        }

        private void Calendar_OnSelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            DayValid = true;
            Date = Calendar.SelectedDate;
            Popup.IsOpen = false;
        }

        private void TimePicker_OnDone(object sender, RoutedEventArgs e)
        {
            TimePopup.IsOpen = false;
        }
    }
}

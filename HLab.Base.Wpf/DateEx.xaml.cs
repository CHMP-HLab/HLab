using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using HLab.Base.Extensions;

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
        static readonly int[] _daysToMonth365 = {
            0, 31, 59, 90, 120, 151, 181, 212, 243, 273, 304, 334, 365};

        static readonly int[] _daysToMonth366 = {
            0, 31, 60, 91, 121, 152, 182, 213, 244, 274, 305, 335, 366};

        static int DaysInMonth(int year, int month)
        {
            if (month < 1) return _daysToMonth366.Max();
            int[] days = year < 1 ? _daysToMonth366 : DateTime.IsLeapYear(year) ? _daysToMonth366 : _daysToMonth365;
            return days[month] - days[month - 1];
        }

        public DateEx()
        {
            InitializeComponent();
        }
        public DependencyProperty MandatoryProperty => DateProperty;

        public static readonly DependencyProperty DateProperty =
            H.Property<DateTime?>()
            .BindsTwoWayByDefault
            .OnChange((e, a) => e.SetDate(a.NewValue, !e.EmptyDayAllowed || e.DayValid)).Register();

        public static readonly DependencyProperty DateUtcProperty =
            H.Property<DateTime?>()
            .BindsTwoWayByDefault
            .OnChange((e, a) => e.SetDate(a.NewValue, !e.EmptyDayAllowed || e.DayValid)).Register();

        public static readonly DependencyProperty DayValidProperty =
            H.Property<bool>()
            .BindsTwoWayByDefault
            .OnChange((e, a) => e.SetDate(e.Date, a.NewValue)).Register();


        public static readonly DependencyProperty IsReadOnlyProperty =
            H.Property<bool>().Default(false).OnChange((e, a) => e.SetReadOnly(a.NewValue)).Register();

        public static readonly DependencyProperty EmptyDayAllowedProperty =
            H.Property<bool>().OnChange((e, a) =>
            {
                //e.Set(e.Date, a.NewValue);
            }).Register();

        public static readonly DependencyProperty ShowTimeProperty =
            H.Property<bool>().OnChange((e, a) => e.SetShowTime(a.NewValue)).Default(false).Register();

        public static readonly DependencyProperty MandatoryNotFilledProperty = H.Property<bool>()
            .OnChange((s, a) => s.SetMandatoryNotFilled(a.NewValue))
            .Register();

        public static readonly DependencyProperty ContentBackgroundProperty = H.Property<Brush>()
            .OnChange((s, a) => s.SetContentBackground(a.NewValue))
            .Default(new SolidColorBrush(Colors.Transparent))
            .Register();

        void SetContentBackground(Brush brush)
        {
            TextDay.Background = brush;
            TextMonth.Background = brush;
            TextYear.Background = brush;
            TextHour.Background = brush;
            TextMin.Background = brush;
        }

        void SetReadOnly(bool readOnly)
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
            SetTimeButton(readOnly);
        }


        bool _preventSetDate = false;

        void SetDate(DateTime? date, bool dayValid)
        {
            if(_preventSetDate) return;
            _preventSetDate = true;

            if (date.HasValue)
            {
                var d = date.Value.ToLocalTime();

                var daysInMonth = DaysInMonth(d.Year, d.Month);

                dayValid = dayValid || d.Day < daysInMonth;

                TextDay.Value = dayValid ? d.Day : 0;
                TextMonth.Value = d.Month;
                TextYear.Value = d.Year;

                TextHour.Value = d.Hour;
                TextMin.Value = d.Minute;

                Calendar.DisplayDate = d;
                TimePicker.Time = d;
                Date = d;
                DateUtc = d.ToUniversalTime();
            }
            else
            {
                TextDay.Text = "";
                TextMonth.Text = "";
                TextYear.Text = "";

                TextHour.Value = 0;
                TextMin.Value = 0;
                Date = null;
            }
            DayValid = dayValid;
            _preventSetDate = false;
        }


        void SetMandatoryNotFilled(bool mnf)
        {
            Mandatory.Visibility = mnf ? Visibility.Visible : Visibility.Collapsed;
            IconMandatory.Visibility = mnf ? Visibility.Visible : Visibility.Collapsed;
        }


        void SetShowTime(bool showTime)
        {
            SetTimeButton(showTime);
            if (showTime)
            {
                SetShowTime(Visibility.Visible, new GridLength(1, GridUnitType.Star));
            }
            else
            {
                SetShowTime(Visibility.Collapsed, GridLength.Auto);
            }
        }


        void SetShowTime(Visibility visibility, GridLength width)
        {
            TextHour.Visibility = visibility;
            TextMin.Visibility = visibility;
            TimeDots.Visibility = visibility;
            TimeDots.Visibility = visibility;
            HourColumn.Width = width;
            MinColumn.Width = width;
        }

        void SetTimeButton(bool showTime)
        {
            if (showTime && !IsReadOnly)
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
            get => ((DateTime?)GetValue(DateProperty)).ToLocalTime();
            set => SetValue(DateProperty, value.ToLocalTime());
        }

        public DateTime? DateUtc
        {
            get => ((DateTime?)GetValue(DateUtcProperty)).ToUniversalTime();
            set => SetValue(DateUtcProperty, value.ToUniversalTime());
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
            FromUi();
        }

        void FromUi()
        {
            var year = TextYear.Value;
            if (year < 1) return;
            if (year > 9999) year = 9999;

            var month = TextMonth.Value;
            if (month < 1) return;
            if (month > 12) month = 12;
            if (month < 1) month = 1;

            var day = TextDay.Value;
            var dayValid = true;
            if (EmptyDayAllowed)
            {
                if (day < 1)
                {
                    dayValid = false;
                    day = DaysInMonth(year, month);
                }
                else
                {
                    var daysInMonth = DaysInMonth(year, month);
                    if (day > daysInMonth)
                        day = daysInMonth;
                }
            }
            else
            {
                if (day < 1) return;
                else
                {
                    var daysInMonth = DaysInMonth(year, month);
                    if (day > daysInMonth)
                        day = daysInMonth;
                }
            }

            var hour = TextHour.Value;
            if (hour > 24) hour = 24;
            if (hour < 0) hour = 0;

            var minute = TextMin.Value;
            if (minute > 59) minute = 59;
            if (minute < 0) minute = 0;

            TextYear.Value = year;
            TextMonth.Value = month;
            TextDay.Value = dayValid ? day : 0;
            TextHour.Value = hour;
            TextMin.Value = minute;

            if (year > 0)
            {
                SetDate(new DateTime(year, month, day, hour, minute, 0),dayValid);
            }
        }

        void Button_OnClick(object sender, RoutedEventArgs e)
        {
            Popup.IsOpen = true;
            Calendar.Focus();
        }

        void TimeButton_OnClick(object sender, RoutedEventArgs e)
        {
            TimePopup.IsOpen = true;
            TimePicker.Focus();
        }

        void NowButton_OnClick(object sender, RoutedEventArgs e)
        {
            DayValid = true;
            Date = DateTime.Now;
        }

        void Calendar_OnSelectedDatesChanged(object sender, SelectionChangedEventArgs e)
        {
            DayValid = true;
            Date = Calendar.SelectedDate;
            Popup.IsOpen = false;
        }

        void TimePicker_OnDone(object sender, RoutedEventArgs e)
        {
            TimePopup.IsOpen = false;
        }
    }
}

using System;
using System.Windows;
using System.Windows.Controls;

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
                e.Set(e.Date, a.NewValue);
            }).Register();

        public static readonly DependencyProperty MandatoryNotFilledProperty = H.Property<bool>()
            .OnChange( (s,a) => s.SetMandatoryNotFilled(a.NewValue) )
            .Register();

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

        private void OnValueChange(object sender, EventArgs e)
        {
            var year = TextYear.Value;
            var month = TextMonth.Value;
            var day = TextDay.Value;
            bool dayValid = true;

            if (month > 12) month = 12;
            if (month < 1) month = 1;
            if (year < 1) year = 1;
            if (year > 9999) year = 9999;

            if (EmptyDayAllowed)
            {
                if (day < 1 || day > DateTime.DaysInMonth(year, month))
                {
                    dayValid = false;
                    day = DateTime.DaysInMonth(year, month);
                }
            }
            else
            {
                if (day < 1) day = 1;
                if (day > DateTime.DaysInMonth(year, month))
                {
                    day = DateTime.DaysInMonth(year, month);
                }
            }


            TextYear.Value = year;
            TextMonth.Value = month;

            TextDay.Value = dayValid ? day : 0;

            Date = new DateTime(year,month,day);
            DayValid = dayValid;
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

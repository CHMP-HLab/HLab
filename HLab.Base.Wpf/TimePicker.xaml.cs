using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace HLab.Base.Wpf
{
    using H = DependencyHelper<TimePicker>;

    /// <summary>
    /// Logique d'interaction pour TimePicker.xaml
    /// </summary>
    public partial class TimePicker : UserControl
    {
        public TimePicker()
        {
            InitializeComponent();
        }

        protected override void OnGotMouseCapture(MouseEventArgs e)
        {
            base.OnGotMouseCapture(e);
        }

        public static readonly RoutedEvent DoneEvent = H.Event().Bubble.Register();

        public event RoutedEventHandler Done
        {
            add => AddHandler(DoneEvent, value);
            remove => RemoveHandler(DoneEvent, value);
        }

        public static readonly DependencyProperty TimeProperty =
            H.Property<DateTime?>()
                .BindsTwoWayByDefault
                .OnChange((e, a) =>
                {
                    e.Set(a.NewValue);
                }).Register();

        void Set(DateTime? time)
        {
            if (time.HasValue)
            {
                HourListBox.SelectedItem = HourListBox.Items.OfType<ListBoxItem>().FirstOrDefault(i => int.Parse((string)i.Tag) == time.Value.Hour);
                MinuteListBox.SelectedItem = HourListBox.Items.OfType<ListBoxItem>().FirstOrDefault(i => int.Parse((string)i.Tag) == time.Value.Minute);;
            }
            else
            {
                HourListBox.SelectedItem = null;
                MinuteListBox.SelectedItem = null;
            }
        }

        public DateTime? Time
        {
            get => (DateTime?)GetValue(TimeProperty);
            set => SetValue(TimeProperty, value);
        }

        void HourListBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is not ListBox list) return;
            if (list.SelectedItem is not ListBoxItem item) return;
            if (item.Tag is not string value) return;

            if (!Time.HasValue) return;
            {

            }
            var time = Time ?? DateTime.Now;

            if (int.TryParse(value, out var hour))
            {
                Time = new DateTime(time.Year, time.Month, time.Day, hour, time.Minute, time.Second);
            }
        }

        void MinuteListBox_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is not ListBox list) return;
            if (list.SelectedItem is not ListBoxItem item) return;
            if (item.Tag is not string value) return;

            var time = Time ?? DateTime.Now;

            if (int.TryParse(value, out var minute))
            {
                Time = new DateTime(time.Year, time.Month, time.Day, time.Hour, minute, time.Second);
            }
            RaiseEvent(new RoutedEventArgs(DoneEvent,this));
        }

        void MinuteListBox_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(DoneEvent,this));
        }

        void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            Time = DateTime.Now;
            RaiseEvent(new RoutedEventArgs(DoneEvent,this));
        }
    }
}

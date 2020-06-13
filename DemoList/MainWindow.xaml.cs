using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DemoList
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {

        public TimeItemCollection Data = new TimeItemCollection();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = Data;

            Timer timer = new Timer((state) =>
            {
                Dispatcher.Invoke(() =>
                {
                    DateTime date = DateTime.Now;
                    Data.CurrentMonth = date.Month - 1;
                    Data.CurrentDay = date.Day - 1;
                    Data.CurrentHour = date.Hour;
                    Data.CurrentMin = date.Minute;
                    Data.CurrentSecond = date.Second;
                });
            });
            timer.Change(0, 100);
        }


        private void SecondItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.RemovedItems.Count > 0)
            {
                Data.CorrectSecond();
                SecondItems.BeginRotate(0, 6);
            }

        }

        private void MonthItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.RemovedItems.Count > 0)
            {
                Data.CorrectMonth();
                MonthItems.BeginRotate(0, 30);
            }
        }

        private void DayItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.RemovedItems.Count > 0)
            {
                Data.CorrectDay();
                DayItems.BeginRotate(0, 360.0 / 31.0);
            }
        }

        private void HourItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.RemovedItems.Count > 0)
            {
                Data.CorrectHour();
                HourItems.BeginRotate(0, 15);
            }
        }

        private void MinItems_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.RemovedItems.Count > 0)
            {
                Data.CorrectMin();
                MinItems.BeginRotate(0, 6);
            }
        }
    }

    public static class UIElementExtensions
    {
        public static void BeginRotate(this UIElement uIElement, double from, double to, int mili = 500)
        {
            RotateTransform rtf = new RotateTransform();
            uIElement.RenderTransform = rtf;
            uIElement.RenderTransform.BeginAnimation(RotateTransform.AngleProperty, new DoubleAnimation(from, to, new Duration(TimeSpan.FromMilliseconds(mili)))
            {
                BeginTime = new TimeSpan(0),
            });
        }
    }

    public class TimeItemCollection : BaseModel
    {
        public ObservableCollection<TimeItem> Months { get; set; } = new ObservableCollection<TimeItem>();
        public ObservableCollection<TimeItem> Days { get; set; } = new ObservableCollection<TimeItem>();
        public ObservableCollection<TimeItem> Hours { get; set; } = new ObservableCollection<TimeItem>();
        public ObservableCollection<TimeItem> Mins { get; set; } = new ObservableCollection<TimeItem>();
        public ObservableCollection<TimeItem> Seconds { get; set; } = new ObservableCollection<TimeItem>();


        private int _currentMonth;
        public int CurrentMonth { get { return _currentMonth; } set { Set(ref _currentMonth, value); } }

        private int _currentDay;
        public int CurrentDay { get { return _currentDay; } set { Set(ref _currentDay, value); } }

        private int _currentHour;

        public int CurrentHour { get { return _currentHour; } set { Set(ref _currentHour, value); } }

        private int _currentMin;
        public int CurrentMin { get { return _currentMin; } set { Set(ref _currentMin, value); } }

        private int _currentSecond;
        public int CurrentSecond { get { return _currentSecond; } set { Set(ref _currentSecond, value); } }


        public TimeItemCollection()
        {
            DateTime date = DateTime.Now;
            int month = date.Month;
            int day = date.Day;
            int hour = date.Hour;
            int min = date.Minute;
            int second = date.Second;

            for (int i = 1; i <= 12; i++)
            {
                Months.Add(new TimeItem()
                {
                    Content = $"{i.ToString().PadLeft(2, '0')}月",
                    Value = i,
                    Angle = 30 * (i - month - 1)
                });
            }
            for (int i = 1; i <= 31; i++)
            {
                Days.Add(new TimeItem()
                {
                    Content = $"{i.ToString().PadLeft(2, '0')}天",
                    Value = i,
                    Angle = 360.0 / 31.0 * (i - day - 1)
                });
            }
            for (int i = 0; i < 24; i++)
            {
                Hours.Add(new TimeItem()
                {
                    Content = $"{i.ToString().PadLeft(2, '0')}点",
                    Value = i,
                    Angle = 15 * (i - hour - 1)
                });
            }
            for (int i = 0; i < 60; i++)
            {
                Mins.Add(new TimeItem()
                {
                    Content = $"{i.ToString().PadLeft(2, '0')}分",
                    Value = i,
                    Angle = 6 * (i - min - 1)
                });
            }
            for (int i = 0; i < 60; i++)
            {
                Seconds.Add(new TimeItem()
                {
                    Content = $"{i.ToString().PadLeft(2, '0')}秒",
                    Value = i,
                    Angle = 6 * (i - second - 1)
                });
            }
        }


        public void CorrectSecond()
        {

            int second = DateTime.Now.Second;
            Parallel.For(0, 60, (i) =>
            {
                Seconds[i].Angle = 6 * (i - second -1);
            });
        }
        public void CorrectMonth()
        {

            int month = DateTime.Now.Month;
            Parallel.For(0, 12, (i) =>
            {
                Months[i].Angle = 30 * (i - month);
            });
        }
        public void CorrectDay()
        {

            int day = DateTime.Now.Day;
            Parallel.For(0, 31, (i) =>
            {
                Days[i].Angle = 360.0 / 31.0 * (i - day);
            });
        }
        public void CorrectHour()
        {

            int hour = DateTime.Now.Hour;
            Parallel.For(0, 24, (i) =>
            {
                Hours[i].Angle = 15 * (i - hour - 1);
            });
        }
        public void CorrectMin()
        {

            int min = DateTime.Now.Minute;
            Parallel.For(0, 60, (i) =>
            {
                Mins[i].Angle = 6 * (i - min - 1);
            });
        }
    }

    public class TimeItem : BaseModel
    {
        public string Content { get; set; }
        public int Value { get; set; }
        private double _anlge;
        public double Angle { get { return _anlge; } set { Set(ref _anlge, value); } }
    }

    public class BaseModel : INotifyPropertyChanged
    {
        protected bool Set<T>(ref T field, T newValue = default(T), [CallerMemberName]string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(field, newValue))
            {
                return false;
            }
            var oldValue = field;
            field = newValue;
            RaisePropertyChanged(propertyName);
            return true;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

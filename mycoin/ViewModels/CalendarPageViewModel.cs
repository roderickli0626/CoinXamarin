using mycoin.Models;
using mycoin.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Plugin.Calendar.Models;

namespace mycoin.ViewModels
{
    public class CalendarPageViewModel : BaseViewModel
    {
        //public ICommand TodayCommand => new Command(() =>
        //{
        //    Year = DateTime.Today.Year;
        //    Month = DateTime.Today.Month;
        //});

        public ICommand CalendarSelectedCommand => new Command(async (item) => await ExecuteCalendarSelectedCommand(item));
        public ICommand AddCalendarCommand => new Command(async () => await ExecuteAddCalendarCommand());
        public ICommand DeleteAllCalendarCommand => new Command(async () => await ExecuteDeleteAllCalendarCommand());

        public CalendarPageViewModel()
        {
            Events = new EventCollection();
            CreateCalendarCollection();
            //Device.BeginInvokeOnMainThread(async () => await App.Current.MainPage.DisplayAlert("Info", "Loading events with delay, and changeing current view.", "Ok"));

            //// testing all kinds of adding events
            //// when initializing collection
            //Events = new EventCollection
            //{
            //    [DateTime.Now.AddDays(-3)] = new List<EventModel>(GenerateEvents(10, "Cool")),
            //    [DateTime.Now.AddDays(4)] = new List<EventModel>(GenerateEvents(2, "Simple2")),
            //    [DateTime.Now.AddDays(2)] = new List<EventModel>(GenerateEvents(1, "Simple1")),
            //    [DateTime.Now.AddDays(1)] = new List<EventModel>(GenerateEvents(3, "Simple3")),
            //};

            //// with add method
            //Events.Add(DateTime.Now.AddDays(-1), new List<EventModel>(GenerateEvents(5, "Cool")));

            //// with indexer
            //Events[DateTime.Now] = new List<EventModel>(GenerateEvents(2, "Boring"));

            //Task.Delay(5000).ContinueWith(_ =>
            //{
            //    // indexer - update later
            //    Events[DateTime.Now] = new ObservableCollection<EventModel>(GenerateEvents(10, "Cool"));

            //    // add later
            //    Events.Add(DateTime.Now.AddDays(3), new List<EventModel>(GenerateEvents(5, "Cool")));

            //    // indexer later
            //    Events[DateTime.Now.AddDays(10)] = new List<EventModel>(GenerateEvents(10, "Boring"));

            //    // add later
            //    Events.Add(DateTime.Now.AddDays(15), new List<EventModel>(GenerateEvents(10, "Cool")));

            //    Month += 1;

            //    Task.Delay(3000).ContinueWith(t =>
            //    {
            //        // get observable collection later
            //        var todayEvents = Events[DateTime.Now] as ObservableCollection<EventModel>;

            //        // insert/add items to observable collection
            //        todayEvents.Insert(0, new EventModel { Name = "Cool event insert", TimePeriod = "This is Cool event's description!" });
            //        todayEvents.Add(new EventModel { Name = "Cool event add", TimePeriod = "This is Cool event's description!" });

            //        Month += 1;
            //    }, TaskScheduler.FromCurrentSynchronizationContext());
            //}, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private IEnumerable<EventModel> GenerateEvents(int count, string name)
        {
            return Enumerable.Range(1, count).Select(x => new EventModel
            {
                Name = $"{name} event{x}",
                TimePeriod = $"This is {name} event{x}'s description!"
            });
        }

        void CreateCalendarCollection()
        {
            List<DateTime> dates = App.Database.GetAllCalendarsAsync().Result.Select(c => c.startDate).Distinct().ToList();
            foreach (DateTime startDate in dates)
            {
                List<Calendar> calendars = App.Database.GetCalendarsAsync(startDate).Result.ToList();
                List<EventModel> events = new List<EventModel>();
                foreach (Calendar calendar in calendars)
                {
                    events.Add(new EventModel
                    {
                        ID = calendar.ID,
                        Name = calendar.substanceName,
                        TimePeriod = calendar.startTime.ToString("HH:mm:ss tt") + " ~ " + calendar.startTime.AddMinutes(calendar.Duration).ToString("HH:mm:ss tt"),
                    });
                }
                Events.Add(startDate, events);
            }
        }

        public EventCollection Events { get; }

        private int _month = DateTime.Today.Month;

        public int Month
        {
            get => _month;
            set => SetProperty(ref _month, value);
        }

        private int _year = DateTime.Today.Year;

        public int Year
        {
            get => _year;
            set => SetProperty(ref _year, value);
        }

        private DateTime _selectedDate = DateTime.Today;

        public DateTime SelectedDate
        {
            get => _selectedDate;
            set => SetProperty(ref _selectedDate, value);
        }

        private DateTime _minimumDate = new DateTime(2019, 4, 29);

        public DateTime MinimumDate
        {
            get => _minimumDate;
            set => SetProperty(ref _minimumDate, value);
        }

        private DateTime _maximumDate = DateTime.Today.AddYears(3);

        public DateTime MaximumDate
        {
            get => _maximumDate;
            set => SetProperty(ref _maximumDate, value);
        }

        private async Task ExecuteCalendarSelectedCommand(object item)
        {
            if (item is EventModel eventModel)
            {
                App.Current.MainPage = new NavigationPage(new CalendarSettingPage(eventModel.ID, null));
            }
        }

        private async Task ExecuteAddCalendarCommand()
        {
            if (SelectedDate == null) return;
            App.Current.MainPage = new NavigationPage(new CalendarSettingPage(0, SelectedDate));
        }

        private async Task ExecuteDeleteAllCalendarCommand()
        {
            var result = await App.Current.MainPage.DisplayAlert("Delete", "Really Delete All Appointment?", "OK", "Cancel");
            if (result)
            {
                await App.Database.DeleteAllCalendarsAsync(SelectedDate);
                App.Current.MainPage = new NavigationPage(new CalendarPage());
            }
            else return;
        }
    }
}

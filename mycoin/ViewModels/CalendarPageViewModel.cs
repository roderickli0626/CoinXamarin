using mycoin.DependencyServices;
using mycoin.Extensions;
using mycoin.Models;
using mycoin.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
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

        //ProgressBar State
        public System.Timers.Timer timer;

        string _imgUrl, _timelabel, _calendarTitle, _addButton, _deleteButton;
        double _progressState;
        int totalMinutes;
        public string imgUrl { get => _imgUrl; set => SetProperty(ref _imgUrl, value); }
        public string timelabel { get => _timelabel; set => SetProperty(ref _timelabel, value); }
        public string CalendarTitle { get => _calendarTitle; set => SetProperty(ref _calendarTitle, value); }
        public string AddButton { get => _addButton; set => SetProperty(ref _addButton, value); }
        public string DeleteButton { get => _deleteButton; set => SetProperty(ref _deleteButton, value); }
        public double progressState { get => _progressState; set => SetProperty(ref _progressState, value); }

        public ICommand CalendarSelectedCommand => new Command(async (item) => await ExecuteCalendarSelectedCommand(item));
        public ICommand AddCalendarCommand => new Command(async () => await ExecuteAddCalendarCommand());
        public ICommand DeleteAllCalendarCommand => new Command(async () => await ExecuteDeleteAllCalendarCommand());
        public ICommand StopCommand => new Command(async () => await ExecuteStopCommand());

        public CalendarPageViewModel()
        {
            CalendarTitle = GlobalConstants.LangGUI.GetValueOrDefault("My Plans", "My Plans");
            AddButton = "+ " + GlobalConstants.LangGUI.GetValueOrDefault("Add", "Add");
            DeleteButton = GlobalConstants.LangGUI.GetValueOrDefault("Delete", "Delete");
            Events = new EventCollection();
            CreateCalendarCollection();

            InitTimer();
            InitProgressbar();

            MessagingCenter.Subscribe<ILocalNotificationService>(this, "Music Start", (sender) =>
            {
                //Calendar Music Play handle
                timer.Start();
                imgUrl = "icons8_square_green_48.png";
            });

            MessagingCenter.Subscribe<ILocalNotificationService>(this, "Music End", (sender) =>
            {
                //Calendar Music End handle
                timer.Stop();
                imgUrl = "icons8_play_48.png";
                progressState = 1;

                totalMinutes = App.Database.GetCalendarsAsync(DateTime.Today).Result.Where(c => c.startTime > DateTime.Now).Sum(c => c.Duration);
                timelabel = (totalMinutes / 60).ToString("D2") + ":" + (totalMinutes % 60).ToString("D2") + ":" + "00";
            });
        }
        void InitTimer()
        {
            timer = new System.Timers.Timer(1000);
            timer.Elapsed += TimerElapsed;
            timer.AutoReset = true;
        }
        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            //timelabel = DateTime.Now.Subtract(startTime).ToString().Substring(0, 8);
            //timelabel = TimeSpan.Parse(timelabel).Add(DateTime.Now.Subtract(startTime)).ToString().Substring(0, 8);
            progressState = 1 - (DateTime.Now.Subtract(GlobalConstants.StartTime)).TotalSeconds / (GlobalConstants.Duration * 60);
            var totalSeconds = (int)(totalMinutes * 60 - (DateTime.Now.Subtract(GlobalConstants.StartTime)).TotalSeconds);
            timelabel = (totalSeconds / 3600).ToString("D2") + ":" + ((totalSeconds % 3600) / 60).ToString("D2") + ":" + ((totalSeconds % 3600) % 60).ToString("D2");
        }
        void InitProgressbar()
        {
            totalMinutes = App.Database.GetCalendarsAsync(DateTime.Today).Result.Where(c => c.startTime > DateTime.Now).Sum(c => c.Duration);
            if (GlobalConstants.RunFlag)
            {
                imgUrl = "icons8_square_green_48.png";
                totalMinutes += GlobalConstants.Duration;
                timelabel = (totalMinutes / 60).ToString("D2") + ":" + (totalMinutes % 60).ToString("D2") + ":" + "00";
                progressState = 1;
                timer.Start();
            }
            else
            {
                imgUrl = "icons8_play_48.png";
                progressState = 1;
                timelabel = (totalMinutes / 60).ToString("D2") + ":" + (totalMinutes % 60).ToString("D2") + ":" + "00";
            }
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
                    var substance = App.Database.GetNotesAsync().Result.Where(n => n.Substance == calendar.substanceName).FirstOrDefault();
                    int substanceID = substance == null ? 0 : substance.SubstanceID;
                    events.Add(new EventModel
                    {
                        ID = calendar.ID,
                        Name = GlobalConstants.SubTexts.GetValueOrDefault(substanceID, calendar.substanceName ?? ""),
                        TimePeriod = calendar.startTime.ToString("HH:mm:ss tt") + " ~ " + calendar.startTime.AddMinutes(calendar.Duration).ToString("HH:mm:ss tt"),
                        Duration = GlobalConstants.LangGUI.GetValueOrDefault("Duration", "Duration") + ":" + ((calendar.Duration > 59) ? calendar.Duration / 60 + "h " + calendar.Duration % 60 + "min" : calendar.Duration + "min")
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
            var result = await App.Current.MainPage.DisplayAlert(GlobalConstants.LangGUI.GetValueOrDefault("Delete", "Delete"), 
                GlobalConstants.LangGUI.GetValueOrDefault("Really Delete All Appointment?", "Really Delete All Appointment?"), GlobalConstants.LangGUI.GetValueOrDefault("OK", "OK"), GlobalConstants.LangGUI.GetValueOrDefault("Cancel", "Cancel"));
            if (result)
            {
                await App.Database.DeleteAllCalendarsAsync(SelectedDate);

                //Remove from notifications
                List<int> calendarIds = App.Database.GetCalendarsAsync(SelectedDate).Result.ToList().Select(s => s.ID).ToList();
                foreach (int id in calendarIds)
                {
                    DependencyService.Get<ILocalNotificationService>().Cancel(id);
                }

                App.Current.MainPage = new NavigationPage(new CalendarPage());
            }
            else return;
        }

        private async Task ExecuteStopCommand()
        {
            //MessagingCenter.Send(EventArgs.Empty, "Music Stop(Close)", "CalendarPage");
            if (GlobalConstants.RunFlag) MessagingCenter.Send<CalendarPageViewModel>(this, "Music Stop(Close)");
            else return;
        }
    }
}

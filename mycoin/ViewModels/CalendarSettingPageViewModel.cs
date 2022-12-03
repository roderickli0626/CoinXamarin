using mycoin.Models;
using mycoin.Views;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xam.Plugin;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;
using static Xam.Plugin.PopupMenu;
using Calendar = mycoin.Models.Calendar;

namespace mycoin.ViewModels
{
    public class CalendarSettingPageViewModel : BaseViewModel
    {
        string _musicName, _totalDuration, _repeat, _startTime, _saveIcon, _hours, _minutes, _shours, _sminutes;
        bool _musicEnabled;

        public string MusicName { get => _musicName; set => SetProperty(ref _musicName, value); }
        public string TotalDuration { get => _totalDuration; set => SetProperty(ref _totalDuration, value); }
        public string StartTime { get => _startTime; set => SetProperty(ref _startTime, value); }
        public string Repeat { get => _repeat; set => SetProperty(ref _repeat, value); }
        public string saveIcon { get => _saveIcon; set => SetProperty(ref _saveIcon, value); }
        public string hours { get => _hours; set => SetProperty(ref _hours, value); }
        public string minutes { get => _minutes; set => SetProperty(ref _minutes, value); }
        public string Shours { get => _shours; set => SetProperty(ref _shours, value); }
        public string Sminutes { get => _sminutes; set => SetProperty(ref _sminutes, value); }
        public bool musicEnabled { get => _musicEnabled; set => SetProperty(ref _musicEnabled, value); }

        public List<MySubstance> mySubstances { get; private set; }
        public ObservableRangeCollection<MyGroupViewModel> MyGroups { get; private set; }
            = new ObservableRangeCollection<MyGroupViewModel>();

        public ICommand LoadDataCommand { get; private set; }
        public ICommand ShowPopUpClickCommand { get; private set; }
        public ICommand HeaderClickCommand { get; private set; }
        public ICommand SaveCommand { get; private set; }

        public PopupMenu Popup;
        //public Note note;
        public int noteId;
        public Calendar calendar;
        public bool isEdit;

        public List<string> HoursList { get; private set; }
        public List<string> MinutesList { get; private set; }

        public CalendarSettingPageViewModel(int id, DateTime? selectedDate)
        {
            this.LoadDataCommand = new Command(async () => await ExecuteLoadDataCommand());
            this.SaveCommand = new Command(async () => await ExecuteSaveCommand());
            this.HeaderClickCommand = new Command<MyGroupViewModel>((item) => ExecuteHeaderClickCommand(item));
            this.ShowPopUpClickCommand = new Command<object>((sender) => ExecuteShowPopUpClickCommand(sender));

            Popup = new PopupMenu()
            {
                ItemsSource = new List<string>() { "Add to Calendar" },
            };
            Popup.OnItemSelected += ItemSelectedDelegate;

            InitTitle();

            if (id == 0)
            {
                isEdit = false;
                calendar = new Calendar();
                calendar.startDate = selectedDate ?? DateTime.Today;
                musicEnabled = false;
                saveIcon = "icons8_save.png";
                hours = "00";
                minutes = "00";
                Shours = DateTime.Now.Hour.ToString("D2");
                Sminutes = DateTime.Now.Minute.ToString("D2");
            }
            else
            {
                isEdit = true;
                calendar = App.Database.GetCalendarAsync(id).Result;
                MusicName = calendar.substanceName ?? "No Name Music";
                StartTime = calendar.startTime.ToString("HH:mm");
                TotalDuration = calendar.Duration / 60 + "h " + calendar.Duration % 60 + "min";
                musicEnabled = true;
                saveIcon = "icons8_save_blue.png";
                hours = (calendar.Duration / 60).ToString("D2");
                minutes = (calendar.Duration % 60).ToString("D2");
                Shours = calendar.startTime.Hour.ToString("D2");
                Sminutes = calendar.startTime.Minute.ToString("D2");
            }


            HoursList = new List<string>();
            MinutesList = new List<string>();
            for (int i = 0; i < 60; i++)
            {
                string ii = i.ToString("D2");
                if (i < 24) HoursList.Add(ii);
                MinutesList.Add(ii);
            }
        }

        void InitTitle()
        {
            MusicName = "MUSIC NAME";
            StartTime = "START TIME";
            TotalDuration = "TOTAL DURATION";
            Repeat = "REPEAT";
        }

        private async Task ExecuteLoadDataCommand()
        {
            List<Note> noteList = App.Database.GetNotesAsync().Result;
            foreach (string groupName in noteList.Select(n => n.GroupName ?? "No GroupName").Distinct().ToList())
            {
                mySubstances = new List<MySubstance>();

                foreach (Note note in noteList.Where(s => s.GroupName == groupName).ToList())
                {
                    mySubstances.Add(new MySubstance
                    {
                        ID = note.ID,
                        SubstanceImageUrl = "dot.png",
                        SubstanceName = note.Substance ?? "No SubstanceName",
                        Duration = note.Duration,
                    });
                }

                MyGroups.Add(new MyGroupViewModel(new MyGroup
                {
                    ImageUrl = "ic_biapp_icon_favorit.xml",
                    GroupName = groupName,
                    MySubstances = mySubstances
                }));
            }
        }

        private void ExecuteHeaderClickCommand(MyGroupViewModel item)
        {
            item.Expanded = !item.Expanded;
        }

        private void ExecuteShowPopUpClickCommand(object sender)
        {
            ImageButton btn = sender as ImageButton;
            MySubstanceViewModel substance = btn.BindingContext as MySubstanceViewModel;
            if (substance == null) return;

            noteId = substance.ID;

            Popup.ShowPopup(sender as View);
        }

        private async Task ExecuteSaveCommand()
        {
            if (calendar.substanceName == null) return;
            calendar.startTime = DateTime.ParseExact(Shours + ":" + Sminutes, "HH:mm", CultureInfo.InvariantCulture);
            calendar.Duration = Convert.ToInt32(hours) * 60 + Convert.ToInt32(minutes);
            if (isEdit) await App.Database.UpdateCalendarAsync(calendar);
            else await App.Database.SaveCalendarAsync(calendar);
            App.Current.MainPage = new NavigationPage(new CalendarPage());
        }


        protected void ItemSelectedDelegate(string item)
        {
            if (item == "Add to Calendar")
            {
                if (noteId == 0) return;
                Note note = App.Database.GetNoteAsync(noteId).Result;
                calendar.substanceName = note.Substance;
                calendar.WavFile = note.WavFile;
                MusicName = note.Substance ?? "No Name Music";
                musicEnabled = true;
                saveIcon = "icons8_save_blue.png";
            }
            else return;
        }
    }
}

﻿using mycoin.Extensions;
using mycoin.Models;
using mycoin.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.BehaviorsPack;

namespace mycoin.ViewModels
{
    public class MainDashboardPage1ViewModel : BaseViewModel
    {
        public System.Timers.Timer timer;
        public DateTime startTime;
        public Plugin.SimpleAudioPlayer.ISimpleAudioPlayer audio;
        public int timerCount = 0;
        public bool endFlag = false;
        public bool closeFlag = false;
        public bool stopFlag = false;

        string _markImageUrl, _timelabel, _title, _favouriteTab, _historyTab, _allTab, _searchHolder;
        bool _stopContainer, _continueContainer;
        int _selectedIndex;

        public string markImageUrl { get => _markImageUrl; set => SetProperty(ref _markImageUrl, value); }
        public string timelabel { get => _timelabel; set => SetProperty(ref _timelabel, value); }
        public bool StopContainer { get => _stopContainer; set => SetProperty(ref _stopContainer, value); }
        public bool ContinueContainer { get => _continueContainer; set => SetProperty(ref _continueContainer, value); }
        public int selectedIndex { get => _selectedIndex; set => SetProperty(ref _selectedIndex, value); }
        public string Title { get => _title; set => SetProperty(ref _title, value); }
        public string FavouriteTab { get => _favouriteTab; set => SetProperty(ref _favouriteTab, value); }
        public string HistoryTab { get => _historyTab; set => SetProperty(ref _historyTab, value); }
        public string AllTab { get => _allTab; set => SetProperty(ref _allTab, value); }
        public string SearchHolder { get => _searchHolder; set => SetProperty(ref _searchHolder, value); }

        public ObservableRangeCollection<MyGroupViewModel> MyGroups { get; private set; }
            = new ObservableRangeCollection<MyGroupViewModel>();

        public ICommand LoadDataCommand { get; private set; }
        public ICommand HeaderClickCommand { get; private set; }

        public List<MySubstance> mySubstances { get; private set; }

        readonly IList<MySubstance> favoriteSource;
        public ObservableCollection<MySubstance> MyFavoriteSubstances { get; private set; }

        public MainDashboardPage1ViewModel(string from)
        {
            this.LoadDataCommand = new Command(async () => await ExecuteLoadDataCommand());
            this.HeaderClickCommand = new Command<MyGroupViewModel>((item) => ExecuteHeaderClickCommand(item));
            favoriteSource = new List<MySubstance>();
            CreateFavoriteSubstanceCollection();
            InitAudioPlayer();
            InitTimer();

            markImageUrl = "ic_coin_large_background_silver.png";
            timelabel = "";
            StopContainer = false;
            ContinueContainer = false;
            Title = GlobalConstants.LangGUI.GetValueOrDefault("My Applications", "My Applications");
            FavouriteTab = GlobalConstants.LangGUI.GetValueOrDefault("Favorites", "Favorites");
            HistoryTab = GlobalConstants.LangGUI.GetValueOrDefault("History", "History");
            AllTab = GlobalConstants.LangGUI.GetValueOrDefault("All Applications", "All Applications");
            SearchHolder = GlobalConstants.LangGUI.GetValueOrDefault("Search", "Search");

            if (from == "default")
                selectedIndex = 2;
            else selectedIndex = 0;
        }

        void InitAudioPlayer()
        {
            audio = Plugin.SimpleAudioPlayer.CrossSimpleAudioPlayer.Current;
            //if (audio == null || note.WavFile == null) return;
            //audio.Load(new MemoryStream(note.WavFile));
            audio.Loop = true;
            audio.PlaybackEnded += EndAudioPlaying;
        }

        void InitTimer()
        {
            timer = new System.Timers.Timer(1000);
            timer.Elapsed += TimerElapsed;
            timer.AutoReset = true;
        }
        private void TimerElapsed(object sender, ElapsedEventArgs e)
        {
            timelabel = DateTime.Now.Subtract(startTime).ToString().Substring(0, 8);
            //timelabel = TimeSpan.Parse(timelabel).Add(DateTime.Now.Subtract(startTime)).ToString().Substring(0, 8);
        }
        void EndAudioPlaying(object sender, EventArgs e)
        {
            endFlag = true;
            //timer.Stop();
            //markImageUrl = "ic_coin_large_background_silver.png";
            //timelabel = "";
            //StopContainer = false;
            //ContinueContainer = false;
        }

        public ICommand playCommand => new Command<object>(async (arg) => {
            MySubstance substance = arg as MySubstance;
            if (substance == null) return;
            var favoriteId = substance.ID;
            if (favoriteId == 0) selectedIndex = 2;
            else
            {
                //int id = int.Parse(arg);
                Note note = App.Database.GetNoteAsync(favoriteId).Result;
                markImageUrl = "animation_green_02.png";
                markImageUrl = "animation_green_02.gif";
                StopContainer = true;
                ContinueContainer = false;
                timelabel = "00:00:00";

                startTime = DateTime.Now;
                if (audio == null || note.WavFile == null) return;
                audio.Load(new MemoryStream(note.WavFile));
                audio.Play();
                timer.Start();
                timerCount = 0;
                closeFlag = false;
                stopFlag = false;
                Device.StartTimer(TimeSpan.FromSeconds(0.1), () =>
                {
                    if (closeFlag) return false;
                    if (stopFlag) return true;
                    else timerCount++;
                    double total = timerCount * 0.1;
                    if (note.Duration * 60 > total)
                    {
                        if (endFlag)
                        {
                            audio.Play();
                            endFlag = false;
                        }
                        return true;
                    }
                    else
                    {
                        audio.Stop();
                        timer.Stop();
                        markImageUrl = "ic_coin_large_background_silver.png";
                        timelabel = "";
                        StopContainer = false;
                        ContinueContainer = false;
                        return false;
                    }
                });

            }
        });

        public ICommand stopCommand => new Command(async () => {
            audio.Pause();
            stopFlag = true;
            closeFlag = false;
            markImageUrl = "animation_blue_02.gif";
            //markImageUrl = "animation_green_02.gif";
            StopContainer = false;
            ContinueContainer = true;
            timer.Stop();
        });

        public ICommand closeCommand => new Command(async () => {
            audio.Play();
            audio.Stop();
            timer.Stop();
            closeFlag = true;
            markImageUrl = "ic_coin_large_background_silver.png";
            StopContainer = false;
            ContinueContainer = false;
            timelabel = "";
        });

        public ICommand continueCommand => new Command(async () => {
            audio.Play();
            startTime = DateTime.Now.Subtract(TimeSpan.Parse(timelabel));
            timer.Start();
            stopFlag = false;
            closeFlag = false;
            markImageUrl = "animation_green_02.gif";
            StopContainer = true;
            ContinueContainer = false;
        });

        private async Task ExecuteLoadDataCommand()
        {
            List<Note> noteList = App.Database.GetNotesAsync().Result;
            foreach (string groupName in noteList.Select(n => n.GroupName ?? "No GroupName").Distinct().ToList())
            {
                int groupNumber = noteList.Where(n => n.GroupName == groupName).FirstOrDefault().GroupNumber;
                mySubstances = new List<MySubstance>();

                foreach (Note note in noteList.Where(s => s.GroupName == groupName).ToList())
                {
                    mySubstances.Add(new MySubstance
                    {
                        ID = note.ID,
                        SubstanceImageUrl = "dot.png",
                        SubstanceName = GlobalConstants.SubTexts.GetValueOrDefault(note.SubstanceID, note.Substance ?? "No SubstanceName"),
                        Duration = note.Duration,
                    });
                }

                MyGroups.Add(new MyGroupViewModel(new MyGroup
                {
                    ImageUrl = "ic_biapp_icon_favorit.xml",
                    GroupName = GlobalConstants.GroupTexts.GetValueOrDefault(groupNumber, groupName),
                    MySubstances = mySubstances
                }));
            }
        }
        private void ExecuteHeaderClickCommand(MyGroupViewModel item)
        {
            item.Expanded = !item.Expanded;
        }

        

        void CreateFavoriteSubstanceCollection()
        {
            List<Note> noteList = App.Database.GetNotesAsync().Result.Where(n => n.Isfavorite).ToList();
            foreach (Note note in noteList)
            {
                favoriteSource.Add(new MySubstance
                {
                    ID = note.ID,
                    SubstanceImageUrl = "icons8_play_button_circled_50.png",
                    SubstanceName = GlobalConstants.SubTexts.GetValueOrDefault(note.SubstanceID, note.Substance ?? "No SubstanceName"),
                    Duration = note.Duration,
                    DurationTimeFormat = note.Duration > 59 ? (note.Duration / 60) + "h " + (note.Duration % 60) + "min" : (note.Duration % 60) + "min",
                    favoriteExtraIconVisible = true
                });
            }
            favoriteSource.Add(new MySubstance
            {
                ID = 0,
                SubstanceImageUrl = "icons8_plus_50",
                DurationTimeFormat = "",
                favoriteExtraIconVisible = false
            });

            MyFavoriteSubstances = new ObservableCollection<MySubstance>(favoriteSource);
        }
    }
}

using mycoin.Extensions;
using mycoin.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;
using XF.Material.Forms.Resources;
using XF.Material.Forms.UI.Dialogs.Configurations;
using XF.Material.Forms.UI.Dialogs;
using mycoin.Views;
using System.IO;
using System.Timers;
using mycoin.DependencyServices;

namespace mycoin.ViewModels
{
    public class MainDashboardPageViewModel : BaseViewModel
    {
        readonly IList<MySubstance> source;
        public ObservableCollection<MySubstance> MySubstances { get; private set; }
        public MainDashboardPageViewModel()
        {
            Note note = App.Database.GetNotesAsync().Result.Where(n => n.Isfavorite && n.PlayDateTime == null).FirstOrDefault();
            markImageUrl = note == null ? GetMarkDefaultURL() : "ic_coin_large_background_silver.png";
            //visibleFlag = note == null ? false : true;
            visibleFlag = true;
            Align = note == null ? "Vertical" : "Horizontal";
            nextTherapy = GlobalConstants.LangGUI.GetValueOrDefault("Your Next Therapy", "Your Next Therapy");
            firstProgram = note == null ? "No Program" : GlobalConstants.SubTexts.GetValueOrDefault(note.SubstanceID, note.Substance ?? "No Program");
            currentDate = note == null ? "No DefaultTime" : note.DefaultDateTime.ToString();
            titleFromUserInfo = GlobalConstants.LangGUI.GetValueOrDefault("Hello", "Hello") + " " 
                + App.Userdata.userName.Substring(0, (App.Userdata.userName.Length > 12 ? 12 : App.Userdata.userName.Length));
            timelabel = note == null ? "00:00:00" : DateTime.Parse("00:00:00").AddMinutes(note.Duration).Subtract(DateTime.Parse("00:00:00")).ToString().Substring(0, 8);
            StopContainer = false;
            ContinueContainer = false;
            source = new List<MySubstance>();
            CreateGroupCollection();
            InitAudioPlayer();
            InitTimer();
        }

        public System.Timers.Timer timer;
        public DateTime startTime, endTime;
        public Plugin.SimpleAudioPlayer.ISimpleAudioPlayer audio;
        public int timerCount = 0;
        public bool endFlag = false;
        public bool closeFlag = false;
        public bool stopFlag = false;

        string _currentDate, _firstProgram, _titleFromUserInfo, _markImageUrl, _align, _nextTherapy, _timelabel;
        bool _visibleFlag, _stopContainer, _continueContainer;
        public string currentDate { get => _currentDate; set => SetProperty(ref _currentDate, value); }

        public string firstProgram { get => _firstProgram; set => SetProperty(ref _firstProgram, value); }
        public string titleFromUserInfo { get => _titleFromUserInfo; set => SetProperty(ref _titleFromUserInfo, value); }
        public string markImageUrl { get => _markImageUrl; set => SetProperty(ref _markImageUrl, value); }
        public bool visibleFlag { get => _visibleFlag; set => SetProperty(ref _visibleFlag, value); }
        public string Align { get => _align; set => SetProperty(ref _align, value); }
        public string nextTherapy { get => _nextTherapy; set => SetProperty(ref _nextTherapy, value); }
        public string timelabel { get => _timelabel; set => SetProperty(ref _timelabel, value); }
        public bool StopContainer { get => _stopContainer; set => SetProperty(ref _stopContainer, value); }
        public bool ContinueContainer { get => _continueContainer; set => SetProperty(ref _continueContainer, value); }

        public void CreateGroupCollection()
        {
            List<Note> noteList = App.Database.GetNotesAsync().Result.Where(n => n.Isfavorite).ToList();
            foreach (Note note in noteList)
            {
                source.Add(new MySubstance
                {
                    ID = note.ID,
                    SubstanceImageUrl = "ic_biapp_icon_favorit.xml",
                    SubstanceName = GlobalConstants.SubTexts.GetValueOrDefault(note.SubstanceID, note.Substance ?? "No SubstanceName"),
                    Duration = note.Duration,
                    DurationTimeFormat = note.Duration > 59 ? (note.Duration / 60) + "h " + (note.Duration % 60) + "min" : (note.Duration % 60) + "min",
                    favoriteExtraIconVisible = true,
                    SubstanceID = note.SubstanceID,
                });

            }
            source.Add(new MySubstance
            {
                ID = 0,
                //SubstanceImageUrl = "ic_biapp_icon_pause.xml",
                SubstanceImageUrl = "ic_biapp_icon_favorit_hinzufuegen.xml",
                SubstanceName = GlobalConstants.LangGUI.GetValueOrDefault("Favorites Add", "Favorites Add"),
                DurationTimeFormat = "",
                favoriteExtraIconVisible = false
            });
            MySubstances = new ObservableCollection<MySubstance>(source);
        }

        string GetMarkDefaultURL()
        {
            AppSettings setting = App.Database.GetSettingsAsync().Result;
            if (setting == null || setting.coverSkinDefault == "") return "ic_coin_large_background_wood.png";
            else return setting.coverSkinDefault ?? "ic_coin_large_background_wood.png";
        }

        // Commands
        public ICommand LongPressCommand => new Command<object>(async (arg) =>
        {
            MySubstance substance = arg as MySubstance;
            if (substance == null) return;
            var deletFavorId = substance.ID;
            if (deletFavorId == 0) return;

            var alertDialogConfiguration = new MaterialAlertDialogConfiguration()
            {
                BackgroundColor = XF.Material.Forms.Material.GetResource<Color>(MaterialConstants.Color.SURFACE),
                TitleTextColor = XF.Material.Forms.Material.GetResource<Color>(MaterialConstants.Color.ON_SURFACE),
                MessageTextColor = XF.Material.Forms.Material.GetResource<Color>(MaterialConstants.Color.ON_SURFACE),
                //TintColor = XF.Material.Forms.Material.GetResource<Color>(MaterialConstants.Color.ON_BACKGROUND),
                TintColor = Color.FromHex("#018BD3"),
                CornerRadius = 30,
                ScrimColor = Color.FromHex("#232F34").MultiplyAlpha(0.32),
                ButtonAllCaps = false
            };
            var deleteFromFavorite = await MaterialDialog.Instance.ConfirmAsync(GlobalConstants.LangGUI.GetValueOrDefault("Do you want to delete selected substance from favorites?", "Do you want to delete selected substance from favorites?"),
                GlobalConstants.LangGUI.GetValueOrDefault("Delete", "Delete"), GlobalConstants.LangGUI.GetValueOrDefault("OK", "OK"), GlobalConstants.LangGUI.GetValueOrDefault("Cancel", "Cancel"), alertDialogConfiguration);

            if (deleteFromFavorite ?? false)
            {
                Note note = App.Database.GetNoteAsync(deletFavorId).Result;
                note.Isfavorite = false;
                await App.Database.UpdateNoteAsync(note);
                await App.Database.DeleteFavoritesBySubstanceIDAsync(substance.SubstanceID);
                closeCommand.Execute(this);
                App.Current.MainPage = new NavigationPage(new MainDashboardPage());
            }
        });


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
            timelabel = endTime.Subtract(DateTime.Now).ToString().Substring(0, 8);
            //timelabel = DateTime.Now.Subtract(startTime).ToString().Substring(0, 8);
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
            try
            {
                MySubstance substance = arg as MySubstance;
                if (substance == null) return;
                var favoriteId = substance.ID;
                if (favoriteId == 0) return;
                else
                {
                    //int id = int.Parse(arg);
                    Note note = App.Database.GetNoteAsync(favoriteId).Result;
                    if (audio == null || note.WavFile == null)
                    {
                        var alertDialogConfiguration = new MaterialAlertDialogConfiguration()
                        {
                            BackgroundColor = XF.Material.Forms.Material.GetResource<Color>(MaterialConstants.Color.SURFACE),
                            TitleTextColor = XF.Material.Forms.Material.GetResource<Color>(MaterialConstants.Color.ON_SURFACE),
                            MessageTextColor = XF.Material.Forms.Material.GetResource<Color>(MaterialConstants.Color.ON_SURFACE),
                            //TintColor = XF.Material.Forms.Material.GetResource<Color>(MaterialConstants.Color.ON_BACKGROUND),
                            TintColor = Color.FromHex("#018BD3"),
                            CornerRadius = 30,
                            ScrimColor = Color.FromHex("#232F34").MultiplyAlpha(0.32),
                            ButtonAllCaps = false
                        };
                        var result = await MaterialDialog.Instance.ConfirmAsync(GlobalConstants.LangGUI.GetValueOrDefault("There is no content in the Substance", "There is no content in the Substance"),
                        GlobalConstants.LangGUI.GetValueOrDefault("Warning", "Warning"), GlobalConstants.LangGUI.GetValueOrDefault("OK", "OK"), "", alertDialogConfiguration);

                        return;
                    }
                    markImageUrl = "animation_green_02.png";
                    markImageUrl = "animation_green_02.gif";
                    StopContainer = true;
                    ContinueContainer = false;
                    //timelabel = "00:00:00";
                    timelabel = note == null ? "00:00:00" : DateTime.Parse("00:00:00").AddMinutes(note.Duration).Subtract(DateTime.Parse("00:00:00")).ToString().Substring(0, 8);
                    nextTherapy = GlobalConstants.LangGUI.GetValueOrDefault("Your Current Therapy", "Your Current Therapy");
                    firstProgram = note == null ? "No Program" : GlobalConstants.SubTexts.GetValueOrDefault(note.SubstanceID, note.Substance ?? "No Program");

                    startTime = DateTime.Now;
                    note.PlayDateTime = startTime;
                    await App.Database.UpdateNoteAsync(note);
                    endTime = startTime.AddMinutes(note.Duration).AddSeconds(1);

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
                            timelabel = "00:00:00";
                            StopContainer = false;
                            ContinueContainer = false;

                            Note note1 = App.Database.GetNotesAsync().Result.Where(n => n.Isfavorite && n.PlayDateTime == null).FirstOrDefault();
                            if (note1 != null)
                            {
                                //Send Message to notice Music End
                                MessagingCenter.Send<MainDashboardPageViewModel>(this, "Favor End");
                            }
                            else
                            {
                                nextTherapy = GlobalConstants.LangGUI.GetValueOrDefault("Your Next Therapy", "Your Next Therapy");
                                firstProgram = note1 == null ? "No Program" : GlobalConstants.SubTexts.GetValueOrDefault(note1.SubstanceID, note1.Substance ?? "No Program");
                            }
                            return false;
                        }
                    });

                }
            }
            catch (Exception e)
            {
                Console.Write(e.ToString());
            }

        });

        public ICommand stopCommand => new Command(async () => {
            try
            {
                audio.Pause();
                stopFlag = true;
                closeFlag = false;
                markImageUrl = "animation_blue_02.png";
                markImageUrl = "animation_blue_02.gif";
                //markImageUrl = "animation_green_02.gif";
                StopContainer = false;
                ContinueContainer = true;
                timer.Stop();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        });

        public ICommand closeCommand => new Command(async () => {
            audio.Play();
            audio.Stop();
            timer.Stop();
            closeFlag = true;
            markImageUrl = "ic_coin_large_background_silver.png";
            StopContainer = false;
            ContinueContainer = false;
            //timelabel = "00:00:00";
            Note note1 = App.Database.GetNotesAsync().Result.Where(n => n.Isfavorite && n.PlayDateTime == null).FirstOrDefault();
            nextTherapy = GlobalConstants.LangGUI.GetValueOrDefault("Your Next Therapy", "Your Next Therapy");
            firstProgram = note1 == null ? "No Program" : GlobalConstants.SubTexts.GetValueOrDefault(note1.SubstanceID, note1.Substance ?? "No Program");
            timelabel = note1 == null ? "00:00:00" : DateTime.Parse("00:00:00").AddMinutes(note1.Duration).Subtract(DateTime.Parse("00:00:00")).ToString().Substring(0, 8);
        });

        public ICommand continueCommand => new Command(async () => {
            audio.Play();
            startTime = DateTime.Now.Subtract(TimeSpan.Parse(timelabel));
            endTime = DateTime.Now.Add(TimeSpan.Parse(timelabel));
            timer.Start();
            stopFlag = false;
            closeFlag = false;
            markImageUrl = "animation_green_02.png";
            markImageUrl = "animation_green_02.gif";
            StopContainer = true;
            ContinueContainer = false;
        });
    }
}

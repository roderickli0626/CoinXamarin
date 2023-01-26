using mycoin.Extensions;
using mycoin.Models;
using mycoin.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Xam.Plugin;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XF.Material.Forms.Resources;
using XF.Material.Forms.UI.Dialogs.Configurations;
using XF.Material.Forms.UI.Dialogs;

namespace mycoin.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainDashboardPage1 : ContentPage
    {
        MainDashboardPage1ViewModel vm;
        public PopupMenu Popup;
        private int id;
        public ViewCell lastCell;

        //public System.Timers.Timer timer;
        //public DateTime startTime;
        //public Plugin.SimpleAudioPlayer.ISimpleAudioPlayer audio;
        //public int timerCount = 0;
        //public bool endFlag = false;
        //public bool closeFlag = false;
        //public bool stopFlag = false;

        public MainDashboardPage1(string from = "default")
        {
            InitializeComponent();
            this.BindingContext = vm = new MainDashboardPage1ViewModel(from);

            Popup = new PopupMenu()
            {
                ItemsSource = new List<string>() { GlobalConstants.LangGUI.GetValueOrDefault("Add to Favorite", "Add to Favorite"), GlobalConstants.LangGUI.GetValueOrDefault("Test PLAY", "Test PLAY") },
            };

            Popup.OnItemSelected += ItemSelectedDelegate;

            

            //InitAudioPlayer();
            //InitTimer();

            if (Device.RuntimePlatform == Device.Android) Padding = new Thickness(0, 10, 0, 0);
            BackgroundColor = Color.White;
            // Remove the Navigation bar form the top of the page 
            Xamarin.Forms.NavigationPage.SetHasNavigationBar(this, false);
        }

        //void InitAudioPlayer()
        //{
        //    audio = Plugin.SimpleAudioPlayer.CrossSimpleAudioPlayer.Current;
        //    //if (audio == null || note.WavFile == null) return;
        //    //audio.Load(new MemoryStream(note.WavFile));
        //    audio.Loop = true;
        //    audio.PlaybackEnded += EndAudioPlaying;
        //}

        //void InitTimer()
        //{
        //    timer = new System.Timers.Timer(1000);
        //    timer.Elapsed += TimerElapsed;
        //    timer.AutoReset = true;
        //}

        //private void TimerElapsed(object sender, ElapsedEventArgs e)
        //{
        //    timelabel.Text = e.SignalTime.ToString("HH:mm:ss");
        //    //timelabel.Text = (DateTime.Now - startTime).ToString("HH:mm:ss");
        //}

        //void EndAudioPlaying(object sender, EventArgs e)
        //{
        //    endFlag = true;
        //    timer.Stop();
        //    markImageUrl.Source = "ic_coin_large_background_silver.png";
        //    timelabel.Text = "aaa";
        //    StopContainer.IsVisible = false;
        //    ContinueContainer.IsVisible = false;
        //}

        void OnImageButtonClicked(object sender, EventArgs e)
        {
            MessagingCenter.Send(EventArgs.Empty, "OpenMenu", "MainDashboardPage1");
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            vm.LoadDataCommand.Execute(null);

        }

        private void StateImage_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("Source"))
            {
                var image = sender as Image;
                image.Opacity = 0;
                image.FadeTo(1, 1000);
            }
        }

        void ShowPopup_Clicked(object sender, EventArgs e)
        {
            ImageButton btn = sender as ImageButton;
            MySubstanceViewModel substance = btn.BindingContext as MySubstanceViewModel;
            if (substance == null) return;

            id = substance.ID;
           
            Popup.ShowPopup(sender as View);
        }

        //void FavoriteImageButton_Clicked(object sender, EventArgs e)
        //{
        //    ImageButton btn = sender as ImageButton;
        //    MySubstance substance = btn.BindingContext as MySubstance;
        //    if (substance == null) return;
        //    var favoriteId = substance.ID;
        //    if (favoriteId == 0) App.Current.MainPage = new NavigationPage(new MainDashboardPage1("default"));
        //    else
        //    {
        //        Note note = App.Database.GetNoteAsync(favoriteId).Result;
        //        markImageUrl.Source = "animation_green_02.png";
        //        markImageUrl.Source = "animation_green_02.gif";
        //        StopContainer.IsVisible = true;
        //        ContinueContainer.IsVisible = false;

        //        timelabel.Text = "";

        //        startTime = DateTime.Now;
        //        if (audio == null || note.WavFile == null) return;
        //        audio.Load(new MemoryStream(note.WavFile));
        //        audio.Play();
        //        timer.Start();
        //        timerCount = 0;
        //        closeFlag = false;
        //        stopFlag = false;
        //        //Device.StartTimer(TimeSpan.FromSeconds(0.1), () =>
        //        //{
        //        //    if (closeFlag) return false;
        //        //    if (stopFlag) return true;
        //        //    else timerCount++;
        //        //    double total = timerCount * 0.1;
        //        //    if (note.Duration * 60 > total)
        //        //    {
        //        //        if (endFlag)
        //        //        {
        //        //            audio.Play();
        //        //            endFlag = false;
        //        //        }
        //        //        return true;
        //        //    }
        //        //    else
        //        //    {
        //        //        audio.Stop();
        //        //        timer.Stop();
        //        //        markImageUrl.Source = "ic_coin_large_background_silver.png";
        //        //        timelabel.Text = "";
        //        //        StopContainer.IsVisible = false;
        //        //        ContinueContainer.IsVisible = false;
        //        //        return false;
        //        //    }
        //        //});

        //    }
        //}

        protected async void ItemSelectedDelegate(string item)
        {
            if (item == GlobalConstants.LangGUI.GetValueOrDefault("Add to Favorite", "Add to Favorite")) {
                if (id == 0) return;
                Note note = App.Database.GetNoteAsync(id).Result;
                if (note.WavFile == null)
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
                    await MaterialDialog.Instance.ConfirmAsync(GlobalConstants.LangGUI.GetValueOrDefault("Can't add to favorites because there is no content in the Substance", "Can't add to favorites because there is no content in the Substance"),
                    GlobalConstants.LangGUI.GetValueOrDefault("Warning", "Warning"), GlobalConstants.LangGUI.GetValueOrDefault("OK", "OK"), "", alertDialogConfiguration);
                    
                    App.Current.MainPage = new NavigationPage(new MainDashboardPage1("default"));
                    return;
                }
                note.Isfavorite = true;
                App.Database.UpdateNoteAsync(note);
                App.Current.MainPage = new NavigationPage(new MainDashboardPage1("allTab"));
            }
            else if (item == GlobalConstants.LangGUI.GetValueOrDefault("Test PLAY", "Test PLAY"))
            {
                if (id == 0) return;
                Note note = App.Database.GetNoteAsync(id).Result;
                if (note.WavFile == null)
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
                    await MaterialDialog.Instance.ConfirmAsync(GlobalConstants.LangGUI.GetValueOrDefault("Can't test play because there is no content in the Substance", "Can't test play because there is no content in the Substance"),
                    GlobalConstants.LangGUI.GetValueOrDefault("Warning", "Warning"), GlobalConstants.LangGUI.GetValueOrDefault("OK", "OK"), "", alertDialogConfiguration);

                    App.Current.MainPage = new NavigationPage(new MainDashboardPage1("default"));
                    return;
                }
                App.Current.MainPage = new NavigationPage(new PlayPage(id));
            }
        }

        private void BacktoPrevious(object sender, EventArgs e)
        {
            App.Current.MainPage = new NavigationPage(new MainDashboardPage());
        }

        public void DeletFromFavorite(object sender, EventArgs e)
        {
            ImageButton btn = sender as ImageButton;
            MySubstance substance = btn.BindingContext as MySubstance;
            if (substance == null) return;
            var deletFavorId = substance.ID;
            if (deletFavorId == 0) return;
            Note note = App.Database.GetNoteAsync(deletFavorId).Result;
            note.Isfavorite = false;
            App.Database.UpdateNoteAsync(note);
            App.Current.MainPage = new NavigationPage(new MainDashboardPage1("favoriteTab"));
        }

        public async void ShowInfo(object sender, EventArgs e)
        {
            ImageButton btn = sender as ImageButton;
            MySubstance substance = btn.BindingContext as MySubstance;
            if (substance == null) return;
            var InfoId = substance.ID;
            if (InfoId == 0) return;
            Note infoSubstance = App.Database.GetNoteAsync(InfoId).Result;

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
            await MaterialDialog.Instance.ConfirmAsync(GlobalConstants.LangGUI.GetValueOrDefault("Group", "Group") +
                ":" + GlobalConstants.GroupTexts.GetValueOrDefault(infoSubstance.GroupNumber, infoSubstance.GroupName ?? "") + "\n" +
                GlobalConstants.LangGUI.GetValueOrDefault("Duration", "Duration") + ":" + infoSubstance.Duration + "min",
                GlobalConstants.SubTexts.GetValueOrDefault(infoSubstance.SubstanceID, infoSubstance.Substance ?? "") + " " +
                GlobalConstants.LangGUI.GetValueOrDefault("Information", "Information"), GlobalConstants.LangGUI.GetValueOrDefault("OK", "OK"), "", alertDialogConfiguration);

            //await DisplayAlert(GlobalConstants.SubTexts.GetValueOrDefault(infoSubstance.SubstanceID, infoSubstance.Substance ?? "") + " " + 
            //    GlobalConstants.LangGUI.GetValueOrDefault("Information", "Information"), GlobalConstants.LangGUI.GetValueOrDefault("Group", "Group") + 
            //    ":" + GlobalConstants.GroupTexts.GetValueOrDefault(infoSubstance.GroupNumber, infoSubstance.GroupName ?? "") + "\n" + 
            //    GlobalConstants.LangGUI.GetValueOrDefault("Duration", "Duration") + ":" + infoSubstance.Duration + "min", GlobalConstants.LangGUI.GetValueOrDefault("OK", "OK"));
        }

        //Close, Stop, Continue Music
        //public void BtnStopClicked(object sender, EventArgs e)
        //{
        //    audio.Pause();
        //    stopFlag = true;
        //    closeFlag = false;
        //    //markImageUrl.Source = "animation_blue_02.gif";
        //    markImageUrl.Source = "animation_green_02.gif";
        //    StopContainer.IsVisible = false;
        //    ContinueContainer.IsVisible = true;
        //    timer.Stop();
        //}

        //public void BtnCloseClicked(object sender, EventArgs e)
        //{
        //    audio.Play();
        //    audio.Stop();
        //    timer.Stop();
        //    closeFlag = true;
        //    markImageUrl.Source = "ic_coin_large_background_silver.png";
        //    StopContainer.IsVisible = false;
        //    ContinueContainer.IsVisible = false;
        //    timelabel.Text = "";
        //}

        //public void BtnContinueClicked(object sender, EventArgs e)
        //{
        //    audio.Play();
        //    timer.Start();
        //    stopFlag = false;
        //    closeFlag = false;
        //    markImageUrl.Source = "animation_green_02.gif";
        //    StopContainer.IsVisible = true;
        //    ContinueContainer.IsVisible = false;
        //}

        private void ImageButton_Clicked(object sender, EventArgs e)
        {
            App.Current.MainPage = new NavigationPage(new MainDashboardPage());
        }

        private void ViewCell_Tapped(object sender, EventArgs e)
        {
            if (lastCell != null)
                lastCell.View.BackgroundColor = Color.Transparent;
            var viewCell = (ViewCell)sender;
            if (viewCell.View != null)
            {
                viewCell.View.BackgroundColor = Color.FromHex("#B16BBF");
                lastCell = viewCell;
            }
        }
    }
}
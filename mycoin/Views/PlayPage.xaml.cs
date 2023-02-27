using mycoin.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFImageLoading.Forms;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using mycoin.Models;
using mycoin.Extensions;
using XF.Material.Forms.Resources;
using XF.Material.Forms.UI.Dialogs.Configurations;
using XF.Material.Forms.UI.Dialogs;

namespace mycoin.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PlayPage : ContentPage
    {
        PlayPageViewModel vm;
        int noteID;
        Note note = null;
        public PlayPage(int id = 0)
        {
            InitializeComponent();
            noteID = id;
            if (noteID == 0) return;
            this.BindingContext = vm = new PlayPageViewModel(noteID);

            note = App.Database.GetNoteAsync(noteID).Result;
            if (note == null) return;

            if (Device.RuntimePlatform == Device.Android) Padding = new Thickness(0, 10, 0, 0);
            BackgroundColor = Color.White;
            // Remove the Navigation bar form the top of the page 
            Xamarin.Forms.NavigationPage.SetHasNavigationBar(this, false);

            //New Module Count
            NewModuleCount.Text = GlobalConstants.NewModuleCount > 0 ? GlobalConstants.NewModuleCount.ToString() : "";
        }

        void OnImageButtonClicked(object sender, EventArgs e)
        {
            vm.closeCommand.Execute(closeBtn.Source);
            if (note.Isfavorite) App.Current.MainPage = new NavigationPage(new MainDashboardPage());
            else App.Current.MainPage = new NavigationPage(new MainDashboardPage1());
        }

        public async void FavoriteButtonClicked(object sender, EventArgs e)
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
            var result = await MaterialDialog.Instance.ConfirmAsync(GlobalConstants.LangGUI.GetValueOrDefault("Do you want to add the substance to favorites?", "Do you want to add the substance to favorites?"),
                GlobalConstants.LangGUI.GetValueOrDefault("Add to Favorite", "Add to Favorite"), GlobalConstants.LangGUI.GetValueOrDefault("OK", "OK"),
                GlobalConstants.LangGUI.GetValueOrDefault("Cancel", "Cancel"), alertDialogConfiguration);

            if (result ?? false)
            {
                note.Isfavorite = true;
                await App.Database.UpdateNoteAsync(note);
                await App.Database.SaveFavoritesAsync(new Favorite() { SubstanceID = note.SubstanceID, UserName = App.Userdata.userName });
            }
            else return;
        }

        public async void InfoButtonClicked(object sender, EventArgs e)
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
            await MaterialDialog.Instance.ConfirmAsync(GlobalConstants.LangGUI.GetValueOrDefault("Group", "Group") + ":" + GlobalConstants.GroupTexts.GetValueOrDefault(note.GroupNumber, note.GroupName ?? "") + "\n" + GlobalConstants.LangGUI.GetValueOrDefault("Duration", "Duration") +
                ":" + note.Duration + "min", GlobalConstants.SubTexts.GetValueOrDefault(note.SubstanceID, note.Substance ?? "") + " " + GlobalConstants.LangGUI.GetValueOrDefault("Information", "Information"), GlobalConstants.LangGUI.GetValueOrDefault("OK", "OK"),"", alertDialogConfiguration);

            //await DisplayAlert(GlobalConstants.SubTexts.GetValueOrDefault(note.SubstanceID, note.Substance ?? "") + " " + GlobalConstants.LangGUI.GetValueOrDefault("Information", "Information"),
            //    GlobalConstants.LangGUI.GetValueOrDefault("Group", "Group") + ":" + GlobalConstants.GroupTexts.GetValueOrDefault(note.GroupNumber, note.GroupName ?? "") + "\n" + GlobalConstants.LangGUI.GetValueOrDefault("Duration", "Duration") +
            //    ":" + note.Duration + "min", GlobalConstants.LangGUI.GetValueOrDefault("OK", "OK"));
        }

        private void ImageButton_Clicked(object sender, EventArgs e)
        {
            vm.closeCommand.Execute(closeBtn.Source);
            App.Current.MainPage = new NavigationPage(new MainDashboardPage());
        }

        private void ImageButton2_Clicked(object sender, EventArgs e)
        {
            if (GlobalConstants.NewModuleCount > 0)
            {
                NewModuleCount.Text = "";
                Navigation.PushAsync(new ModuleViewAllPage(true));
            }
            else return;
        }
    }
}
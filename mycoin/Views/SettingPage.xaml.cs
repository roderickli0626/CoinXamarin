using mycoin.Extensions;
using mycoin.Models;
using mycoin.ViewModels;
using Plugin.BLE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace mycoin.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingPage : ContentPage
    {
        SettingPageViewModel vm;
        string coverSkinUrl = "";
        string? language = "";
        int languageNumber = 0;
        public SettingPage()
        {
            InitializeComponent();
            this.BindingContext = vm = new SettingPageViewModel();

            if (Device.RuntimePlatform == Device.Android) Padding = new Thickness(0, 10, 0, 0);
            BackgroundColor = Color.White;
            // Remove the Navigation bar form the top of the page 
            Xamarin.Forms.NavigationPage.SetHasNavigationBar(this, false);           
        }

        void OnImageButtonClicked(object sender, EventArgs e)
        {
            App.Current.MainPage = new NavigationPage(new MainDashboardPage());
        }

        private void Image_Focused(object sender, EventArgs e)
        {
            List<View> flexList = CoverSkin.Children.ToList();
            foreach (View flex in flexList)
            {
                FlexLayout flexLayout = (FlexLayout)flex;
                foreach(View img in flexLayout.Children.ToList())
                {
                    ImageButton imageButton = (ImageButton)img;
                    imageButton.BorderColor = Color.Transparent;
                }
            }
            ImageButton image = sender as ImageButton;
            image.BorderColor = Color.DeepSkyBlue;
            image.BorderWidth = 3;
            coverSkinUrl = image.Source.ToString().Substring(6);
        }

        private async void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var result = await App.Current.MainPage.DisplayAlert(GlobalConstants.LangGUI.GetValueOrDefault("Language Change", "Language Change"),
                GlobalConstants.LangGUI.GetValueOrDefault("Do you really want to change the language?", "Do you really want to change the language?"), GlobalConstants.LangGUI.GetValueOrDefault("OK", "OK"), GlobalConstants.LangGUI.GetValueOrDefault("Cancel", "Cancel"));
            if (result)
            {
                languageNumber = ((Language)e.SelectedItem).languageNumber;
                language = ((Language)e.SelectedItem).description;
            }
            else return;
        }

        private void ImageButton_Clicked(object sender, EventArgs e)
        {
            AppSettings appSettings = new AppSettings();
            appSettings.coverSkinDefault = coverSkinUrl;
            appSettings.language = language;
            App.Database.DeleteAllAppSettingAsync();
            App.Database.SaveSettingsAsync(appSettings);

            if (languageNumber == 0) App.Current.MainPage = new NavigationPage(new MainDashboardPage());
            else
            {
                GlobalConstants.LangGUI.Clear();
                GlobalConstants.SubTexts.Clear();
                GlobalConstants.GroupTexts.Clear();
                List<SubstanceText> subTextList = App.Database.GetSubstanceTextByLangAsync(languageNumber).Result;
                List<GroupText> groupTextList = App.Database.GetGroupTextByLanguageAsync(languageNumber).Result;
                List<LanguageGUI> langGUIList = App.Database.GetLanguageGUIByLanguageAsync(languageNumber).Result;
                foreach(SubstanceText substanceText in subTextList)
                {
                    GlobalConstants.SubTexts.Add(substanceText.SubstanceID ?? 0, substanceText.Description ?? "");
                }
                foreach (GroupText groupText in groupTextList)
                {
                    GlobalConstants.GroupTexts.Add(groupText.GroupNumber ?? 0, groupText.Description ?? "");
                }
                foreach (LanguageGUI langGUI in langGUIList)
                {
                    GlobalConstants.LangGUI.Add(langGUI.key ?? "", langGUI.content ?? "");
                }
                App.Current.MainPage = new NavigationPage(new MainDashboardPage());
            }
        }

        private void Slider_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            string value = (e.NewValue / 100).ToString();
            MessagingCenter.Send(EventArgs.Empty, "Brightness", value);
        }

        private void ImageButton1_Clicked(object sender, EventArgs e)
        {
            App.Current.MainPage = new NavigationPage(new MainDashboardPage());
        }
    }
}
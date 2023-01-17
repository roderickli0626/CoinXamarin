using mycoin.Extensions;
using mycoin.Models;
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
    public partial class UserInfoPage : ContentPage
    {
        public UserInfoPage()
        {
            InitializeComponent();
            InitUserInfo();

            if (Device.RuntimePlatform == Device.Android) Padding = new Thickness(0, 10, 0, 0);
            BackgroundColor = Color.White;
            // Remove the Navigation bar form the top of the page 
            Xamarin.Forms.NavigationPage.SetHasNavigationBar(this, false);
        }

        void InitUserInfo()
        {
            UserInfoTitle.Text = GlobalConstants.LangGUI.GetValueOrDefault("User Information", "User Information");
            UpdateButton.Text = GlobalConstants.LangGUI.GetValueOrDefault("Update", "Update");
            DeleteButton.Text = GlobalConstants.LangGUI.GetValueOrDefault("Delete Account", "Delete Account");
            PasswordButton.Text = GlobalConstants.LangGUI.GetValueOrDefault("Password Change", "Password Change");
            userName.Text = App.Userdata.userName;
            Name.Text = Name.Text + App.Userdata.userName;
            DeviceNumber.Text = DeviceNumber.Text + App.Userdata.devicenum;
        }

        void OnImageButtonClicked(object sender, EventArgs e)
        {
            var flyoutPage = new MainDashboardPage();
            flyoutPage.Detail = new NavigationPage(new UserInfoPage());
            App.Current.MainPage = flyoutPage;
            flyoutPage.IsPresented = true;

            //App.Current.MainPage = new NavigationPage(new MainDashboardPage());
        }

        async void BtnDeleteAccountClicked(object sender, EventArgs e)
        {
            var result = await App.Current.MainPage.DisplayAlert(GlobalConstants.LangGUI.GetValueOrDefault("Delete Account", "Delete Account"),
                GlobalConstants.LangGUI.GetValueOrDefault("Do you really want to delete account?", "Do you really want to delete account?"), GlobalConstants.LangGUI.GetValueOrDefault("OK", "OK"), GlobalConstants.LangGUI.GetValueOrDefault("Cancel", "Cancel"));
            if (result)
            {
                QuestionOption option = App.Database.GetSelectedQuestionsByUserAsync(App.Userdata.userid).Result;
                if (option != null)
                {
                    await App.Database.DeleteSelectedQuestionsByUserAsync(option);
                }

                Userdata savedUserdata = App.Database.GetUserdataAsync().Result;
                if (savedUserdata == null) return;
                savedUserdata.isActive = false;
                await App.Database.UpdateUserdataAsync(savedUserdata);
            }
            else return;
        }

        private void ImageButton_Clicked(object sender, EventArgs e)
        {
            App.Current.MainPage = new NavigationPage(new MainDashboardPage());
        }

    }
}
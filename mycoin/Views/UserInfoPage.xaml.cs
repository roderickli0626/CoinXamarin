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
            userName.Text = App.Userdata.userName;
            Name.Text = Name.Text + App.Userdata.userName;
            DeviceNumber.Text = DeviceNumber.Text + App.Userdata.devicenum;
        }

        void OnImageButtonClicked(object sender, EventArgs e)
        {
            App.Current.MainPage = new NavigationPage(new MainDashboardPage());
        }

        void BtnDeleteAccountClicked(object sender, EventArgs e)
        {
            Userdata savedUserdata = App.Database.GetUserdataAsync().Result;
            if (savedUserdata == null) return;
            savedUserdata.isActive = false;
            App.Database.UpdateUserdataAsync(savedUserdata);
        }
    }
}
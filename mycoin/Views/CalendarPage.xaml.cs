using mycoin.Models;
using mycoin.ViewModels;
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
    public partial class CalendarPage : ContentPage
    {
        CalendarPageViewModel vm;
        public CalendarPage()
        {
            InitializeComponent();
            this.BindingContext = vm = new CalendarPageViewModel();


            if (Device.RuntimePlatform == Device.Android) Padding = new Thickness(0, 10, 0, 0);
            BackgroundColor = Color.White;
            // Remove the Navigation bar form the top of the page 
            Xamarin.Forms.NavigationPage.SetHasNavigationBar(this, false);
        }

        void OnImageButtonClicked(object sender, EventArgs e)
        {
            MessagingCenter.Send(EventArgs.Empty, "OpenMenu", "CalendarPage");
        }

        public async void DeleteCalendarClicked(object sender, EventArgs e)
        {
            ImageButton btn = sender as ImageButton;
            EventModel eventModel = btn.BindingContext as EventModel;
            if (eventModel == null) return;

            var result = await DisplayAlert("Delete", "Really Delete " + eventModel.Name + "?", "OK", "Cancel");
            if (result)
            {
                await App.Database.DeleteCalendarAsync(App.Database.GetCalendarAsync(eventModel.ID).Result);
                App.Current.MainPage = new NavigationPage(new CalendarPage());
            }
            else return;
        }

    }
}
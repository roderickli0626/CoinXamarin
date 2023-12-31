﻿using mycoin.DependencyServices;
using mycoin.Extensions;
using mycoin.Models;
using mycoin.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XF.Material.Forms.Resources;
using XF.Material.Forms.UI.Dialogs.Configurations;
using XF.Material.Forms.UI.Dialogs;

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

            //New Module Count
            NewModuleCount.Text = GlobalConstants.NewModuleCount > 0 ? GlobalConstants.NewModuleCount.ToString() : "";
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
            var result = await MaterialDialog.Instance.ConfirmAsync(GlobalConstants.LangGUI.GetValueOrDefault("Really Delete", "Really Delete") + " " + eventModel.Name + "?",
                GlobalConstants.LangGUI.GetValueOrDefault("Delete", "Delete"), GlobalConstants.LangGUI.GetValueOrDefault("OK", "OK"), GlobalConstants.LangGUI.GetValueOrDefault("Cancel", "Cancel"), alertDialogConfiguration);

            //var result = await DisplayAlert(GlobalConstants.LangGUI.GetValueOrDefault("Delete", "Delete"), GlobalConstants.LangGUI.GetValueOrDefault("Really Delete", "Really Delete") + 
            //    " " + eventModel.Name + "?", GlobalConstants.LangGUI.GetValueOrDefault("OK", "OK"), GlobalConstants.LangGUI.GetValueOrDefault("Cancel", "Cancel"));
            if (result ?? false)
            {
                await App.Database.DeleteCalendarAsync(App.Database.GetCalendarAsync(eventModel.ID).Result);
                //Remove from notifications
                DependencyService.Get<ILocalNotificationService>().Cancel(eventModel.ID);

                App.Current.MainPage = new NavigationPage(new CalendarPage());
            }
            else return;
        }

        private void ImageButton_Clicked(object sender, EventArgs e)
        {
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
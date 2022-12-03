﻿using mycoin.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FFImageLoading.Forms;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using mycoin.Models;

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
        }

        void OnImageButtonClicked(object sender, EventArgs e)
        {
            if (note.Isfavorite) App.Current.MainPage = new NavigationPage(new MainDashboardPage());
            else App.Current.MainPage = new NavigationPage(new MainDashboardPage1());
        }

        public void FavoriteButtonClicked(object sender, EventArgs e)
        {
            note.Isfavorite = true;
            App.Database.UpdateNoteAsync(note);
        }

        public async void InfoButtonClicked(object sender, EventArgs e)
        {
            await DisplayAlert(note.Substance + " Information", "Group:" + note.GroupName + "\nDuration:" + note.Duration + "min", "OK");
        }
    }
}
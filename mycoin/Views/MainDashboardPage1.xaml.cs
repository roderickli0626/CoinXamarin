using mycoin.Models;
using mycoin.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xam.Plugin;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace mycoin.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainDashboardPage1 : ContentPage
    {
        MainDashboardPage1ViewModel vm;
        public PopupMenu Popup;
        private int id;
        public MainDashboardPage1()
        {
            InitializeComponent();
            this.BindingContext = vm = new MainDashboardPage1ViewModel();

            Popup = new PopupMenu()
            {
                ItemsSource = new List<string>() { "Add to Favorite"},
            };

            Popup.OnItemSelected += ItemSelectedDelegate;

            MyTabs.SelectedIndex = 2;

            if (Device.RuntimePlatform == Device.Android) Padding = new Thickness(0, 10, 0, 0);
            BackgroundColor = Color.White;
            // Remove the Navigation bar form the top of the page 
            Xamarin.Forms.NavigationPage.SetHasNavigationBar(this, false);
        }

        void OnImageButtonClicked(object sender, EventArgs e)
        {
            MessagingCenter.Send(EventArgs.Empty, "OpenMenu");
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

        protected void ItemSelectedDelegate(string item)
        {
            if (item == "Add to Favorite") {
                if (id == 0) return;
                Note note = App.Database.GetNoteAsync(id).Result;
                note.Isfavorite = true;
                App.Database.UpdateNoteAsync(note);
            }
        }

        private void BacktoPrevious(object sender, EventArgs e)
        {
            App.Current.MainPage = new NavigationPage(new MainDashboardPage());
        }
    }
}
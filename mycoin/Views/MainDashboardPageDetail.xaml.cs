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
    public partial class MainDashboardPageDetail : ContentPage
    {
        MainDashboardPageViewModel vm;
        public MainDashboardPageDetail()
        {
            InitializeComponent();
            this.BindingContext = vm = new MainDashboardPageViewModel();

            List<Note> notes = App.Database.GetNotesAsync().Result.Where(n => n.Isfavorite).ToList();
            if (notes.Count > 0)
            {
                favorCol.ItemsLayout = LinearItemsLayout.Horizontal;
            } else
            {
                favorCol.ItemsLayout = LinearItemsLayout.Vertical;
            }

            //On<iOS>().SetUseSafeArea(true);

            if (Device.RuntimePlatform == Device.Android) Padding = new Thickness(0, 10, 0, 0);
            BackgroundColor = Color.White;
            // Remove the Navigation bar form the top of the page 
            Xamarin.Forms.NavigationPage.SetHasNavigationBar(this, false);
        }

        void OnImageButtonClicked(object sender, EventArgs e)
        {
            MessagingCenter.Send(EventArgs.Empty, "OpenMenu", "MainDashboardPageDetail");
        }

        private void ImageButton_Clicked(object sender, EventArgs e)
        {
            App.Current.MainPage = new NavigationPage(new MainDashboardPage1());
        }

        private void FavoriteImageButton_Clicked(object sender, EventArgs e)
        {
            ImageButton btn = sender as ImageButton;
            MySubstance substance = btn.BindingContext as MySubstance;
            if (substance == null) return;
            else if (substance.ID == 0) App.Current.MainPage = new NavigationPage(new MainDashboardPage1());
            else App.Current.MainPage = new NavigationPage(new PlayPage(substance.ID));
        }

        private void FavoriteImageButton_DoubleClicked(object sender, EventArgs e)
        {
            Image image = sender as Image;
            MySubstance substance = image.BindingContext as MySubstance;
            if (substance == null) return;
            else if (substance.ID == 0) return;
            else App.Current.MainPage = new NavigationPage(new PlayPage(substance.ID));
        }

        private void FavoriteImageButton_SingleClicked(object sender, EventArgs e)
        {
            Image btn = sender as Image;
            MySubstance substance = btn.BindingContext as MySubstance;
            if (substance == null) return;
            else if (substance.ID == 0) App.Current.MainPage = new NavigationPage(new MainDashboardPage1());
            else return;
        }

        private void titleLabelDoubleClicked(object sender, EventArgs e)
        {
            Note note = App.Database.GetNotesAsync().Result.Where(n => n.Isfavorite && n.PlayDateTime == null).FirstOrDefault();
            if (note == null) return;
            App.Current.MainPage = new NavigationPage(new PlayPage(note.ID));
        }
    }
}
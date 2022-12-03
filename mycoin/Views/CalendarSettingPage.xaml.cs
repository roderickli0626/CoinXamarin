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
using static Xam.Plugin.PopupMenu;

namespace mycoin.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CalendarSettingPage : ContentPage
    {
        CalendarSettingPageViewModel vm;
        public PopupMenu Popup;
        private int id;


        public CalendarSettingPage(int id, DateTime? selectedDate)
        {
            InitializeComponent();
            this.BindingContext = vm = new CalendarSettingPageViewModel(id, selectedDate);

            //Popup = new PopupMenu()
            //{
            //    ItemsSource = new List<string>() { "Add to Calendar" },
            //};
            //Popup.OnItemSelected += ItemSelectedDelegate;

            if (Device.RuntimePlatform == Device.Android) Padding = new Thickness(0, 10, 0, 0);
            BackgroundColor = Color.White;
            // Remove the Navigation bar form the top of the page 
            Xamarin.Forms.NavigationPage.SetHasNavigationBar(this, false);
        }

        void OnImageButtonClicked(object sender, EventArgs e)
        {
            App.Current.MainPage = new NavigationPage(new CalendarPage());
        }

        void StartTimeChanged(object sender, EventArgs e)
        {
            st.Text = stv.SelectedItem + " : " + stv.SecondarySelectedItem.ToString();
        }
        void DurationTimeChanged(object sender, EventArgs e)
        {
            dt.Text = dtv.SelectedItem + "h " + dtv.SecondarySelectedItem + "min";
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

        //void ShowPopup_Clicked(object sender, EventArgs e)
        //{
        //    ImageButton btn = sender as ImageButton;
        //    MySubstanceViewModel substance = btn.BindingContext as MySubstanceViewModel;
        //    if (substance == null) return;

        //    id = substance.ID;

        //    Popup.ShowPopup(sender as View);
        //}
        //protected void ItemSelectedDelegate(string item)
        //{
        //    if (item == "Add to Calendar")
        //    {
        //        if (id == 0) return;
        //        Note note = App.Database.GetNoteAsync(id).Result;
        //        note.Isfavorite = true;
        //        App.Database.UpdateNoteAsync(note);
        //        App.Current.MainPage = new NavigationPage(new MainDashboardPage1("allTab"));
        //    }
        //    else return;
        //}
    }
}
using mycoin.DependencyServices;
using mycoin.Extensions;
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
        public string repeatDates = "";
        public ViewCell lastCell;



        public CalendarSettingPage(int id, DateTime? selectedDate)
        {
            InitializeComponent();
            this.BindingContext = vm = new CalendarSettingPageViewModel(id, selectedDate);

            repeatDatePicker.DateSelected += (sender, e) =>
            {
                repeatTimePicker.Focus();

                //ImageButton imgbtn = new ImageButton { Source = "icons8_minus.png", HeightRequest = 20, WidthRequest = 20, Margin = new Thickness(10,4,0,0) };
                //imgbtn.Clicked += repeatDelClicked;
                //Repeat.Children.Add(new FlexLayout()
                //{
                //    JustifyContent = FlexJustify.Center,
                //    Children =
                //    {
                //        new Label() {Text = repeatDatePicker.Date.ToString("MM/dd/yyyy"), FontSize = 20},
                //        imgbtn
                //    }
                //});

                //repeatDates += repeatDatePicker.Date.ToString("MM/dd/yyyy") + " ";
            };
            repeatTimePicker.Unfocused += (sender, e) =>
            {
                ImageButton imgbtn = new ImageButton { Source = "icons8_minus.png", HeightRequest = 20, WidthRequest = 20, Margin = new Thickness(10, 4, 0, 0) };
                imgbtn.Clicked += repeatDelClicked;
                Repeat.Children.Add(new FlexLayout()
                {
                    JustifyContent = FlexJustify.Center,
                    Children =
                    {
                        new Label() {Text = repeatDatePicker.Date.ToString("MM/dd/yyyy") + " " + repeatTimePicker.Time, FontSize = 20},
                        imgbtn
                    }
                });

                repeatDates += repeatDatePicker.Date.ToString("MM/dd/yyyy") + " " + repeatTimePicker.Time + ",";
            };
            MessagingCenter.Subscribe<CalendarSettingPageViewModel>(this, "Save Repeats", (sender) =>
            {
                MessagingCenter.Send(EventArgs.Empty, "repeatDates", repeatDates);
            });

            if (Device.RuntimePlatform == Device.Android) Padding = new Thickness(0, 10, 0, 0);
            BackgroundColor = Color.White;
            // Remove the Navigation bar form the top of the page 
            Xamarin.Forms.NavigationPage.SetHasNavigationBar(this, false);

            //New Module Count
            NewModuleCount.Text = GlobalConstants.NewModuleCount > 0 ? GlobalConstants.NewModuleCount.ToString() : "";
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

        void repeatAddClicked(object sender, EventArgs e)
        {
            repeatDatePicker.Focus();
        }
        void repeatDelClicked(object sender, EventArgs e)
        {
            ImageButton imageButton = sender as ImageButton;
            FlexLayout fl = (FlexLayout)imageButton.Parent;
            string delSDate = ((Label)fl.Children.ElementAt(0)).Text;
            fl.IsVisible = false;

            repeatDates = repeatDates.Replace(delSDate + ",", "");
        }

        void repeatAllDelClicked (object sender, EventArgs e)
        {
            Repeat.Children.Clear();
            repeatDates = "";
        }

        //private void RadioButton_CheckedChanged(object sender, CheckedChangedEventArgs e)
        //{
        //    RadioButton rb = sender as RadioButton;
        //    if (!rb.IsChecked) return;
        //    var selectedR = rb.Value;
        //    switch (selectedR)
        //    {
        //        case "0": repeatDates.IsVisible = false; break;
        //        case "1": {
        //                repeatDates.IsVisible = true;
        //                repeatDatePicker1.IsVisible = true;
        //                repeatDatePicker2.IsVisible = false;
        //                repeatDatePicker3.IsVisible = false;
        //            } break;
        //        case "2":
        //            {
        //                repeatDates.IsVisible = true;
        //                repeatDatePicker1.IsVisible = true;
        //                repeatDatePicker2.IsVisible = true;
        //                repeatDatePicker3.IsVisible = false;
        //            } break;
        //        default:
        //            {
        //                repeatDates.IsVisible = true;
        //                repeatDatePicker1.IsVisible = true;
        //                repeatDatePicker2.IsVisible = true;
        //                repeatDatePicker3.IsVisible = true;
        //            } break;
        //    }

        //}

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

        private void ImageButton_Clicked(object sender, EventArgs e)
        {
            App.Current.MainPage = new NavigationPage(new MainDashboardPage());
        }

        private void ViewCell_Tapped(object sender, EventArgs e)
        {
            if (lastCell != null)
                lastCell.View.BackgroundColor = Color.Transparent;
            var viewCell = (ViewCell)sender;
            if (viewCell.View != null)
            {
                viewCell.View.BackgroundColor = Color.FromHex("#B16BBF");
                lastCell = viewCell;
            }
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
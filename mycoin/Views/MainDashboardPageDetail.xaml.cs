using mycoin.DependencyServices;
using mycoin.Extensions;
using mycoin.Models;
using mycoin.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            }
            else
            {
                favorCol.ItemsLayout = LinearItemsLayout.Vertical;
            }

            //On<iOS>().SetUseSafeArea(true);

            if (Device.RuntimePlatform == Device.Android) Padding = new Thickness(0, 10, 0, 0);
            BackgroundColor = Color.White;
            // Remove the Navigation bar form the top of the page 
            Xamarin.Forms.NavigationPage.SetHasNavigationBar(this, false);

            //New Module Count
            NewModuleCount.Text = GlobalConstants.NewModuleCount > 0 ? GlobalConstants.NewModuleCount.ToString() : "";

            MessagingCenter.Subscribe<MainDashboardPageViewModel>(this, "Favor End", (sender) =>
            {
                //Continue Favorite Music Automatically
                Note note1 = App.Database.GetNotesAsync().Result.Where(n => n.Isfavorite && n.PlayDateTime == null).FirstOrDefault();
                if (note1 == null) return;

                vm.MySubstances.Clear();
                List<Note> noteList = App.Database.GetNotesAsync().Result.Where(n => n.Isfavorite).ToList();
                foreach (Note note in noteList)
                {
                    vm.MySubstances.Add(new MySubstance
                    {
                        ID = note.ID,
                        SubstanceImageUrl = note1.ID == note.ID ? "ic_biapp_icon_pause.xml" : "ic_biapp_icon_favorit.xml",
                        SubstanceName = GlobalConstants.SubTexts.GetValueOrDefault(note.SubstanceID, note.Substance ?? "No SubstanceName"),
                        Duration = note.Duration,
                        DurationTimeFormat = note.Duration > 59 ? (note.Duration / 60) + "h " + (note.Duration % 60) + "min" : (note.Duration % 60) + "min",
                        favoriteExtraIconVisible = true,
                        SubstanceID = note.SubstanceID,
                    });

                }
                vm.MySubstances.Add(new MySubstance
                {
                    ID = 0,
                    //SubstanceImageUrl = "ic_biapp_icon_pause.xml",
                    SubstanceImageUrl = "ic_biapp_icon_favorit_hinzufuegen.xml",
                    SubstanceName = GlobalConstants.LangGUI.GetValueOrDefault("Favorites Add", "Favorites Add"),
                    DurationTimeFormat = "",
                    favoriteExtraIconVisible = false
                });

                vm.playCommand.Execute(new MySubstance
                {
                    ID = note1.ID,
                    SubstanceImageUrl = "ic_biapp_icon_pause.xml",
                    SubstanceName = GlobalConstants.SubTexts.GetValueOrDefault(note1.SubstanceID, note1.Substance ?? "No SubstanceName"),
                    Duration = note1.Duration,
                    DurationTimeFormat = note1.Duration > 59 ? (note1.Duration / 60) + "h " + (note1.Duration % 60) + "min" : (note1.Duration % 60) + "min",
                    favoriteExtraIconVisible = true,
                    SubstanceID = note1.SubstanceID,
                });
            });
        }

        void OnImageButtonClicked(object sender, EventArgs e)
        {
            MessagingCenter.Send(EventArgs.Empty, "OpenMenu", "MainDashboardPageDetail");
        }

        private void ImageButton_Clicked(object sender, EventArgs e)
        {
            vm.closeCommand.Execute(this);
            App.Current.MainPage = new NavigationPage(new MainDashboardPage1());
        }

        private void CalendarButton_Clicked(object sender, EventArgs e)
        {
            vm.closeCommand.Execute(this);
            App.Current.MainPage = new NavigationPage(new CalendarPage());
        }

        private void ShopButton_Click(object sender, EventArgs e)
        {
            //Move to module page
            GlobalConstants.NewModuleCount = 0;
            App.Database.DeleteAllConstantsAsync();
            vm.closeCommand.Execute(this);
            App.Current.MainPage = new NavigationPage(new ModulePage());
        }

        private void FavoriteImageButton_Clicked(object sender, EventArgs e)
        {
            Image image = sender as Image;
            MySubstance substance = image.BindingContext as MySubstance;
            if (substance.SubstanceImageUrl == "ic_biapp_icon_pause.xml")
            {
                vm.MySubstances.Clear();
                List<Note> noteList = App.Database.GetNotesAsync().Result.Where(n => n.Isfavorite).ToList();
                foreach (Note note in noteList)
                {
                    vm.MySubstances.Add(new MySubstance
                    {
                        ID = note.ID,
                        SubstanceImageUrl = "ic_biapp_icon_favorit.xml",
                        SubstanceName = GlobalConstants.SubTexts.GetValueOrDefault(note.SubstanceID, note.Substance ?? "No SubstanceName"),
                        Duration = note.Duration,
                        DurationTimeFormat = note.Duration > 59 ? (note.Duration / 60) + "h " + (note.Duration % 60) + "min" : (note.Duration % 60) + "min",
                        favoriteExtraIconVisible = true,
                        SubstanceID = note.SubstanceID,
                    });

                }
                vm.MySubstances.Add(new MySubstance
                {
                    ID = 0,
                    //SubstanceImageUrl = "ic_biapp_icon_pause.xml",
                    SubstanceImageUrl = "ic_biapp_icon_favorit_hinzufuegen.xml",
                    SubstanceName = GlobalConstants.LangGUI.GetValueOrDefault("Favorites Add", "Favorites Add"),
                    DurationTimeFormat = "",
                    favoriteExtraIconVisible = false
                });

                //image.Source = ImageSource.FromFile("ic_biapp_icon_favorit.xml");
                //substance.SubstanceImageUrl = "ic_biapp_icon_favorit.xml";
                vm.closeCommand.Execute(closeBtn.Source);
                return;
            }
            if (substance == null) return;
            else if (substance.ID == 0)
            {
                vm.closeCommand.Execute(this);
                App.Current.MainPage = new NavigationPage(new MainDashboardPage1());
            }
            else return;
            //else App.Current.MainPage = new NavigationPage(new PlayPage(substance.ID));
        }

        private void FavoriteImageButton_DoubleClicked(object sender, EventArgs e)
        {
            Image img = sender as Image;
            MySubstance substance = img.BindingContext as MySubstance;
            if (substance == null || substance.SubstanceImageUrl == "ic_biapp_icon_favorit_hinzufuegen.xml") return;
            if (substance.SubstanceImageUrl == "ic_biapp_icon_favorit.xml")
            {
                //var parent = (CollectionView)img.Parent.Parent;
                //vm.MySubstances.Where(x => x.ID != 0).ToList().ForEach(x => x.SubstanceImageUrl = "ic_biapp_icon_favorit.xml");
                //parent.ItemsSource = vm.MySubstances.ToList();

                vm.MySubstances.Clear();
                List<Note> noteList = App.Database.GetNotesAsync().Result.Where(n => n.Isfavorite).ToList();
                foreach (Note note in noteList)
                {
                    vm.MySubstances.Add(new MySubstance
                    {
                        ID = note.ID,
                        SubstanceImageUrl = substance.ID == note.ID ? "ic_biapp_icon_pause.xml" : "ic_biapp_icon_favorit.xml",
                        SubstanceName = GlobalConstants.SubTexts.GetValueOrDefault(note.SubstanceID, note.Substance ?? "No SubstanceName"),
                        Duration = note.Duration,
                        DurationTimeFormat = note.Duration > 59 ? (note.Duration / 60) + "h " + (note.Duration % 60) + "min" : (note.Duration % 60) + "min",
                        favoriteExtraIconVisible = true,
                        SubstanceID = note.SubstanceID,
                    });

                }
                vm.MySubstances.Add(new MySubstance
                {
                    ID = 0,
                    //SubstanceImageUrl = "ic_biapp_icon_pause.xml",
                    SubstanceImageUrl = "ic_biapp_icon_favorit_hinzufuegen.xml",
                    SubstanceName = GlobalConstants.LangGUI.GetValueOrDefault("Favorites Add", "Favorites Add"),
                    DurationTimeFormat = "",
                    favoriteExtraIconVisible = false
                });

                //img.Source = ImageSource.FromFile("ic_biapp_icon_pause.xml");
                //substance.SubstanceImageUrl = "ic_biapp_icon_pause.xml";
            }
            else
            {
                vm.MySubstances.Clear();
                List<Note> noteList = App.Database.GetNotesAsync().Result.Where(n => n.Isfavorite).ToList();
                foreach (Note note in noteList)
                {
                    vm.MySubstances.Add(new MySubstance
                    {
                        ID = note.ID,
                        SubstanceImageUrl = "ic_biapp_icon_favorit.xml",
                        SubstanceName = GlobalConstants.SubTexts.GetValueOrDefault(note.SubstanceID, note.Substance ?? "No SubstanceName"),
                        Duration = note.Duration,
                        DurationTimeFormat = note.Duration > 59 ? (note.Duration / 60) + "h " + (note.Duration % 60) + "min" : (note.Duration % 60) + "min",
                        favoriteExtraIconVisible = true,
                        SubstanceID = note.SubstanceID,
                    });

                }
                vm.MySubstances.Add(new MySubstance
                {
                    ID = 0,
                    //SubstanceImageUrl = "ic_biapp_icon_pause.xml",
                    SubstanceImageUrl = "ic_biapp_icon_favorit_hinzufuegen.xml",
                    SubstanceName = GlobalConstants.LangGUI.GetValueOrDefault("Favorites Add", "Favorites Add"),
                    DurationTimeFormat = "",
                    favoriteExtraIconVisible = false
                });

                //img.Source = ImageSource.FromFile("ic_biapp_icon_favorit.xml");
                //substance.SubstanceImageUrl = "ic_biapp_icon_favorit.xml";
                vm.closeCommand.Execute(closeBtn.Source);
            }
        }

        private void FavoriteLabel_Clicked(object sender, EventArgs e)
        {
            Label label = sender as Label;
            MySubstance substance = label.BindingContext as MySubstance;
            if (substance.SubstanceImageUrl == "ic_biapp_icon_pause.xml")
            {
                vm.MySubstances.Clear();
                List<Note> noteList = App.Database.GetNotesAsync().Result.Where(n => n.Isfavorite).ToList();
                foreach (Note note in noteList)
                {
                    vm.MySubstances.Add(new MySubstance
                    {
                        ID = note.ID,
                        SubstanceImageUrl = "ic_biapp_icon_favorit.xml",
                        SubstanceName = GlobalConstants.SubTexts.GetValueOrDefault(note.SubstanceID, note.Substance ?? "No SubstanceName"),
                        Duration = note.Duration,
                        DurationTimeFormat = note.Duration > 59 ? (note.Duration / 60) + "h " + (note.Duration % 60) + "min" : (note.Duration % 60) + "min",
                        favoriteExtraIconVisible = true,
                        SubstanceID = note.SubstanceID,
                    });

                }
                vm.MySubstances.Add(new MySubstance
                {
                    ID = 0,
                    //SubstanceImageUrl = "ic_biapp_icon_pause.xml",
                    SubstanceImageUrl = "ic_biapp_icon_favorit_hinzufuegen.xml",
                    SubstanceName = GlobalConstants.LangGUI.GetValueOrDefault("Favorites Add", "Favorites Add"),
                    DurationTimeFormat = "",
                    favoriteExtraIconVisible = false
                });

                //((Image)((StackLayout)label.Parent).Children.ElementAt(0)).Source = ImageSource.FromFile("ic_biapp_icon_favorit.xml");
                //substance.SubstanceImageUrl = "ic_biapp_icon_favorit.xml";
                vm.closeCommand.Execute(closeBtn.Source);
                return;
            }
            if (substance == null) return;
            else if (substance.ID == 0)
            {
                vm.closeCommand.Execute(this);
                App.Current.MainPage = new NavigationPage(new MainDashboardPage1());
            }
            else
            {
                vm.closeCommand.Execute(this);
                App.Current.MainPage = new NavigationPage(new PlayPage(substance.ID));
            }
        }

        //private void FavoriteImageButton_SingleClicked(object sender, EventArgs e)
        //{
        //    Image btn = sender as Image;
        //    MySubstance substance = btn.BindingContext as MySubstance;
        //    if (substance == null) return;
        //    else if (substance.ID == 0) App.Current.MainPage = new NavigationPage(new MainDashboardPage1());
        //    else return;
        //}

        private void titleLabelDoubleClicked(object sender, EventArgs e)
        {
            Note note = App.Database.GetNotesAsync().Result.Where(n => n.Isfavorite && n.PlayDateTime == null).FirstOrDefault();
            if (note == null) return;
            vm.closeCommand.Execute(this);
            App.Current.MainPage = new NavigationPage(new PlayPage(note.ID));
        }

        private void ImageButton1_Clicked(object sender, EventArgs e)
        {
            vm.closeCommand.Execute(this);
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
using System;
using System.IO;
using mycoin.Data;
using mycoin.Models;
using mycoin.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace mycoin
{
    public partial class App : Application
    {
        static Userdata userdata;
        static NoteDatabase database;
        public static Userdata Userdata
        { get; set; }

        // Create the database connection as a singleton.
        public static NoteDatabase Database
        {
            get
            {
                if (database == null)
                {
                    database = new NoteDatabase(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Notes.db3"));
                }
                return database;
            }
        }

        public App()
        {
            InitializeComponent();
            XF.Material.Forms.Material.Init(this);

            //MainPage = new NavigationPage(new LoginPage());

            MessagingCenter.Subscribe<EventArgs, string>(this, "OpenMenu", (sender, args) =>
            {
                Page detailPage = null;
                if (args == "MainDashboardPageDetail") detailPage = new MainDashboardPageDetail();
                else if (args == "CalendarPage") detailPage = new CalendarPage();
                else detailPage = new MainDashboardPage1("normal");
                var flyoutPage = new MainDashboardPage();
                flyoutPage.Detail = new NavigationPage(detailPage);
                MainPage = flyoutPage;
                flyoutPage.IsPresented = true;
            });

        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}

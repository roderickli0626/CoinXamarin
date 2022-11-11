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
    public partial class MainDashboardPageDetail : ContentPage
    {
        public string? firstProgram;
        public string? currentDate { get; set; }
        public MainDashboardPageDetail()
        {
            InitializeComponent();
            LoadInfo();
        }

        async void LoadInfo()
        {
            currentDate = DateTime.Now.ToString();
            //Note note = await App.Database.GetNotesAsync().Result.FirstOrDefault();
            //firstProgram = note.Substance;
        }

        
    }
}
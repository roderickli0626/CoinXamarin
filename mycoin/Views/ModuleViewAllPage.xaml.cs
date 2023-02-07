using mycoin.Extensions;
using mycoin.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace mycoin.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ModuleViewAllPage : BasePage
    {
        public ModuleViewAllPage()
        {
            InitializeComponent();
            LoadModuleDB();

            Title.Text = GlobalConstants.LangGUI.GetValueOrDefault("NADA", "NADA");
            subTitle.Text = GlobalConstants.LangGUI.GetValueOrDefault("Join us in 30 Day Sound Healing Journey, and immerse yourself in music based on mystical frequencies to heal mind, body and soul.",
                "Join us in 30 Day Sound Healing Journey, and immerse yourself in music based on mystical frequencies to heal mind, body and soul.");
        }

        async private void LoadModuleDB()
        {
            try
            {
                List<Module> AllModules = await App.Database.GetAllModulesAsync();
                List<ModuleRes> AllResModules = new List<ModuleRes>();
                foreach (var module in AllModules)
                {
                    ModuleRes moduleRes = new ModuleRes();
                    moduleRes.ModuleID = module.ModuleID;
                    moduleRes.Color = module.Color;
                    moduleRes.Description = module.Description;
                    moduleRes.File = module.File;
                    moduleRes.ModuleID = module.ModuleID;
                    moduleRes.NameModule = module.NameModule;
                    moduleRes.ProductNumber = module.ProductNumber;
                    moduleRes.Price = module.Price;
                    moduleRes.Location = module.Location;

                    byte[] Base64Stream = Convert.FromBase64String(module.File);
                    moduleRes.imageSource = ImageSource.FromStream(() => new MemoryStream(Base64Stream));

                    AllResModules.Add(moduleRes);
                }

                ModuleList.ItemsSource = AllResModules.ToArray();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }

        }

        private void PreviousButton_Clicked(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }

        private async void ImageButton_Clicked(object sender, EventArgs e)
        {
            ImageButton imgBtn = sender as ImageButton;
            ModuleRes module = imgBtn.BindingContext as ModuleRes;
            if (module == null) return;
            await Launcher.OpenAsync(new Uri(module.Location));
            //await Launcher.OpenAsync(new Uri("http://www.google.com"));
        }
    }
}
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
using XF.Material.Forms.Resources;
using XF.Material.Forms.UI.Dialogs.Configurations;
using XF.Material.Forms.UI.Dialogs;

namespace mycoin.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ModuleViewAllPage : BasePage
    {
        public ModuleViewAllPage(bool isForNewModules = false)
        {
            InitializeComponent();
            LoadModuleDB(isForNewModules);

            if (isForNewModules)
            {
                subTitle.Text = "These are new Modules.";
            }
            else
            {
                subTitle.Text = GlobalConstants.LangGUI.GetValueOrDefault("Join us in 30 Day Sound Healing Journey, and immerse yourself in music based on mystical frequencies to heal mind, body and soul.",
                "Join us in 30 Day Sound Healing Journey, and immerse yourself in music based on mystical frequencies to heal mind, body and soul.");
            }

            Title.Text = GlobalConstants.LangGUI.GetValueOrDefault("NADA", "NADA");
        }

        async private void LoadModuleDB(bool isForNew)
        {
            try
            {
                List<Module> AllModules = await App.Database.GetAllModulesAsync();
                if (isForNew)
                {
                    AllModules = AllModules.GetRange(AllModules.Count() - GlobalConstants.NewModuleCount, GlobalConstants.NewModuleCount);
                    GlobalConstants.NewModuleCount = 0;
                    await App.Database.DeleteAllConstantsAsync();
                }
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

                    try
                    {
                        byte[] Base64Stream = Convert.FromBase64String(module.File);
                        moduleRes.imageSource = ImageSource.FromStream(() => new MemoryStream(Base64Stream));
                    }
                    catch (Exception ex)
                    {
                        moduleRes.imageSource = null;
                    }

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
            //await Launcher.OpenAsync(new Uri(module.Location));
            if (Uri.IsWellFormedUriString(module.Location, UriKind.Absolute))
            {
                await Browser.OpenAsync(new Uri(module.Location));
            }
            else
            {
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
                var result = await MaterialDialog.Instance.ConfirmAsync(GlobalConstants.LangGUI.GetValueOrDefault("The link is not correct.", "The link is not correct."),
                GlobalConstants.LangGUI.GetValueOrDefault("Warning", "Warning"), GlobalConstants.LangGUI.GetValueOrDefault("OK", "OK"), "", alertDialogConfiguration);

            }
            //await Launcher.OpenAsync(new Uri("http://www.google.com"));
        }
    }
}
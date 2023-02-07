using mycoin.Extensions;
using mycoin.Models;
using System;
using System.Buffers.Text;
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
	public partial class ModulePage : BasePage
	{
		List<ModuleVertical> VerticalModuleList = new List<ModuleVertical>();
		public ModulePage ()
		{
			InitializeComponent ();
			LoadModuleDB();

			Title.Text = GlobalConstants.LangGUI.GetValueOrDefault("NADA", "NADA");
			subTitle.Text = GlobalConstants.LangGUI.GetValueOrDefault("30 Day Sound Healing Journey", "30 Day Sound Healing Journey");
			ViewAllBtn.Text = GlobalConstants.LangGUI.GetValueOrDefault("VIEW ALL", "VIEW ALL");
		}

		async private void LoadModuleDB()
		{
			try
			{
				List<Module> AllModules = await App.Database.GetAllModulesAsync();
				List<ModuleRes> AllResModules = new List<ModuleRes>();
				foreach (var module in AllModules)
				{
					ModuleRes moduleRes= new ModuleRes();
					moduleRes.ModuleID= module.ModuleID;
					moduleRes.Color= module.Color;
					moduleRes.Description= module.Description;
					moduleRes.File = module.File;
					moduleRes.ModuleID = module.ModuleID;
					moduleRes.NameModule = module.NameModule;
					moduleRes.ProductNumber= module.ProductNumber;
					moduleRes.Price = module.Price;
					moduleRes.Location= module.Location;

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
                
                while (AllResModules.Count() > 5)
				{
                    ModuleVertical vModule = new ModuleVertical();
					vModule.VerticalModules = AllResModules.GetRange(0, 5).ToArray();
                    AllResModules = AllResModules.GetRange(5, AllResModules.Count() - 5);
                    VerticalModuleList.Add(vModule);
                }

				VerticalModuleList.Add (new ModuleVertical()
				{
					VerticalModules = AllResModules.ToArray(),
				});


                ModuleList.ItemsSource = VerticalModuleList.ToArray();

            } 
			catch (Exception ex)
			{
                Console.WriteLine(ex.Message);
				return;
            }

		}

        private void PreviousButton_Clicked(object sender, EventArgs e)
        {
			App.Current.MainPage = new NavigationPage(new MainDashboardPage());
        }

        private void ViewAllButton_Clicked(object sender, EventArgs e)
        {
			Navigation.PushAsync(new ModuleViewAllPage());
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
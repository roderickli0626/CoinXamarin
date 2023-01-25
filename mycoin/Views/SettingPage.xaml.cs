using mycoin.Extensions;
using mycoin.Models;
using mycoin.ViewModels;
using Plugin.BLE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XF.Material.Forms.Resources;
using XF.Material.Forms.UI.Dialogs.Configurations;
using XF.Material.Forms.UI.Dialogs;
using Plugin.BLE.Abstractions.Contracts;
using XamarinEssentials = Xamarin.Essentials;
using Plugin.BLE.Abstractions;

namespace mycoin.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingPage : ContentPage
    {
        SettingPageViewModel vm;
        string coverSkinUrl = "";
        string? language = "";
        int languageNumber = 0;
        //---------For BLE--------//
        private readonly IAdapter _bluetoothAdapter;
        private List<IDevice> _gattDevices = new List<IDevice>();
        //---------For BLE--------//
        public SettingPage()
        {
            InitializeComponent();
            this.BindingContext = vm = new SettingPageViewModel();

            //---------For BLE---------//
            _bluetoothAdapter = CrossBluetoothLE.Current.Adapter;
            _bluetoothAdapter.DeviceDiscovered += (sender, foundBleDevice) =>
            {
                if (foundBleDevice.Device != null && !string.IsNullOrEmpty(foundBleDevice.Device.Name))
                    _gattDevices.Add(foundBleDevice.Device);
            };
            //---------For BLE--------//

            if (Device.RuntimePlatform == Device.Android) Padding = new Thickness(0, 10, 0, 0);
            BackgroundColor = Color.White;
            // Remove the Navigation bar form the top of the page 
            Xamarin.Forms.NavigationPage.SetHasNavigationBar(this, false);           
        }

        //----------------- For BLE ----------------//
        private async Task<bool> PermissionsGrantedAsync()
        {
            var locationPermissionStatus = await XamarinEssentials.Permissions.CheckStatusAsync<XamarinEssentials.Permissions.LocationAlways>();

            if (locationPermissionStatus != XamarinEssentials.PermissionStatus.Granted)
            {
                var status = await XamarinEssentials.Permissions.RequestAsync<XamarinEssentials.Permissions.LocationAlways>();
                return status == XamarinEssentials.PermissionStatus.Granted;
            }
            return true;
        }

        private async void ScanButton_Clicked(object sender, EventArgs e)
        {
            IsBusyIndicator.IsVisible = IsBusyIndicator.IsRunning = !(ScanButton.IsEnabled = false);
            foundBleDevicesListView.ItemsSource = null;

            if (!await PermissionsGrantedAsync())
            {
                await DisplayAlert("Permission required", "Application needs location permission", "OK");
                IsBusyIndicator.IsVisible = IsBusyIndicator.IsRunning = !(ScanButton.IsEnabled = true);
                return;
            }

            _gattDevices.Clear();

            foreach (var device in _bluetoothAdapter.ConnectedDevices)
                _gattDevices.Add(device);

            await _bluetoothAdapter.StartScanningForDevicesAsync();

            foundBleDevicesListView.ItemsSource = _gattDevices.ToArray();
            IsBusyIndicator.IsVisible = IsBusyIndicator.IsRunning = !(ScanButton.IsEnabled = true);
        }

        private async void FoundBluetoothDevicesListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            IsBusyIndicator.IsVisible = IsBusyIndicator.IsRunning = !(ScanButton.IsEnabled = false);
            IDevice selectedItem = e.Item as IDevice;

            if (selectedItem.State == DeviceState.Connected)
            {
                //await Navigation.PushAsync(new BluetoothDataPage(selectedItem));
            }
            else
            {
                try
                {
                    var connectParameters = new ConnectParameters(false, true);
                    await _bluetoothAdapter.ConnectToDeviceAsync(selectedItem, connectParameters);
                    //await Navigation.PushAsync(new BluetoothDataPage(selectedItem));
                }
                catch
                {
                    await DisplayAlert("Error connecting", $"Error connecting to BLE device: {selectedItem.Name ?? "N/A"}", "Retry");
                }
            }

            IsBusyIndicator.IsVisible = IsBusyIndicator.IsRunning = !(ScanButton.IsEnabled = true);
        }
        //------------------For BLE-------------------//

        void OnImageButtonClicked(object sender, EventArgs e)
        {
            var flyoutPage = new MainDashboardPage();
            flyoutPage.Detail = new NavigationPage(new SettingPage());
            App.Current.MainPage = flyoutPage;
            flyoutPage.IsPresented = true;

            //App.Current.MainPage = new NavigationPage(new MainDashboardPage());
        }

        private void Image_Focused(object sender, EventArgs e)
        {
            List<View> flexList = CoverSkin.Children.ToList();
            foreach (View flex in flexList)
            {
                FlexLayout flexLayout = (FlexLayout)flex;
                foreach(View img in flexLayout.Children.ToList())
                {
                    ImageButton imageButton = (ImageButton)img;
                    imageButton.BorderColor = Color.Transparent;
                }
            }
            ImageButton image = sender as ImageButton;
            image.BorderColor = Color.DeepSkyBlue;
            image.BorderWidth = 3;
            coverSkinUrl = image.Source.ToString().Substring(6);

            Gallery.ActiveLeftImage = coverSkinUrl;
        }

        private async void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
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
            var result = await MaterialDialog.Instance.ConfirmAsync(GlobalConstants.LangGUI.GetValueOrDefault("Do you really want to change the language?", "Do you really want to change the language?"), 
                GlobalConstants.LangGUI.GetValueOrDefault("Language Change", "Language Change"), GlobalConstants.LangGUI.GetValueOrDefault("OK", "OK"), 
                GlobalConstants.LangGUI.GetValueOrDefault("Cancel", "Cancel"), alertDialogConfiguration);


            //var result = await App.Current.MainPage.DisplayAlert(GlobalConstants.LangGUI.GetValueOrDefault("Language Change", "Language Change"),
            //    GlobalConstants.LangGUI.GetValueOrDefault("Do you really want to change the language?", "Do you really want to change the language?"), GlobalConstants.LangGUI.GetValueOrDefault("OK", "OK"), GlobalConstants.LangGUI.GetValueOrDefault("Cancel", "Cancel"));
            if (result ?? false)
            {
                languageNumber = ((Language)e.SelectedItem).languageNumber;
                language = ((Language)e.SelectedItem).description;
            }
            else return;
        }

        private void ImageButton_Clicked(object sender, EventArgs e)
        {
            AppSettings appSettings = new AppSettings();
            appSettings.coverSkinDefault = coverSkinUrl;
            appSettings.language = language;
            appSettings.languageNumber = languageNumber;
            App.Database.DeleteAllAppSettingAsync();
            App.Database.SaveSettingsAsync(appSettings);

            if (languageNumber == 0) App.Current.MainPage = new NavigationPage(new MainDashboardPage());
            else
            {
                GlobalConstants.LangGUI.Clear();
                GlobalConstants.SubTexts.Clear();
                GlobalConstants.GroupTexts.Clear();
                List<SubstanceText> subTextList = App.Database.GetSubstanceTextByLangAsync(languageNumber).Result;
                List<GroupText> groupTextList = App.Database.GetGroupTextByLanguageAsync(languageNumber).Result;
                List<LanguageGUI> langGUIList = App.Database.GetLanguageGUIByLanguageAsync(languageNumber).Result;
                foreach(SubstanceText substanceText in subTextList)
                {
                    GlobalConstants.SubTexts.Add(substanceText.SubstanceID ?? 0, substanceText.Description ?? "");
                }
                foreach (GroupText groupText in groupTextList)
                {
                    GlobalConstants.GroupTexts.Add(groupText.GroupNumber ?? 0, groupText.Description ?? "");
                }
                foreach (LanguageGUI langGUI in langGUIList)
                {
                    GlobalConstants.LangGUI.Add(langGUI.key ?? "", langGUI.content ?? "");
                }
                App.Current.MainPage = new NavigationPage(new MainDashboardPage());
            }
        }

        private void Slider_ValueChanged(object sender, ValueChangedEventArgs e)
        {
            string value = (e.NewValue / 100).ToString();
            MessagingCenter.Send(EventArgs.Empty, "Brightness", value);
            BrightnessValue.Text = e.NewValue.ToString() + "%";
        }

        private void ImageButton1_Clicked(object sender, EventArgs e)
        {
            App.Current.MainPage = new NavigationPage(new MainDashboardPage());
        }
    }
}
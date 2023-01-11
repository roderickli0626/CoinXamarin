using mycoin.Extensions;
using mycoin.Models;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.EventArgs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using XamarinEssentials = Xamarin.Essentials;

namespace mycoin.ViewModels
{
    public class SettingPageViewModel : BaseViewModel
    {
        string _settingTitle, _blueToothTitle, _statusTitle, _spaceTitle, _languageTitle, _coverSkin, _saveButton, _spaceContent, _brightness;
        string _addedDevices, _connectedDevices, _searchButtonTitle, _pair, _noDevices, _details, _disconnect;
        double _diskSpace;
        bool _noDeviceVisible, _deviceListVisible, _conDeviceVisible;
        public string SettingTitle { get => _settingTitle; set => SetProperty(ref _settingTitle, value); }
        public string BlueToothTitle { get => _blueToothTitle; set => SetProperty(ref _blueToothTitle, value); }
        public string StatusTitle { get => _statusTitle; set => SetProperty(ref _statusTitle, value); }
        public string SpaceTitle { get => _spaceTitle; set => SetProperty(ref _spaceTitle, value); }
        public string LanguageTitle { get => _languageTitle; set => SetProperty(ref _languageTitle, value); }
        public string CoverSkin { get => _coverSkin; set => SetProperty(ref _coverSkin, value); }
        public string SaveButton { get => _saveButton; set => SetProperty(ref _saveButton, value); }
        public string SpaceContent { get => _spaceContent; set => SetProperty(ref _spaceContent, value); }
        public string Brightness { get => _brightness; set => SetProperty(ref _brightness, value); }
        public string AddedDevices { get => _addedDevices; set=> SetProperty(ref _addedDevices, value); }
        public string ConnectedDevices { get => _connectedDevices; set => SetProperty(ref _connectedDevices, value); }
        public string SearchButtonTitle { get => _searchButtonTitle; set => SetProperty(ref _searchButtonTitle, value); }
        public string Pair { get => _pair; set => SetProperty(ref _pair, value); }
        public string NoDevices { get => _noDevices; set => SetProperty(ref _noDevices, value); }
        public string Details { get => _details; set => SetProperty(ref _details, value); }
        public string Disconnect { get => _disconnect; set => SetProperty(ref _disconnect, value); }
        public double DiskSpace { get => _diskSpace; set => SetProperty(ref _diskSpace, value); }
        public bool NoDeviceVisible { get => _noDeviceVisible; set => SetProperty(ref _noDeviceVisible, value); }
        public bool DeviceListVisible { get => _deviceListVisible; set => SetProperty(ref _deviceListVisible, value); }
        public bool ConDeviceVisible { get => _conDeviceVisible; set => SetProperty(ref _conDeviceVisible, value); }



        readonly IList<Language> languages;
        public ObservableCollection<Language> Languages { get; private set; }

        public ObservableCollection<IDevice> DeviceList { get; private set; }
        public ObservableCollection<IDevice> ConnectedDeviceList { get; private set; }
        private List<IDevice> _deviceList = new List<IDevice>();
        private List<IDevice> _connectedDeviceList = new List<IDevice>();
        IBluetoothLE ble;
        IAdapter adapter;
        List<byte> buffer = new List<byte>();
        public SettingPageViewModel()
        {
            CreateLanguageCollection();
            SettingTitle = GlobalConstants.LangGUI.GetValueOrDefault("Settings", "Settings");
            BlueToothTitle = GlobalConstants.LangGUI.GetValueOrDefault("Bluetooth Devices", "Bluetooth Devices");
            StatusTitle = GlobalConstants.LangGUI.GetValueOrDefault("Status LED", "Status LED");
            SpaceTitle = GlobalConstants.LangGUI.GetValueOrDefault("Space on Disk", "Space on Disk");
            LanguageTitle = GlobalConstants.LangGUI.GetValueOrDefault("Language", "Language");
            CoverSkin = GlobalConstants.LangGUI.GetValueOrDefault("Cover-Skin", "Cover-Skin");
            SaveButton = GlobalConstants.LangGUI.GetValueOrDefault("Save", "Save");
            Brightness = GlobalConstants.LangGUI.GetValueOrDefault("Brightness", "Brightness");
            DiskSpace = 0.7;
            SpaceContent = GlobalConstants.LangGUI.GetValueOrDefault("Occupied Storage Space", "Occupied Storage Space");
            AddedDevices = GlobalConstants.LangGUI.GetValueOrDefault("---Added Devices---", "---Added Devices---");
            ConnectedDevices = GlobalConstants.LangGUI.GetValueOrDefault("---Connected Devices---", "---Connected Devices---");
            SearchButtonTitle = GlobalConstants.LangGUI.GetValueOrDefault("SEARCH DEVICES", "SEARCH DEVICES");
            Pair = GlobalConstants.LangGUI.GetValueOrDefault("PAIR", "PAIR");
            NoDevices = GlobalConstants.LangGUI.GetValueOrDefault("No Devices", "No Devices");
            Details = GlobalConstants.LangGUI.GetValueOrDefault("DETAIL", "DETAIL");
            Disconnect = GlobalConstants.LangGUI.GetValueOrDefault("DISCONNECT", "DISCONNECT");
            NoDeviceVisible = true;
            DeviceListVisible = false;
            ConDeviceVisible = false;

            InitBLE();
        }
        void InitBLE()
        {
            ble = CrossBluetoothLE.Current;
            adapter = CrossBluetoothLE.Current.Adapter;
            adapter.ScanTimeout = 10000;
            adapter.ScanMode = ScanMode.LowLatency;

            var state = ble.State;
            ble.StateChanged += (s, e) =>
            {
            };
            adapter.DeviceDiscovered += (s, e) =>
            {
                if (e.Device != null && !string.IsNullOrEmpty(e.Device.Name))
                    _deviceList.Add(e.Device);
            };
        }
        void CreateLanguageCollection()
        {
            List<Language> languages = App.Database.GetAllLanguagesAsync().Result;
            Languages = new ObservableCollection<Language>(languages);
        }

        public ICommand ScanDeviceCommand => new Command(async () => await ScanDevice());
        public ICommand PairCommand => new Command<object>(async (sender) => await ConnectDevice(sender));
        public ICommand UnPairCommand => new Command<object>(async (sender) => await DisconnectDevice(sender));
        public ICommand DetailCommand => new Command<object>(async (sender) => await DeviceDetail(sender));
        private async Task ScanDevice()
        {
            NoDeviceVisible = false;
            DeviceListVisible = true;
            ConDeviceVisible = true;

            //if (!await PermissionsGrantedAsync())
            //{
            //    return;
            //}

            //DeviceList.Clear();
            //ConnectedDeviceList.Clear();

            //Test Devices
            //DeviceList.Add(new CustomDevice() { Name = "Device1" });
            //DeviceList.Add(new CustomDevice() { Name = "Device2" });
            //
            try 
            {
                foreach (var connectedDevice in adapter.ConnectedDevices)
                {
                    try
                    {
                        //await connectedDevice.UpdateRssiAsync();
                        _connectedDeviceList.Add(connectedDevice);
                    }
                    catch (Exception ex)
                    {
                        await App.Current.MainPage.DisplayAlert("Error1", ex.ToString(), "OK");
                    }
                }
                await adapter.StartScanningForDevicesAsync();
                DeviceList = new ObservableCollection<IDevice>(_deviceList);
                ConnectedDeviceList = new ObservableCollection<IDevice>(_connectedDeviceList);
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Error2", ex.ToString(), "OK");
            }
        }

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

        private async Task ConnectDevice(object sender)
        {
            if (sender is IDevice device)
            {
                await adapter.ConnectToDeviceAsync(device);
                DeviceList.Remove(device);
                ConnectedDeviceList.Add(device);
            }
        }
        private async Task DisconnectDevice(object sender)
        {
            if (sender is IDevice device)
            {
                await adapter.DisconnectDeviceAsync(device);
                DeviceList.Add(device);
                ConnectedDeviceList.Remove(device);
            }
        }
        private async Task DeviceDetail(object sender)
        {
            if (sender is IDevice device)
            {
                await App.Current.MainPage.DisplayAlert("Device Information", device.Name, "Cancel");

                try
                {
                    if (adapter.IsScanning)
                    {
                        await adapter.StopScanningForDevicesAsync();
                    }
                    // now get the service and characteristics of connected device
                    IService service = device.GetServiceAsync(Guid.Parse("0000ffe0-1000-8000-00805f9b34fb")).Result;

                    ICharacteristic characteristic = service.GetCharacteristicAsync(Guid.Parse("0000ffe1-1000-8000-00805f9b34fb")).Result;
                    // we attach the UpdateVale event to the characteristic
                    // and we start the service
                    characteristic.ValueUpdated += Characteristic_ValueUpdated;
                    await characteristic.StartUpdatesAsync();

                    // now we can write and hopefully read values
                    //byte[] data = { Coderequest.InitRequest, Coderequest.Info }; // My message to sendo to the machine to trigger his functions and his response

                    //await characteristic.WriteAsync(data); // Send the data

                }
                catch (Exception ex)
                {
                    Console.WriteLine("ERROR: " + ex.Message);
                }
            }
        }

        private void Characteristic_ValueUpdated(object sender, CharacteristicUpdatedEventArgs e)
        {
            //read value here
            buffer.AddRange(e.Characteristic.Value);
        }

        

    }
}

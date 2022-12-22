using mycoin.Extensions;
using mycoin.Models;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;

namespace mycoin.ViewModels
{
    public class SettingPageViewModel : BaseViewModel
    {
        string _settingTitle, _blueToothTitle, _statusTitle, _spaceTitle, _languageTitle, _coverSkin, _saveButton, _spaceContent, _brightness;
        string _addedDevices, _connectedDevices, _searchButtonTitle, _pair;
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
        public double DiskSpace { get => _diskSpace; set => SetProperty(ref _diskSpace, value); }
        public bool NoDeviceVisible { get => _noDeviceVisible; set => SetProperty(ref _noDeviceVisible, value); }
        public bool DeviceListVisible { get => _deviceListVisible; set => SetProperty(ref _deviceListVisible, value); }
        public bool ConDeviceVisible { get => _conDeviceVisible; set => SetProperty(ref _conDeviceVisible, value); }



        readonly IList<Language> languages;
        public ObservableCollection<Language> Languages { get; private set; }

        public ObservableCollection<IDevice> DeviceList { get; private set; } = new ObservableCollection<IDevice>();
        public ObservableCollection<IDevice> ConnectedDeviceList { get; private set; } = new ObservableCollection<IDevice>();
        IBluetoothLE ble;
        IAdapter adapter;
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
            adapter.DeviceDiscovered += (s, e) => DeviceList.Add(e.Device);
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

            DeviceList.Clear();
            ConnectedDeviceList.Clear();

            //Test Devices
            DeviceList.Add(new CustomDevice() { Name = "Device1" });
            DeviceList.Add(new CustomDevice() { Name = "Device2" });
            //

            foreach (var connectedDevice in adapter.ConnectedDevices)
            {
                try
                {
                    await connectedDevice.UpdateRssiAsync();
                }
                catch (Exception ex)
                {
                    return;
                }
            }
            await adapter.StartScanningForDevicesAsync();
        }

        private async Task ConnectDevice(object sender)
        {
            if (sender is IDevice device)
            {
                //await adapter.ConnectToDeviceAsync(device);
                DeviceList.Remove(device);
                ConnectedDeviceList.Add(device);
            }
        }
        private async Task DisconnectDevice(object sender)
        {
            if (sender is IDevice device)
            {
                //await adapter.DisconnectDeviceAsync(device);
                DeviceList.Add(device);
                ConnectedDeviceList.Remove(device);
            }
        }
        private async Task DeviceDetail(object sender)
        {
            if (sender is IDevice device)
            {
                var services = await device.GetServicesAsync();
                foreach (IService service in services)
                {
                    var characteristics = await service.GetCharacteristicsAsync();
                    foreach (ICharacteristic characteristic in characteristics)
                    {
                        var bytes = await characteristic.ReadAsync();
                        string info = bytes.ToString();

                        await App.Current.MainPage.DisplayAlert("Device Information", info, "Cancel");

                        //await characteristic.WriteAsync(bytes);
                        //var descriptors = await characteristic.GetDescriptorsAsync();
                    }
                }
            }
        }

    }
}

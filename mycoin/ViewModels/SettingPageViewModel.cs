using mycoin.Extensions;
using mycoin.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace mycoin.ViewModels
{
    public class SettingPageViewModel : BaseViewModel
    {
        string _settingTitle, _blueToothTitle, _statusTitle, _spaceTitle, _languageTitle, _coverSkin, _saveButton;
        public string SettingTitle { get => _settingTitle; set => SetProperty(ref _settingTitle, value); }
        public string BlueToothTitle { get => _blueToothTitle; set => SetProperty(ref _blueToothTitle, value); }
        public string StatusTitle { get => _statusTitle; set => SetProperty(ref _statusTitle, value); }
        public string SpaceTitle { get => _spaceTitle; set => SetProperty(ref _spaceTitle, value); }
        public string LanguageTitle { get => _languageTitle; set => SetProperty(ref _languageTitle, value); }
        public string CoverSkin { get => _coverSkin; set => SetProperty(ref _coverSkin, value); }
        public string SaveButton { get => _saveButton; set => SetProperty(ref _saveButton, value); }

        readonly IList<Language> languages;
        public ObservableCollection<Language> Languages { get; private set; }
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
        }
        void CreateLanguageCollection()
        {
            List<Language> languages = App.Database.GetAllLanguagesAsync().Result;
            Languages = new ObservableCollection<Language>(languages);
        }

    }
}

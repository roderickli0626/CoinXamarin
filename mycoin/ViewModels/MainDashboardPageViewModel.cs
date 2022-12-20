using mycoin.Extensions;
using mycoin.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace mycoin.ViewModels
{
    public class MainDashboardPageViewModel : BaseViewModel
    {
        public MainDashboardPageViewModel()
        {
            Note note = App.Database.GetNotesAsync().Result.Where(n => n.Isfavorite && n.PlayDateTime == null).FirstOrDefault();
            markImageUrl = note == null ? GetMarkDefaultURL() : "ic_coin_large_background_silver.png";
            visibleFlag = note == null ? false : true;
            Align = note == null ? "Vertical" : "Horizontal";
            nextTherapy = GlobalConstants.LangGUI.GetValueOrDefault("Your Next Therapy", "Your Next Therapy");
            firstProgram = note == null ? "No Program" : GlobalConstants.SubTexts.GetValueOrDefault(note.SubstanceID, note.Substance ?? "No Program");
            currentDate = note == null ? "No DefaultTime" : note.DefaultDateTime.ToString();
            titleFromUserInfo = GlobalConstants.LangGUI.GetValueOrDefault("Hello", "Hello") + " " + App.Userdata.userName;
            source = new List<MySubstance>();
            CreateGroupCollection();
        }
        string _currentDate, _firstProgram, _titleFromUserInfo, _markImageUrl, _align, _nextTherapy;
        bool _visibleFlag;
        public string currentDate { get => _currentDate; set => SetProperty(ref _currentDate, value); }

        public string firstProgram { get => _firstProgram; set => SetProperty(ref _firstProgram, value); }
        public string titleFromUserInfo { get => _titleFromUserInfo; set => SetProperty(ref _titleFromUserInfo, value); }
        public string markImageUrl { get => _markImageUrl; set => SetProperty(ref _markImageUrl, value); }
        public bool visibleFlag { get => _visibleFlag; set => SetProperty(ref _visibleFlag, value); }
        public string Align { get => _align; set => SetProperty(ref _align, value); }
        public string nextTherapy { get => _nextTherapy; set => SetProperty(ref _nextTherapy, value); }

        readonly IList<MySubstance> source;
        public ObservableCollection<MySubstance> MySubstances { get; private set; }

        void CreateGroupCollection()
        {
            List<Note> noteList = App.Database.GetNotesAsync().Result.Where(n => n.Isfavorite).ToList();
            foreach (Note note in noteList)
            {
                source.Add(new MySubstance
                {
                    ID = note.ID,
                    SubstanceImageUrl = "ic_biapp_icon_favorit.xml",
                    SubstanceName = GlobalConstants.SubTexts.GetValueOrDefault(note.SubstanceID, note.Substance ?? "No SubstanceName"),
                    Duration = note.Duration,
                });

            }
            source.Add(new MySubstance
            {
                ID = 0,
                SubstanceImageUrl = "ic_biapp_icon_favorit_hinzufuegen.xml",
                SubstanceName = GlobalConstants.LangGUI.GetValueOrDefault("Favorites Add", "Favorites Add")
            });
            MySubstances = new ObservableCollection<MySubstance>(source);
        }

        string GetMarkDefaultURL()
        {
            AppSettings setting = App.Database.GetSettingsAsync().Result;
            if (setting == null || setting.coverSkinDefault == "") return "ic_coin_large_background_wood.png";
            else return setting.coverSkinDefault ?? "ic_coin_large_background_wood.png";
        }
    }
}

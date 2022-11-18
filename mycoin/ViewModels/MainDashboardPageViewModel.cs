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
            Note note = App.Database.GetNotesAsync().Result.Where(n => n.Isfavorite).FirstOrDefault();
            markImageUrl = note == null ? "ic_coin_large_background_wood.png" : "ic_coin_large_background_silver.png";
            visibleFlag = note == null ? false : true;
            Align = note == null ? "Vertical" : "Horizontal";
            firstProgram = note == null ? "No Program" : note.Substance ?? "No Program";
            currentDate = note == null ? "No DefaultTime" : note.DefaultDateTime.ToString();
            titleFromUserInfo = "Hello " + App.Userdata.userName;
            source = new List<MySubstance>();
            CreateGroupCollection();
        }
        string _currentDate, _firstProgram, _titleFromUserInfo, _markImageUrl, _align;
        bool _visibleFlag;
        public string currentDate { get => _currentDate; set => SetProperty(ref _currentDate, value); }

        public string firstProgram { get => _firstProgram; set => SetProperty(ref _firstProgram, value); }
        public string titleFromUserInfo { get => _titleFromUserInfo; set => SetProperty(ref _titleFromUserInfo, value); }
        public string markImageUrl { get => _markImageUrl; set => SetProperty(ref _markImageUrl, value); }
        public bool visibleFlag { get => _visibleFlag; set => SetProperty(ref _visibleFlag, value); }
        public string Align { get => _align; set => SetProperty(ref _align, value); }

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
                    SubstanceName = note.Substance ?? "No SubstanceName"
                });

            }
            source.Add(new MySubstance
            {
                ID = 0,
                SubstanceImageUrl = "ic_biapp_icon_favorit_hinzufuegen.xml",
                SubstanceName = "Favorites Add"
            });
            MySubstances = new ObservableCollection<MySubstance>(source);
        }
    }
}

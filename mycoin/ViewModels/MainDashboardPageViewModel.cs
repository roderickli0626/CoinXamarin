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
            currentDate = DateTime.Now.ToString();
            Note note = App.Database.GetNotesAsync().Result.FirstOrDefault();
            firstProgram = note.Substance ?? "No Program";
            titleFromUserInfo = "Hello " + App.Userdata.userName;
            source = new List<MyGroup>();
            CreateGroupCollection();
        }
        string _currentDate, _firstProgram, _titleFromUserInfo;
        public string currentDate { get => _currentDate; set => SetProperty(ref _currentDate, value); }

        public string firstProgram { get => _firstProgram; set => SetProperty(ref _firstProgram, value); }
        public string titleFromUserInfo { get => _titleFromUserInfo; set => SetProperty(ref _titleFromUserInfo, value); }

        readonly IList<MyGroup> source;
        public ObservableCollection<MyGroup> MyGroups { get; private set; }

        void CreateGroupCollection()
        {
            List<Note> noteList = App.Database.GetNotesAsync().Result;
            foreach (string groupName in noteList.Select(n => n.GroupName ?? "No Name").Distinct().ToList())
            {
                source.Add(new MyGroup
                {
                    ImageUrl = "ic_biapp_icon_favorit.xml",
                    GroupName = groupName
                });

            }
            source.Add(new MyGroup
            {
                ImageUrl = "ic_biapp_icon_favorit_hinzufuegen.xml",
                GroupName = "Favorites Add"
            });
            MyGroups = new ObservableCollection<MyGroup>(source);
        }
    }
}

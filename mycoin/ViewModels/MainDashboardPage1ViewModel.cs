using mycoin.Models;
using mycoin.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace mycoin.ViewModels
{
    public class MainDashboardPage1ViewModel : BaseViewModel
    {
        public ObservableRangeCollection<MyGroupViewModel> MyGroups { get; private set; }
            = new ObservableRangeCollection<MyGroupViewModel>();

        public ICommand LoadDataCommand { get; private set; }
        public ICommand HeaderClickCommand { get; private set; }

        public List<MySubstance> mySubstances { get; private set; }

        public MainDashboardPage1ViewModel()
        {
            this.LoadDataCommand = new Command(async () => await ExecuteLoadDataCommand());
            this.HeaderClickCommand = new Command<MyGroupViewModel>((item) => ExecuteHeaderClickCommand(item));
            
        }

        private async Task ExecuteLoadDataCommand()
        {
            List<Note> noteList = App.Database.GetNotesAsync().Result;
            foreach (string groupName in noteList.Select(n => n.GroupName ?? "No GroupName").Distinct().ToList())
            {
                mySubstances = new List<MySubstance>();

                foreach (Note note in noteList.Where(s => s.GroupName == groupName).ToList())
                {
                    mySubstances.Add(new MySubstance
                    {
                        ID = note.ID,
                        SubstanceImageUrl = "dot.png",
                        SubstanceName = note.Substance ?? "No SubstanceName"
                    });
                }

                MyGroups.Add(new MyGroupViewModel(new MyGroup
                {
                    ImageUrl = "ic_biapp_icon_favorit.xml",
                    GroupName = groupName,
                    MySubstances = mySubstances
                }));
            }
        }
        private void ExecuteHeaderClickCommand(MyGroupViewModel item)
        {
            item.Expanded = !item.Expanded;
        }

        public ICommand AddCommand => new Command(async () => {
            
        });
    }
}

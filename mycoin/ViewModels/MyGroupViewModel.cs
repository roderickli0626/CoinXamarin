using mycoin.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using Xamarin.CommunityToolkit.ObjectModel;

namespace mycoin.ViewModels
{
    public class MyGroupViewModel : ObservableRangeCollection<MySubstanceViewModel>, INotifyPropertyChanged
    {
        private MyGroup _group;

        // It's a backup variable for storing CountryViewModel objects
        private ObservableRangeCollection<MySubstanceViewModel> Substances
            = new ObservableRangeCollection<MySubstanceViewModel>();

        public MyGroupViewModel(MyGroup group, bool expanded = true)
        {
            this._group = group;
            this._expanded = expanded;
            // Continent has many countries. Once we get it, init CountryViewModel and store it in a backup variable
            foreach (MySubstance c in group.MySubstances)
            {
                Substances.Add(new MySubstanceViewModel(c));
            }
            // ContinentViewModel add a range with CountryViewModel
            if (expanded)
                this.AddRange(Substances);
        }

        public string GroupName { get { return _group.GroupName; } }

        private bool _expanded;
        public bool Expanded
        {
            get { return _expanded; }
            set
            {
                if (_expanded != value)
                {
                    _expanded = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("Expanded"));
                    OnPropertyChanged(new PropertyChangedEventArgs("StateIcon"));
                    if (_expanded)
                    {
                        this.AddRange(Substances);
                    }
                    else
                    {
                        this.Clear();
                    }
                }
            }
        }

        public string StateIcon
        {
            get { return Expanded ? "up" : "down"; }
        }
    }
}

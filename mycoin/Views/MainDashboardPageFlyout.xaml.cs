using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace mycoin.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainDashboardPageFlyout : ContentPage
    {
        public ListView ListView;

        public MainDashboardPageFlyout()
        {
            InitializeComponent();

            BindingContext = new MainDashboardPageFlyoutViewModel();
            ListView = MenuItemsListView;
        }

        private class MainDashboardPageFlyoutViewModel : INotifyPropertyChanged
        {
            public ObservableCollection<MainDashboardPageFlyoutMenuItem> MenuItems { get; set; }

            public MainDashboardPageFlyoutViewModel()
            {
                MenuItems = new ObservableCollection<MainDashboardPageFlyoutMenuItem>(new[]
                {
                    new MainDashboardPageFlyoutMenuItem { Id = 1, Title = "User Information", IsVisible = true, TargetType = new UserInfoPage().GetType() },
                    new MainDashboardPageFlyoutMenuItem { Id = 2, Title = "Settings", IsVisible = true },
                    new MainDashboardPageFlyoutMenuItem { Id = 3, Title = "Support",IsVisible = true },
                    new MainDashboardPageFlyoutMenuItem { Id = 4, Title = "Theme", IsVisible = true },
                });
            }

            #region INotifyPropertyChanged Implementation
            public event PropertyChangedEventHandler PropertyChanged;
            void OnPropertyChanged([CallerMemberName] string propertyName = "")
            {
                if (PropertyChanged == null)
                    return;

                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            #endregion
        }
    }
}
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace mycoin.ViewModels
{
    public class BaseViewModel : NotifyChange
    {
        public BaseViewModel()
        {
        }
        //public void RunIndicator(string loadingtext = "Loading...")
        public void RunIndicator(string loadingtext = "Waiting...")
        {
            Acr.UserDialogs.UserDialogs.Instance.ShowLoading(loadingtext, maskType: Acr.UserDialogs.MaskType.Black);
        }

        public void StopIndicator()
        {
            Device.BeginInvokeOnMainThread(() =>
            Acr.UserDialogs.UserDialogs.Instance.HideLoading()
            );
        }
        public void ShowErrorSnackbar(string message)
        {
            Acr.UserDialogs.ToastConfig.DefaultBackgroundColor = Color.Red;
            Acr.UserDialogs.UserDialogs.Instance.Toast(message, TimeSpan.FromSeconds(1.5));
        }


        public void ShowSuccessSnackbar(string message)
        {
            Acr.UserDialogs.ToastConfig.DefaultBackgroundColor = Color.Green;
            Acr.UserDialogs.UserDialogs.Instance.Toast(message, TimeSpan.FromSeconds(1.5));
        }

        public void ShowInternetError()
        {
            Acr.UserDialogs.ToastConfig.DefaultBackgroundColor = Color.Red;
            Acr.UserDialogs.UserDialogs.Instance.Toast("No Internet Connection", TimeSpan.FromSeconds(1.5));
        }

        public void ShowSomethingWrongMsg()
        {
            Acr.UserDialogs.ToastConfig.DefaultBackgroundColor = Color.Red;
            Acr.UserDialogs.UserDialogs.Instance.Toast("Something went wrong!!", TimeSpan.FromSeconds(1.5));
        }

        public ICommand BackCommand => new Command(() => NavigateBack());

        public async Task NavigateToPage(Page pg)
        {
            try
            {
                var lastpage = App.Current.MainPage.Navigation.NavigationStack.LastOrDefault();
                if (lastpage.GetType() == pg.GetType())
                {
                    return;
                }
                await App.Current.MainPage.Navigation.PushAsync(pg);
            }
            catch (Exception ex)
            {

            }
        }

        public void NavigateBack()
        {
            App.Current.MainPage.Navigation.PopAsync();
        }

        public async Task PoptoRoot()
        {
            await App.Current.MainPage.Navigation.PopToRootAsync();
        }
    }
}

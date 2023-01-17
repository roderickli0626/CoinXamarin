using mycoin.Extensions;
using mycoin.Helpers;
using mycoin.Models;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace mycoin.ViewModels
{
    public class ForgotPasswordPageViewModel : BaseViewModel
    {
        public ForgotPasswordPageViewModel()
        {
            ForgotPassword = GlobalConstants.LangGUI.GetValueOrDefault("Forgot Password", "Forgot Password");
            EmailPlaceholder = GlobalConstants.LangGUI.GetValueOrDefault("Email Address", "Email Address");
            SendTitle = GlobalConstants.LangGUI.GetValueOrDefault("Send", "Send");
        }
        #region Properties
        bool _isvalidemail;
        string _email, _emailerror, _forgotPassword, _emailPlaceholder, _sendTitle;

        public string ForgotPassword { get => _forgotPassword; set => SetProperty(ref _forgotPassword, value); }
        public string EmailPlaceholder { get => _emailPlaceholder; set => SetProperty(ref _emailPlaceholder, value); }
        public string SendTitle { get => _sendTitle; set => SetProperty(ref _sendTitle, value); }

        public string EmailAddress { get => _email; set => SetProperty(ref _email, value); }

        public string EmailError { get => _emailerror; set => SetProperty(ref _emailerror, value); }

        public bool IsValidEmail { get => _isvalidemail; set => SetProperty(ref _isvalidemail, value); }

        #endregion

        #region Commands

        public ICommand SendCommand => new Command(async () => {



            if (string.IsNullOrEmpty(EmailAddress))
            {
                IsValidEmail = true;
                EmailError = "Enter email address";
                return;
            }
            else
            {
                IsValidEmail = false;
            }

            if (!EmailAddress.ValidateEmail())
            {
                IsValidEmail = true;
                EmailError = "Enter valid email address";
                return;
            }
            else
            {
                IsValidEmail = false;
            }
            
            RunIndicator();
            LoginResponse response = await HttpHelper.Instance.PostContentAsync<LoginResponse>(ApiURLs.ForgotPassword, EmailAddress);
            try
            {
                if (response.IsSuccess)
                {
                    await App.Current.MainPage.Navigation.PopAsync();
                }
                else
                {
                    ShowErrorSnackbar(response.Message);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                StopIndicator();
            }
        });

        #endregion

    }
}

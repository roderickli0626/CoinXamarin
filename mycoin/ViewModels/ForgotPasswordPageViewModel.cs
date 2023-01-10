using mycoin.Extensions;
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
            try
            {
                List<string> toAddress = new List<string>();
                toAddress.Add(EmailAddress);
                var message = new EmailMessage
                {
                    Subject = "Forgot Password",
                    Body = "Please Reset Password",
                    To = toAddress,
                };
                await Email.ComposeAsync(message);

                await App.Current.MainPage.Navigation.PopAsync();
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

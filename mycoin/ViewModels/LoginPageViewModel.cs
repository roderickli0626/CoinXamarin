using System;
using System.Windows.Input;
using mycoin.Extensions;
using mycoin.Helpers;
using mycoin.Models;
using mycoin.Views;
using Xamarin.Forms;

namespace mycoin.ViewModels
{
    public class LoginPageViewModel: BaseViewModel
    {
        public LoginPageViewModel()
        {
        }
        #region Properties

        bool _isvalidemail, _isvalidpassword;
        string _email, _password, _emailerror, _passworderror;

        public string Email { get => _email; set => SetProperty(ref _email, value); }

        public string Password { get => _password; set => SetProperty(ref _password, value); }

        public string PasswordError { get => _passworderror; set => SetProperty(ref _passworderror, value); }

        public string EmailError { get => _emailerror; set => SetProperty(ref _emailerror, value); }


        public bool IsValidEmail { get => _isvalidemail; set => SetProperty(ref _isvalidemail, value); }

        public bool IsValidPassword { get => _isvalidpassword; set => SetProperty(ref _isvalidpassword, value); }

        #endregion


        #region Commands
        public ICommand LoginCommand => new Command(async() => {

            if (string.IsNullOrEmpty(Email))
            {
                IsValidEmail = true;
                EmailError = "Enter email address";
                return;
            }
            else
            {
                IsValidEmail = false;
                EmailError = "";
            }

            if (!Email.ValidateEmail())
            {
                IsValidEmail = true;
                EmailError = "Enter valid email address";
                return;
            }
            else
            {
                IsValidEmail = false;
                EmailError = "";
            }

            if (string.IsNullOrEmpty(Password))
            {
                IsValidPassword = true;
                PasswordError = "Enter password";
                return;
            }
            else
            {
                IsValidPassword = false;
                PasswordError = "";
            }
            LoginRequest req = new LoginRequest()
            {
                email = Email,
                password = Password
            };

            RunIndicator();
            LoginResponse response = await HttpHelper.Instance.PostContentAsync<LoginResponse>(ApiURLs.Login, req);
            try
            {
                if (response != null)
                {
                    if (response.result == true)
                    {
                        App.Userdata = new Userdata()
                        {
                            userid = response.userId,
                            languageid = response.languageNumber,
                            devicenum = response.deviceNumber,
                            userName = response.userName,
                        };
                        App.Current.MainPage = new NavigationPage(new DashboardPage());
                    }
                    else
                    {
                        ShowSomethingWrongMsg();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                StopIndicator();
            }

        });

        public ICommand RegisterCommand => new Command(async() => {
            await NavigateToPage(new RegisterPage());
        });

        public ICommand ForgotPasswordCommand => new Command(() => {

        });

        #endregion
    }
}

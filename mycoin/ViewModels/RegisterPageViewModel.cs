using System;
using System.Globalization;
using System.Windows.Input;
using mycoin.Extensions;
using mycoin.Helpers;
using mycoin.Models;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;

namespace mycoin.ViewModels
{
    public class RegisterPageViewModel : BaseViewModel
    {
        public RegisterPageViewModel()
        {
            SerialNumber = System.Guid.NewGuid().ToString().Substring(0, 28);
            Language = CultureInfo.CurrentUICulture.EnglishName;
        }

        #region Properties
        bool _isvalidpassword, _isvalidrepeatpassword, _isvalidfirstname, _isvalidfamilyname, _isvalidemail;
        string _email, _password, _firstname, _familyname, _repeatpassword, _passwordlength, _emailerror, _passworderror, _firstnameerror, _familynameerror, _repeatpassworderror;

        public string Email { get => _email; set => SetProperty(ref _email, value); }

        public string Password { get => _password; set => SetProperty(ref _password, value); }

        public string FirstName { get => _firstname; set => SetProperty(ref _firstname, value); }

        public string FamilyName { get => _familyname; set => SetProperty(ref _familyname, value); }

        public string RepeatPassword { get => _repeatpassword; set => SetProperty(ref _repeatpassword, value); }

        public string PasswordLength { get => _passwordlength; set => SetProperty(ref _passwordlength, value); }



        public string EmailError { get => _emailerror; set => SetProperty(ref _emailerror, value); }

        public string PasswordError { get => _passworderror; set => SetProperty(ref _passworderror, value); }

        public string FirstNameError { get => _firstnameerror; set => SetProperty(ref _firstnameerror, value); }

        public string FamilyNameError { get => _familynameerror; set => SetProperty(ref _familynameerror, value); }

        public string RepeatPasswordError { get => _repeatpassworderror; set => SetProperty(ref _repeatpassworderror, value); }

        public string SerialNumber { get; }

        public string Language { get; }


        public bool IsValidEmail { get => _isvalidemail; set => SetProperty(ref _isvalidemail, value); }

        public bool IsValidPassword { get => _isvalidpassword; set => SetProperty(ref _isvalidpassword, value); }

        public bool IsValidFamilyName  { get => _isvalidfamilyname; set => SetProperty(ref _isvalidfamilyname, value); }

        public bool IsValidFirstName { get => _isvalidfirstname; set => SetProperty(ref _isvalidfirstname, value); }

        public bool IsValidRepeatPassword { get => _isvalidrepeatpassword; set => SetProperty(ref _isvalidrepeatpassword, value); }


        #endregion

        #region Commands

        public ICommand RegisterCommand => new Command(async() => {
           


            if (string.IsNullOrEmpty(FirstName))
            {
                IsValidFirstName = true;
                FirstNameError = "Enter first name";
                return;
            }
            else
            {
                IsValidFirstName = false;
            }

            if (string.IsNullOrEmpty(FamilyName))
            {
                IsValidFamilyName = true;
                FamilyNameError = "Enter family name";
                return;
            }
            else
            {
                IsValidFamilyName = false;
            }

            if (string.IsNullOrEmpty(Email))
            {
                IsValidEmail = true;
                EmailError = "Enter email address";
                return;
            }
            else
            {
                IsValidEmail = false;
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
            }
            if (Password.Length < 8)
            {
                PasswordLength = "Your password has to contains at least 8 characters";
                return;
            }
            else
            {
                PasswordLength = "";
            }
            if (Password != RepeatPassword)
            {
                IsValidRepeatPassword = true;
                RepeatPasswordError = "Password and repeat password should be same";
                return;
            }
            else
            {
                IsValidRepeatPassword = false;
            }

            RegisterRequest req = new RegisterRequest()
            {
                email = Email,
                familyName = FamilyName,
                firstName = FirstName,
                password = Password,
                serialNumber = SerialNumber,
                language = Language
            };

            RunIndicator();
            RegisterResponse response = await HttpHelper.Instance.PostContentAsync<RegisterResponse>(ApiURLs.Register, req);
            try
            {
                if (response != null)
                {
                    //if (response.result.ToLower() == "ok")
                    if (response.result == true)
                    {
                        ShowSuccessSnackbar("Registered Successfully");
                        NavigateBack();
                    }
                    else
                    {
                        //ShowErrorSnackbar(response.result);
                        ShowErrorSnackbar("Register Error");
                    }
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                StopIndicator();
            }
        });

        #endregion

    }
}

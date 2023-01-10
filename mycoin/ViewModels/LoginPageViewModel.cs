using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using mycoin.Extensions;
using mycoin.Helpers;
using mycoin.Models;
using mycoin.Views;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;

namespace mycoin.ViewModels
{
    public class LoginPageViewModel: BaseViewModel
    {
        public LoginPageViewModel()
        {
            checkConnection();
            //if (!checkConnection().Result)
            //{
            //    return;
            //}

            AppSettings settings = App.Database.GetSettingsAsync().Result;
            if (settings != null && settings.languageNumber != 0)
            {
                GlobalConstants.LangGUI.Clear();
                List<LanguageGUI> langGUIList = App.Database.GetLanguageGUIByLanguageAsync(settings.languageNumber).Result;
                foreach (LanguageGUI langGUI in langGUIList)
                {
                    GlobalConstants.LangGUI.Add(langGUI.key ?? "", langGUI.content ?? "");
                }
            }
            Title = GlobalConstants.LangGUI.GetValueOrDefault("Welcome!", "Welcome!");
            SubTitle = GlobalConstants.LangGUI.GetValueOrDefault("Sign in to continue", "Sign in to continue");
            EmailPlaceholder = GlobalConstants.LangGUI.GetValueOrDefault("Email Address", "Email Address");
            PasswordPlaceholder = GlobalConstants.LangGUI.GetValueOrDefault("Password", "Password");
            RememberMe = GlobalConstants.LangGUI.GetValueOrDefault("Remember Me", "Remember Me");
            ForgotPassword = GlobalConstants.LangGUI.GetValueOrDefault("Forgot Password", "Forgot Password");
            SignIn = GlobalConstants.LangGUI.GetValueOrDefault("SIGN IN", "SIGN IN");
            NewUser = GlobalConstants.LangGUI.GetValueOrDefault("New User?", "New User?");
            SignUp = GlobalConstants.LangGUI.GetValueOrDefault(" Sign Up", " Sign Up");

            Userdata savedUserInfo = App.Database.GetUserdataAsync().Result;
            if (savedUserInfo == null || !savedUserInfo.isActive) return;
            else
            {
                autoLogin(savedUserInfo);
            }
            isChecked = false;
        }

        public async void checkConnection()
        {
            LoginRequest req = new LoginRequest()
            {
                email = "test@testconnection.com",
                password = "testconnection",
            };
            LoginResponse response = await HttpHelper.Instance.PostContentAsync<LoginResponse>(ApiURLs.Login, req);
            if (response == null)
            {
                await App.Current.MainPage.DisplayAlert("Warning", "Connection Error!. Please try again later", "OK");
                //Close Application
                //System.Threading.Thread.CurrentThread.Abort();
            }
        }

        public async void autoLogin(Userdata savedUserInfo)
        {
            LoginRequest req = new LoginRequest()
            {
                email = savedUserInfo.email,
                password = savedUserInfo.password,
            };

            RunIndicator();
            LoginResponse response = await HttpHelper.Instance.PostContentAsync<LoginResponse>(ApiURLs.Login, req);

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

                    ////Save login User into DB
                    //await App.Database.DeleteAllUserdataAsync();

                    //App.Userdata.email = Email;
                    //App.Userdata.password = Password;
                    //App.Userdata.isActive = true;
                    //await App.Database.SaveUserdataAsync(App.Userdata);

                    App.Current.MainPage = new NavigationPage(new DashboardPage());
                }
                else
                {
                    ShowSomethingWrongMsg();
                }
            }
            StopIndicator();
        }

        #region Properties

        bool _isvalidemail, _isvalidpassword, _isChecked;
        string _email, _password, _emailerror, _passworderror, _title, _subTitle, _emailPlaceholder, _passwordPlaceholder, _rememberMe, _forgotPassword;
        string _signIn, _newUser, _signUp;

        public string Title { get => _title; set => SetProperty(ref _title, value); }
        public string SubTitle { get => _subTitle; set => SetProperty(ref _subTitle, value); }
        public string EmailPlaceholder { get => _emailPlaceholder; set => SetProperty(ref _emailPlaceholder, value); }
        public string PasswordPlaceholder { get => _passwordPlaceholder; set => SetProperty(ref _passwordPlaceholder, value); }
        public string RememberMe { get => _rememberMe; set => SetProperty(ref _rememberMe, value); }
        public string ForgotPassword { get => _forgotPassword; set => SetProperty(ref _forgotPassword, value); }
        public string SignIn { get => _signIn; set => SetProperty(ref _signIn, value); }
        public string SignUp { get => _signUp; set => SetProperty(ref _signUp, value); }
        public string NewUser { get => _newUser; set => SetProperty(ref _newUser, value); }

        public string Email { get => _email; set => SetProperty(ref _email, value); }

        public string Password { get => _password; set => SetProperty(ref _password, value); }

        public string PasswordError { get => _passworderror; set => SetProperty(ref _passworderror, value); }

        public string EmailError { get => _emailerror; set => SetProperty(ref _emailerror, value); }


        public bool IsValidEmail { get => _isvalidemail; set => SetProperty(ref _isvalidemail, value); }

        public bool IsValidPassword { get => _isvalidpassword; set => SetProperty(ref _isvalidpassword, value); }

        public bool isChecked { get => _isChecked; set => SetProperty(ref _isChecked, value); }

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

                        //Save User into DB if 'Remember User' checked
                        if (isChecked)
                        {
                            await App.Database.DeleteAllUserdataAsync();

                            App.Userdata.email = Email;
                            App.Userdata.password = Password;
                            App.Userdata.isActive = true;
                            await App.Database.SaveUserdataAsync(App.Userdata);
                        }
                        else
                        {
                            await App.Database.DeleteAllUserdataAsync();
                        }

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

        public ICommand ForgotPasswordCommand => new Command(async() => {
            await NavigateToPage(new ForgotPasswordPage());
        });

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using mycoin.Extensions;
using mycoin.Helpers;
using mycoin.Models;
using mycoin.Views;
using Plugin.Connectivity;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;
using XF.Material.Forms.Resources;
using XF.Material.Forms.UI.Dialogs.Configurations;
using XF.Material.Forms.UI.Dialogs;

namespace mycoin.ViewModels
{
    public class LoginPageViewModel: BaseViewModel
    {
        public bool? IsOffLineMode = false;
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

                var alertDialogConfiguration = new MaterialAlertDialogConfiguration()
                {
                    BackgroundColor = XF.Material.Forms.Material.GetResource<Color>(MaterialConstants.Color.SURFACE),
                    TitleTextColor = XF.Material.Forms.Material.GetResource<Color>(MaterialConstants.Color.ON_SURFACE),
                    MessageTextColor = XF.Material.Forms.Material.GetResource<Color>(MaterialConstants.Color.ON_SURFACE),
                    //TintColor = XF.Material.Forms.Material.GetResource<Color>(MaterialConstants.Color.ON_BACKGROUND),
                    TintColor = Color.FromHex("#018BD3"),
                    CornerRadius = 30,
                    ScrimColor = Color.FromHex("#232F34").MultiplyAlpha(0.32),
                    ButtonAllCaps = false
                };
                IsOffLineMode = await MaterialDialog.Instance.ConfirmAsync("Your phone is currently offline. Would you like to continue working with your APP in offline mode?",
                    "Warning", "YES", "NO", alertDialogConfiguration);

                Userdata savedUserInfo = App.Database.GetUserdataAsync().Result;
                if (savedUserInfo == null || !savedUserInfo.isActive) return;
                else
                {
                    autoLogin(savedUserInfo);
                }

                //await App.Current.MainPage.DisplayAlert("Warning", "Connection Error!. Please try again later", "OK");
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

            if (IsOffLineMode ?? false)
            {
                App.Userdata = new Userdata()
                {
                    userid = savedUserInfo.userid,
                    languageid = savedUserInfo.languageid,
                    devicenum = savedUserInfo.devicenum,
                    userName = savedUserInfo.userName,
                };
                App.Current.MainPage = new NavigationPage(new MainDashboardPage());
            }
            else
            {
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

                        if (response.Category == "Privatpersonen")
                        {
                            //TODO move to QuestionPage if user is private
                            if (App.Database.GetSelectedQuestionsByUserAsync(App.Userdata.userid).Result == null)
                            {
                                GlobalConstants.GroupIds.Clear();
                                App.Current.MainPage = new NavigationPage(new QuestionPage());
                            }
                            else
                            {
                                List<string> Ids = new List<string>(App.Database.GetSelectedQuestionsByUserAsync(App.Userdata.userid).Result.SelectedQuestionList.Split(","));
                                GlobalConstants.GroupIds = Ids.Select(int.Parse).ToList();
                                App.Current.MainPage = new NavigationPage(new DashboardPage());
                            }
                        }
                        else
                        {
                            // move to DashboarPage if user is not private
                            App.Current.MainPage = new NavigationPage(new DashboardPage());
                        }
                    }
                    else
                    {
                        ShowSomethingWrongMsg();
                    }
                }
                StopIndicator();
            }
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

            if (IsOffLineMode ?? false)
            {
                Userdata userdata = App.Database.GetUserdataAsync().Result;
                if (userdata == null || userdata.email != Email)
                {
                    ShowSomethingWrongMsg();
                    return;
                }
                
                App.Userdata = new Userdata()
                {
                    userid = userdata.userid,
                    languageid = userdata.languageid,
                    devicenum = userdata.devicenum,
                    userName = userdata.userName,
                };
                App.Current.MainPage = new NavigationPage(new MainDashboardPage());
            }
            else
            {
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

                            await App.Database.DeleteAllUserdataAsync();

                            App.Userdata.email = Email;
                            App.Userdata.password = Password;
                            //Save User into DB if 'Remember User' checked
                            if (isChecked)
                            {
                                App.Userdata.isActive = true;
                                await App.Database.SaveUserdataAsync(App.Userdata);
                            }
                            else
                            {
                                App.Userdata.isActive = false;
                                await App.Database.SaveUserdataAsync(App.Userdata);
                            }

                            if (response.Category == "Privatpersonen")
                            {
                                //TODO move to QuestionPage if user is private
                                if (App.Database.GetSelectedQuestionsByUserAsync(App.Userdata.userid).Result == null)
                                {
                                    GlobalConstants.GroupIds.Clear();
                                    App.Current.MainPage = new NavigationPage(new QuestionPage());
                                }
                                else
                                {
                                    List<string> Ids = new List<string>(App.Database.GetSelectedQuestionsByUserAsync(App.Userdata.userid).Result.SelectedQuestionList.Split(","));
                                    if (Ids.Contains("")) GlobalConstants.GroupIds = new List<int> { 0 };
                                    else GlobalConstants.GroupIds = Ids.Select(int.Parse).ToList();
                                    App.Current.MainPage = new NavigationPage(new DashboardPage());
                                }
                            }
                            else
                            {
                                // move to DashboarPage if user is not private
                                App.Current.MainPage = new NavigationPage(new DashboardPage());
                            }
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
            }
        });

        public ICommand RegisterCommand => new Command(async() => {
            if (!CrossConnectivity.Current.IsConnected)
            {
                var alertDialogConfiguration = new MaterialAlertDialogConfiguration()
                {
                    BackgroundColor = XF.Material.Forms.Material.GetResource<Color>(MaterialConstants.Color.SURFACE),
                    TitleTextColor = XF.Material.Forms.Material.GetResource<Color>(MaterialConstants.Color.ON_SURFACE),
                    MessageTextColor = XF.Material.Forms.Material.GetResource<Color>(MaterialConstants.Color.ON_SURFACE),
                    //TintColor = XF.Material.Forms.Material.GetResource<Color>(MaterialConstants.Color.ON_BACKGROUND),
                    TintColor = Color.FromHex("#018BD3"),
                    CornerRadius = 30,
                    ScrimColor = Color.FromHex("#232F34").MultiplyAlpha(0.32),
                    ButtonAllCaps = false
                };
                await MaterialDialog.Instance.ConfirmAsync("Please connect to the Internet first",
                    "Warning", "OK", "", alertDialogConfiguration);
                //await App.Current.MainPage.DisplayAlert("Warning", "Please connect to the Internet first", "OK");
            }
            else await NavigateToPage(new RegisterPage());
        });

        public ICommand ForgotPasswordCommand => new Command(async() => {
            if (!CrossConnectivity.Current.IsConnected)
            {
                var alertDialogConfiguration = new MaterialAlertDialogConfiguration()
                {
                    BackgroundColor = XF.Material.Forms.Material.GetResource<Color>(MaterialConstants.Color.SURFACE),
                    TitleTextColor = XF.Material.Forms.Material.GetResource<Color>(MaterialConstants.Color.ON_SURFACE),
                    MessageTextColor = XF.Material.Forms.Material.GetResource<Color>(MaterialConstants.Color.ON_SURFACE),
                    //TintColor = XF.Material.Forms.Material.GetResource<Color>(MaterialConstants.Color.ON_BACKGROUND),
                    TintColor = Color.FromHex("#018BD3"),
                    CornerRadius = 30,
                    ScrimColor = Color.FromHex("#232F34").MultiplyAlpha(0.32),
                    ButtonAllCaps = false
                };
                await MaterialDialog.Instance.ConfirmAsync("Please connect to the Internet first",
                    "Warning", "OK", "", alertDialogConfiguration);
                //await App.Current.MainPage.DisplayAlert("Warning", "Please connect to the Internet first", "OK");
            }
            else await NavigateToPage(new ForgotPasswordPage());
        });

        #endregion
    }
}

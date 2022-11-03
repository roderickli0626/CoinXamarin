using System;
using System.Collections.Generic;
using mycoin.ViewModels;
using Xamarin.Forms;

namespace mycoin.Views
{
    public partial class LoginPage : BasePage
    {
        LoginPageViewModel vm;
        public LoginPage()
        {
            InitializeComponent();
            this.BindingContext = vm = new LoginPageViewModel();

        }

       
    }
}

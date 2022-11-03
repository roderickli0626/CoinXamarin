using System;
using System.Collections.Generic;
using mycoin.ViewModels;
using Xamarin.Forms;

namespace mycoin.Views
{
    public partial class RegisterPage : BasePage
    {
        RegisterPageViewModel vm;
        public RegisterPage()
        {
            InitializeComponent();
            this.BindingContext = vm = new RegisterPageViewModel();
        }
    }
}

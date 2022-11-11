using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace mycoin.Views
{
    public partial class DashboardPage : BasePage
    {
        Task rotate;
        public DashboardPage()
        {
            InitializeComponent();
            rotate = RotateImageContinously();
        }

        public async Task RotateImageContinously()
        {
            while (true) // a CancellationToken in real life ;-)
            {
                for (int i = 1; i < 7; i++)
                {
                    if (LoadingImage.Rotation >= 360f) LoadingImage.Rotation = 0;
                    await LoadingImage.RotateTo(i * (360 / 6), 100, Easing.Linear);
                }
            }
        }
    }
}

using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using Acr.UserDialogs;
using FFImageLoading.Forms.Platform;
using Android.Content;
using Xamarin.Forms;
using mycoin.Views;
using Android;

namespace mycoin.Droid
{
    [Activity(Label = "BIYOND", Icon = "@mipmap/biyond_coin_top_grey", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        private readonly string[] Permissions =
        {
            Manifest.Permission.Bluetooth,
            Manifest.Permission.BluetoothAdmin,
            //Manifest.Permission.BluetoothPrivileged,
            "android.permission.BLUETOOTH_SCAN",
            //Manifest.Permission.AccessCoarseLocation,
            //Manifest.Permission.AccessFineLocation,
            //Manifest.Permission.LocationHardware,
        };
        protected override void OnCreate(Bundle savedInstanceState)
        {
            UserDialogs.Init(this);

            base.OnCreate(savedInstanceState);

            //For gif file animation
            CachedImageRenderer.Init(true);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            XF.Material.Droid.Material.Init(this, savedInstanceState);

            //Plugin.CurrentActivity.CrossCurrentActivity.Current.Init(this, savedInstanceState);

            //
            var myApp = new App();
            var mBundle = Intent.Extras;
            if (mBundle != null)
            {
                var pageName = mBundle.GetString("pageName");
                if (!string.IsNullOrEmpty(pageName))
                {
                    //get the assemblyQualifiedName of page
                    var pageAssemblyName = typeof(CalendarPage).AssemblyQualifiedName + "." + pageName + "," + typeof(CalendarPage).AssemblyQualifiedName;
                    Type type = Type.GetType(pageAssemblyName);
                    if (type != null)
                    {
                        var currentPage = Activator.CreateInstance(type);
                        //set the main page
                        myApp.MainPage = new NavigationPage((Page)currentPage);
                    }
                    else myApp.MainPage = new NavigationPage(new LoginPage());

                }
                else myApp.MainPage = new NavigationPage(new LoginPage());
            }
            else myApp.MainPage = new NavigationPage(new LoginPage());
            //

            CheckPermissions();
            //LoadApplication(new App()); 
            LoadApplication(myApp);

            MessagingCenter.Subscribe<EventArgs, string>(this, "Brightness", (sender, args) =>
            {
                Window.Attributes.ScreenBrightness = (float)Convert.ToDouble(args);
            });
        }

        private void CheckPermissions()
        {
            bool minimumPermissionsGranted = true;

            foreach (string permission in Permissions)
            {
                if (CheckSelfPermission(permission) != Permission.Granted)
                {
                    minimumPermissionsGranted = false;
                }
            }

            // If any of the minimum permissions aren't granted, we request them from the user
            if (!minimumPermissionsGranted)
            {
                RequestPermissions(Permissions, 0);
            }
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        //
        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);
        }
    }
}

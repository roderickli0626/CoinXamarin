using Android.Animation;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Android.OS.Build;

namespace mycoin.Droid
{
    [Activity(Label = "BIYOND", Icon = "@mipmap/biyond_coin_top_grey", Theme = "@style/Theme.Splash", MainLauncher = true, NoHistory = true)]
    public class SplashActivity : Activity
    {
        System.Timers.Timer timer;
        TextView percentView;
        ProgressBar progressBar;
        int count = 0;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.SplashLayout);
            System.Threading.ThreadPool.QueueUserWorkItem(o => LoadActivity());
            // Create your application here
            InitPercentValue();
        }
        async private void InitPercentValue()
        {
            //progressbar Percent
            percentView = FindViewById<TextView>(Resource.Id.percent);
            timer = new System.Timers.Timer(50);
            timer.AutoReset= true;
            timer.Elapsed += Timer_Elapsed;
            //
        }

        private void Timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            percentView.Text = progressBar.Progress + "%";
        }

        private void LoadActivity()
        {
            System.Threading.Thread.Sleep(3000); // Simulate a long pause    
            RunOnUiThread(() => StartActivity(typeof(MainActivity)));
        }

        public override void OnWindowFocusChanged(bool hasFocus)
        {
            progressBar = FindViewById<ProgressBar>(Resource.Id.progressBar);
            ObjectAnimator progressAnimator = ObjectAnimator.OfInt(progressBar, "progress", 0, 100);
            progressAnimator.SetDuration(3000);
            progressAnimator.SetInterpolator(new LinearInterpolator());
            progressAnimator.Start();
            //
            if (timer.Enabled) timer.Stop();
            else timer.Start();
            //
            //Version
            var context = Android.App.Application.Context;
            var info = context.PackageManager.GetPackageInfo(context.PackageName, 0);
            var vers = info.VersionName;
            TextView versionView = FindViewById<TextView>(Resource.Id.version);
            versionView.Text = "VERSION : " + vers;
            //
            
            ImageView imageView = FindViewById<ImageView>(Resource.Id.animated_loading);

            Android.Graphics.Drawables.AnimationDrawable animation = (Android.Graphics.Drawables.AnimationDrawable)imageView.Drawable;

            animation.Start();

            
        }
    }
}
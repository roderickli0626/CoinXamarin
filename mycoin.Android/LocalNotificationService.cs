using Android.App;
using Android.Content;
using Android.Media;
using Android.Support.V4.App;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
//using AndroidX.Core.App;
using Java.Lang;
using mycoin.DependencyServices;
using mycoin.Droid;
using mycoin.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using mycoin.Views;
using Xamarin.Forms;
using Application = Android.App.Application;
using mycoin.Extensions;
using mycoin.ViewModels;

[assembly: Xamarin.Forms.Dependency(typeof(LocalNotificationService))]

namespace mycoin.Droid

{

    public class LocalNotificationService : ILocalNotificationService
    {
        int _notificationIconId { get; set; }
        readonly DateTime _jan1st1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        internal string _randomNumber;

        public void LocalNotification(string title, string body, int id, DateTime notifyTime)
        {

            //long repeateDay = 1000 * 60 * 60 * 24;    
            long repeateForMinute = 10000; // In milliseconds   
            long totalMilliSeconds = (long)(notifyTime.ToUniversalTime() - _jan1st1970).TotalMilliseconds;
            if (totalMilliSeconds < JavaSystem.CurrentTimeMillis())
            {
                return;
                //totalMilliSeconds = totalMilliSeconds + repeateForMinute;
            }

            var intent = CreateIntent(id);
            var localNotification = new LocalNotification();
            localNotification.Title = title;
            localNotification.Body = body;
            localNotification.Id = id;
            localNotification.NotifyTime = notifyTime;

            if (_notificationIconId != 0)
            {
                localNotification.IconId = _notificationIconId;
            }
            else
            {
                //localNotification.IconId = Resource.Drawable.notificationgrey;
                localNotification.IconId = Resource.Drawable.notification_icon_background;
            }

            var serializedNotification = SerializeNotification(localNotification);
            intent.PutExtra(ScheduledAlarmHandler.LocalNotificationKey, serializedNotification);

            Random generator = new Random();
            _randomNumber = generator.Next(100000, 999999).ToString("D6");

            var pendingIntent = PendingIntent.GetBroadcast(Application.Context, Convert.ToInt32(_randomNumber), intent, PendingIntentFlags.Immutable);
            var alarmManager = GetAlarmManager();
            alarmManager.Set(AlarmType.RtcWakeup, totalMilliSeconds, pendingIntent);
            //alarmManager.SetRepeating(AlarmType.RtcWakeup, totalMilliSeconds, repeateForMinute, pendingIntent);
        }

        public void Cancel(int id)
        {

            var intent = CreateIntent(id);
            var pendingIntent = PendingIntent.GetBroadcast(Application.Context, Convert.ToInt32(_randomNumber), intent, PendingIntentFlags.Immutable);
            var alarmManager = GetAlarmManager();
            alarmManager.Cancel(pendingIntent);
            var notificationManager = NotificationManagerCompat.From(Application.Context);
            notificationManager.CancelAll();
            notificationManager.Cancel(id);
        }

        public static Intent GetLauncherActivity()
        {

            var packageName = Application.Context.PackageName;
            return Application.Context.PackageManager.GetLaunchIntentForPackage(packageName);
        }


        private Intent CreateIntent(int id)
        {

            return new Intent(Application.Context, typeof(ScheduledAlarmHandler))
                .SetAction("LocalNotifierIntent" + id);
        }

        private AlarmManager GetAlarmManager()
        {

            var alarmManager = Application.Context.GetSystemService(Context.AlarmService) as AlarmManager;
            return alarmManager;
        }

        private string SerializeNotification(LocalNotification notification)
        {

            var xmlSerializer = new XmlSerializer(notification.GetType());

            using (var stringWriter = new StringWriter())
            {
                xmlSerializer.Serialize(stringWriter, notification);
                return stringWriter.ToString();
            }
        }
    }

    [BroadcastReceiver(Enabled = true, Label = "Local Notifications Broadcast Receiver")]
    public class ScheduledAlarmHandler : BroadcastReceiver
    {

        public const string LocalNotificationKey = "LocalNotification";

        //Variables for Calendar Music
        public Plugin.SimpleAudioPlayer.ISimpleAudioPlayer audio;
        public int timerCount = 0;
        public bool endFlag = false;
        public bool closeFlag = false;
        public bool stopFlag = false;

        public ScheduledAlarmHandler()
        {
            MessagingCenter.Subscribe<CalendarPageViewModel>(this, "Music Stop(Close)", (sender) =>
            {
                //Calendar Music Stop(Close) handle
                audio.Stop();

                //Send Message to notice Music End
                MessagingCenter.Send<ILocalNotificationService>(new LocalNotificationService(), "Music End");
                //Set GlobalConstants to notice End Music
                GlobalConstants.RunFlag = false;

                stopFlag = true;
                closeFlag = true;
            });

            //MessagingCenter.Subscribe<EventArgs, string>(this, "Music Stop(Close)", (sender, args) =>
            //{

            //});

            //MessagingCenter.Subscribe<object>(this, "Music Resume", (sender) =>
            //{
            //    //Calendar Music Resume handle
            //    stopFlag = false;
            //});
        }
        public override void OnReceive(Context context, Intent intent)
        {
            var extra = intent.GetStringExtra(LocalNotificationKey);
            var notification = DeserializeNotification(extra);

            //Generating notification    
            var builder = new NotificationCompat.Builder(Application.Context)
                .SetContentTitle(notification.Title)
                .SetContentText(notification.Body)
                .SetSmallIcon(notification.IconId)
                .SetSound(RingtoneManager.GetDefaultUri(RingtoneType.Ringtone))
                .SetAutoCancel(true);

            var resultIntent = LocalNotificationService.GetLauncherActivity();
            resultIntent.SetFlags(ActivityFlags.NewTask | ActivityFlags.ClearTask);

            //
            //Navigate to CalendarPage When Notification click
            Bundle bundle = new Bundle();
            bundle.PutString("pageName", "CalendarPage");
            resultIntent.PutExtras(bundle);
            //

            var stackBuilder = Android.Support.V4.App.TaskStackBuilder.Create(Application.Context);
            stackBuilder.AddNextIntent(resultIntent);

            Random random = new Random();
            int randomNumber = random.Next(9999 - 1000) + 1000;

            var resultPendingIntent =
                stackBuilder.GetPendingIntent(randomNumber, (int)PendingIntentFlags.Immutable);
            builder.SetContentIntent(resultPendingIntent);
            // Sending notification    
            var notificationManager = NotificationManagerCompat.From(Application.Context);
            notificationManager.Notify(randomNumber, builder.Build());


            //For Calendar Music Play
            //Play Music
            Calendar calendar = App.Database.GetCalendarAsync(notification.Id).Result;
            InitAudioPlayer();
            if (audio == null || calendar?.WavFile == null) return;
            audio.Load(new MemoryStream(calendar.WavFile));
            audio.Play();
            timerCount = 0;
            closeFlag = false;
            stopFlag = false;

            //Send Message to notice Calendar Music Start
            MessagingCenter.Send<ILocalNotificationService>(new LocalNotificationService(), "Music Start");
            //Set GlobalConstants to notice Start Music
            GlobalConstants.RunFlag = true;
            GlobalConstants.Duration = calendar.Duration;
            GlobalConstants.StartTime = calendar.startTime;

            Device.StartTimer(TimeSpan.FromSeconds(0.5), () =>
            {
                if (closeFlag) return false;
                if (stopFlag) return true;
                else timerCount++;
                double total = timerCount * 0.5;
                if (calendar.Duration * 60 > total)
                {
                    if (endFlag)
                    {
                        audio.Play();
                        endFlag = false;
                    }
                    return true;
                }
                else
                {
                    audio.Stop();
                    //Send Message to notice Music End
                    MessagingCenter.Send<ILocalNotificationService>(new LocalNotificationService(), "Music End");
                    //Set GlobalConstants to notice End Music
                    GlobalConstants.RunFlag = false;
                    return false;
                }
            });

        }
        void InitAudioPlayer()
        {
            audio = Plugin.SimpleAudioPlayer.CrossSimpleAudioPlayer.Current;
            audio.Loop = true;
            audio.PlaybackEnded += EndAudioPlaying;
        }
        void EndAudioPlaying(object sender, EventArgs e)
        {
            endFlag = true;
        }
        private LocalNotification DeserializeNotification(string notificationString)
        {
            var xmlSerializer = new XmlSerializer(typeof(LocalNotification));
            using (var stringReader = new StringReader(notificationString))
            {
                var notification = (LocalNotification)xmlSerializer.Deserialize(stringReader);
                return notification;
            }
        }
    }
}


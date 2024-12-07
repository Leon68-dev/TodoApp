using Android.App;
using Android.Runtime;
using Plugin.LocalNotification;
using Plugin.LocalNotification.AndroidOption;

namespace TodoApp
{
    [Application]
    public class MainApplication : MauiApplication
    {
        public MainApplication(IntPtr handle, JniHandleOwnership ownership)
            : base(handle, ownership)
        {
        }

        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

        public override void OnCreate()
        {
            base.OnCreate();

            //var channel = new NotificationChannelRequest
            //{
            //    Id = "default_channel",
            //    Name = "Default Notifications",
            //    Description = "Notifications for task reminders",
            //    Importance = AndroidImportance.High
            //};

            //LocalNotificationCenter.CreateNotificationChannel();
        }

    }
}

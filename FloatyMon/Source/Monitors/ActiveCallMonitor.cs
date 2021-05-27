using System.Linq;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Telephony;
using FloatyMon.Source.Extras;

namespace FloatyMon.Source.Monitors
{
    public interface ICallActivity
    {
        public void CallStateChanged(CallState CallState, string incomingNumber);
    }

    public class ActiveCallMonitor : ICallActivity
    {
        private PhoneActivityListener PhoneActivityListener;

        readonly ICallActivity CallMonDelegate;

        bool didFirstLaunch = false; //First state is always Idle, so use this to igmore first state

        public ActiveCallMonitor()
        {
            CallMonDelegate = this;
        }

        public void StartListener()
        {
            var AppContext = Application.Context;
            PhoneActivityListener = new PhoneActivityListener(CallMonDelegate);

            TelephonyManager telephonyManager = (TelephonyManager)AppContext.GetSystemService(Context.TelephonyService);
            telephonyManager.Listen(PhoneActivityListener, PhoneStateListenerFlags.CallState);
        }

        public void StopListening()
        {
            var AppContext = Application.Context;
            TelephonyManager telephonyManager = (TelephonyManager)AppContext.GetSystemService(Context.TelephonyService);
            telephonyManager.Listen(PhoneActivityListener, PhoneStateListenerFlags.None);
        }

        public void CallStateChanged(CallState CallState, string incomingNumber)
        {
            System.Diagnostics.Debug.WriteLine("Getting ---> " + incomingNumber);
            var AppContext = Application.Context;

            if (!didFirstLaunch)
            {
                didFirstLaunch = true;
                return;
            }

            LocalIntentHelper.SendLocalIntent(AppContext,
                Constants.FLOATY_LOCAL_INTENT,
                Constants.FLOATY_SETTEXT_KEY, incomingNumber);

            bool makeVisible = true;
            LocalIntentHelper.SendLocalIntent(AppContext,
                Constants.FLOATY_LOCAL_INTENT,
                Constants.FLOATY_MAKEVISIBLE_KEY, makeVisible);

            ShowANotification(incomingNumber);
        }

        void ShowANotification(string NotificationMessage)
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.O) return;
            var appCtx = Application.Context;
            var Manager = (NotificationManager)appCtx.GetSystemService(Context.NotificationService);
            var NotificationCounter = Manager.GetActiveNotifications().Count() + 1;

            Intent intent = new Intent(appCtx, typeof(MainActivity));
            intent.SetAction(Intent.ActionMain);
            intent.AddCategory(Intent.CategoryLauncher);
            intent.PutExtra(Constants.FLOATY_NOTIFICATION_INTENT_PASSED_ACTIIVITY_KEY, NotificationMessage);

            var CHANNEL_ID = Constants.FLOATY_NOTIFICATION_CHANNEL_ID_ACTIVITIES_KEY;
            var notification = new Notification.Builder(appCtx, CHANNEL_ID)
                .SetGroup("Activities")
                .SetGroupAlertBehavior(NotificationGroupAlertBehavior.Summary)
                .SetContentTitle(NotificationMessage)
                .SetSubText("Activity")
                .SetContentText("Added following behaviours")
                .SetStyle(new Notification.InboxStyle()
                .AddLine("Tap to remove")
                .AddLine("Swipe to remove"))
                .SetSmallIcon(Resource.Drawable.abc_ic_star_black_16dp)
                .SetAutoCancel(true)
                .SetContentIntent(PendingIntent.GetActivity(appCtx, 0, intent, PendingIntentFlags.UpdateCurrent)) //Launches the app
                //.SetContentIntent(PendingIntent.GetActivity(appCtx, 0, intent, PendingIntentFlags.Immutable)) //Just Removes the notification
                .Build();
            
            Manager.Notify(NotificationCounter, notification);
        }
    }
}
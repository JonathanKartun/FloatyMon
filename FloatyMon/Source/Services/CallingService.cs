using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using FloatyMon;
using FloatyMon.Source;
using FloatyMon.Source.Monitors;
using Java.Lang;

[Service(Name = "com.jon.FloatyMon.JcallingService")]
public class CallingService : Service
{
    public IBinder Binder { get; private set; }
    private ActiveCallMonitor CallMonitor;
    private PowerManager.WakeLock wakelock;

    #region Service Lifecycle
    public override void OnCreate()
    {
        base.OnCreate();
        Log.Debug("JCallingService", "ON_CREATE");

        if (Build.VERSION.SdkInt >= BuildVersionCodes.O) {
            NotiUpdate();
            CreateActivitiesNotificationChannel();
        }
    }

    void NotiUpdate()
    {
        var CHANNEL_ID = Constants.FLOATY_NOTIFICATION_CHANNEL_ID_MAIN_SERVICE_KEY;
        ICharSequence NotificationName = new String("This is Floaty! - Call");
        NotificationChannel Channel = new NotificationChannel(CHANNEL_ID, NotificationName, NotificationImportance.Default);
        var Manager = (NotificationManager)GetSystemService(NotificationService);
        Manager.CreateNotificationChannel(Channel);

        Intent intent = new Intent(this, typeof(MainActivity));
        PendingIntent pendingIntent = PendingIntent.GetActivity(this, 1, intent, PendingIntentFlags.Immutable);
        Notification.Action action = new Notification.Action.Builder(Resource.Drawable.abc_ic_star_black_16dp, "Open", pendingIntent).Build();

        var notification = new Notification.Builder(ApplicationContext, CHANNEL_ID)
                     .SetContentTitle("Floaty")
                     .SetSubText("Just waiting for a mate")
                     .SetContentText("Floating Caller Awaiting Incoming Call")
                     .SetSmallIcon(Resource.Drawable.abc_ic_star_black_16dp)
                     .SetAutoCancel(true)
                     .AddAction(action)
                     .SetVisibility(NotificationVisibility.Public)
                     .Build();

        StartForeground(1, notification);
    }

    void CreateActivitiesNotificationChannel()
    {
        //The following is to create a second NotificationChannel which would get added to from ActiveCallMonitor
        var CHANNEL_ID = Constants.FLOATY_NOTIFICATION_CHANNEL_ID_ACTIVITIES_KEY;
        var Manager = (NotificationManager)GetSystemService(NotificationService);
        ICharSequence NotificationName = new String("Activity");
        NotificationChannel Channel2 = new NotificationChannel(CHANNEL_ID, NotificationName, NotificationImportance.Default);
        Channel2.EnableLights(true);
        Channel2.EnableVibration(true);
        Manager.CreateNotificationChannel(Channel2);
    }

    public override IBinder OnBind(Intent intent)
    {
        Log.Debug("JCallingService", "ON_BIND");
        Binder = new CallingServiceBinder(this);

        InitializeCallListener();

        return Binder;
    }

    public override bool OnUnbind(Intent intent)
    {
        Log.Debug("JCallingService", "ON_UN_BIND");
        return base.OnUnbind(intent);
    }

    public override ComponentName StartForegroundService(Intent service)
    {
        return base.StartForegroundService(service);
    }

    [return: GeneratedEnum]
    public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
    {
        Log.Debug("JCallingService", "JCallingService started");
        ActivateKeepServiceAlive();

        return StartCommandResult.Sticky;
    }

    public override void OnDestroy()
    {
        Log.Debug("JCallingService", "JCallingService Destr0y3d!");
        DeactivateKeepServiceAlive();
        StopCallListener();
        base.OnDestroy();
    }

    #endregion

    public void InitializeCallListener()
    {
        if (CallMonitor == null)
        {
            CallMonitor = new ActiveCallMonitor();
        }

        CallMonitor.StartListener();
    }

    public void StopCallListener()
    {
        if (CallMonitor != null)
        {
            CallMonitor.StopListening();
        }
    }

    #region Power Manager Handler

    void ActivateKeepServiceAlive()
    {
        PowerManager pmanager = GetSystemService(Context.PowerService) as PowerManager;
        wakelock = pmanager.NewWakeLock(WakeLockFlags.Partial, "JCallService::lock");
        wakelock.SetReferenceCounted(false);
        wakelock.Acquire();
    }

    void DeactivateKeepServiceAlive()
    {
        if (wakelock != null)
        {
            if (wakelock.IsHeld)
            {
                wakelock.Release();
            }
        }
    }

    #endregion
}

public class CallingServiceBinder : Binder
{
    public CallingService Service { get; private set; }
    public MainActivity TheMainActivity { get; set; }

    public CallingServiceBinder(CallingService service)
    {
        Service = service;
    }

    private void InitializeCallListener()
    {
        Service.InitializeCallListener();
    }

    private void StopCallListener()
    {
        Service.StopCallListener();
    }
}
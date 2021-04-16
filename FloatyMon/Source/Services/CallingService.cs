using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using FloatyMon;
using FloatyMon.Source.Monitors;
using Java.Lang;

[Service]
public class CallingService : Service
{
    public IBinder Binder { get; private set; }
    private ActiveCallMonitor CallMonitor;

    #region Service Lifecycle
    public override void OnCreate()
    {
        base.OnCreate();
        Log.Debug("JCallingService", "ON_CREATE");

        if (Build.VERSION.SdkInt >= BuildVersionCodes.O) {
            NotiUpdate();
        }
    }

    void NotiUpdate()
    {
        if (Build.VERSION.SdkInt >= BuildVersionCodes.O) { }
        var CHANNEL_ID = "CallingServiceChannel";
        ICharSequence NotificationName = new String("This is Floaty! - Call");
        NotificationChannel channel = new NotificationChannel(CHANNEL_ID, NotificationName, NotificationImportance.Default);
        var manager = (NotificationManager)GetSystemService(NotificationService);
        manager.CreateNotificationChannel(channel);

        Intent intent = new Intent(this, typeof(MainActivity));
        PendingIntent pendingIntent = PendingIntent.GetActivity(this, 1, intent, PendingIntentFlags.OneShot);
        Notification.Action action = new Notification.Action.Builder(Resource.Drawable.abc_ic_star_black_16dp, "Open", pendingIntent).Build();

        var notification = new Notification.Builder(ApplicationContext, CHANNEL_ID)
                     .SetContentTitle("Floaty")
                     .SetContentText("Floating Caller Awaiting Incoming Call")
                     .SetSmallIcon(Resource.Drawable.abc_ic_star_black_16dp)
                     .SetAutoCancel(true)
                     .AddAction(action)
                     .Build();

        StartForeground(1, notification);
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

        return StartCommandResult.Sticky;
    }

    public override void OnDestroy()
    {
        Log.Debug("JCallingService", "JCallingService Destr0y3d!");
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
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Util;
using FloatyMon;
using FloatyMon.Source.Monitors;

[Service(Name = "com.jon.FloatyMon.JcallingService")]
public class CallingService : Service
{
    public IBinder Binder { get; private set; }
    private ActiveCallMonitor CallMonitor;

    #region Service Lifecycle
    public override void OnCreate()
    {
        base.OnCreate();
        StartForeground();
        Log.Debug("JCallingService", "ON_CREATE");
    }

    [System.Obsolete]
    private void StartForeground()
    {
        string channelid = "com.jon.FloatyMon.Urgent";
        var importance = NotificationImportance.High;
        NotificationChannel Channel = new NotificationChannel("com.jon.FloatyMon.Urgent", "Urgent", importance);
        Channel.LockscreenVisibility = NotificationVisibility.Public;
        NotificationManager notificationManager =
        (NotificationManager)GetSystemService(NotificationService);
        notificationManager.CreateNotificationChannel(Channel);

        var notificationBuilder = new NotificationCompat.Builder(this, channelid);
        Notification.Builder builder = new Notification.Builder(this)
                    .SetContentTitle("Attention!")
                    .SetContentText("This is an urgent notification message")
                    .SetChannelId(channelid);
        Notification not = builder.Build();
        StartForeground(2, not);
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

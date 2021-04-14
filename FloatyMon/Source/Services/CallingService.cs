using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using FloatyMon;
using FloatyMon.Source.Monitors;

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
    }

    public override IBinder OnBind(Intent intent)
    {
        Log.Debug("JCallingService", "ON_BIND");
        Binder = new CallingServiceBinder(this);

        InitializeJonCallListener();

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
        //StopJonCallListener();
        base.OnDestroy();
    }

    #endregion

    public void InitializeJonCallListener()
    {
        if (CallMonitor == null)
        {
            CallMonitor = new ActiveCallMonitor();
        }

        CallMonitor.StartListener();
    }

    public void StopJonCallListener()
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

    private void InitializeJonCallListener()
    {
        Service.InitializeJonCallListener();
    }

    private void StopJonCallListener()
    {
        Service.StopJonCallListener();
    }
}
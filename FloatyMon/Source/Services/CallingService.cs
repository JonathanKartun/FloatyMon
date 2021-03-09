//import android.annotation.TargetApi;
//import android.app.Notification;
//import android.app.NotificationManager;
//import android.app.PendingIntent;
//import android.app.Service;
//import android.content.Intent;
//import android.os.Binder;
//import android.os.Build;
//import android.os.IBinder;
//import android.util.Log;
//import android.widget.Toast;

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
    private JonCallMonitor jonCall;

    #region Service Lifecycle
    public override void OnCreate()
    {
        base.OnCreate();
        Log.Debug("JCallingService", "ON_CREATE");
    }

    public override IBinder OnBind(Intent intent)
    {
        Log.Debug("JCallingService", "ON_BIND");
        this.Binder = new CallingServiceBinder(this);

        InitializeJonCallListener();

        return this.Binder;
    }

    public override bool OnUnbind(Intent intent)
    {
        Log.Debug("JCallingService", "ON_UN_BIND");
        return base.OnUnbind(intent);
    }

    [return: GeneratedEnum]
    public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
    {
        ///
        //if (intent.Action == "START")
        
        // This method executes on the main thread of the application.
        Log.Debug("JCallingService", "JCallingService started");

        //return base.OnStartCommand(intent, flags, startId);
        return StartCommandResult.Sticky;
    }

    public override void OnDestroy()
    {
        Log.Debug("JCallingService", "JCallingService Destr0y3d!");
        StopJonCallListener();
        base.OnDestroy();
    }

    #endregion

    public void InitializeJonCallListener()
    {
        if (jonCall == null)
        {
            jonCall = new JonCallMonitor((Binder as CallingServiceBinder).TheMainActivity); //???
        }

        jonCall.StartListener();
    }

    public void StopJonCallListener()
    {
        if (jonCall != null)
        {
            jonCall.StopListening();
        }
    }
}

public class CallingServiceBinder : Binder
{
    public CallingService Service { get; private set; }
    public MainActivity TheMainActivity { get; set; }

    public CallingServiceBinder(CallingService service)
    {
        this.Service = service;
    }

    private void InitializeJonCallListener()
    {
        this.Service.InitializeJonCallListener();
    }

    private void StopJonCallListener()
    {
        this.Service.StopJonCallListener();
    }
}
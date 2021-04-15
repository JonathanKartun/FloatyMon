using System;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using FloatyMon.Source.Services;

namespace FloatyMon.Source
{
    public class MainActivityServiceManager
    {
        MainActivity mainActivity;
        CallingServiceServiceConnection serviceConnection;

        public MainActivityServiceManager(MainActivity mainActivity)
        {
            this.mainActivity = mainActivity;
        }

        public void LaunchCallingServiceListener()
        {
            if (serviceConnection == null)
            {
                serviceConnection = new CallingServiceServiceConnection();
            }

            Context context = Application.Context;
            Intent serviceToStart = new Intent(context, typeof(CallingService));
            context.StartService(serviceToStart);
            context.BindService(serviceToStart, serviceConnection, Bind.AutoCreate);
        }

        public void LaunchFloatingWindowService()
        {
            var AppContext = Application.Context;
            Intent svc = new Intent(AppContext, typeof(FloatingWidgetService));

            AppContext.StopService(svc);
            AppContext.StartService(svc);
            AppContext.BindService(svc, serviceConnection, Bind.AutoCreate);
        }

        private void StopFloatingWindowService()
        {
            var AppContext = Application.Context;
            Intent svc = new Intent(AppContext, typeof(FloatingWidgetService));

            AppContext.StopService(svc);
        }

        public void CheckDrawOverlayPermission()
        {
            var AppContext = Application.Context;
            if (Android.Provider.Settings.CanDrawOverlays(AppContext))
            {
                LaunchFloatingWindowService();
            }
            else
            {   // Intent to launch the permission request
                Intent intent = new Intent(Android.Provider.Settings.ActionManageOverlayPermission, Android.Net.Uri.Parse("package:" + mainActivity.PackageName));
                mainActivity.StartActivityForResult(intent, Constants.OVERLAY_REQUEST_CODE);
            }
        }

        //Needs this permission check to access the Phone Number calling. Otherwise it's always blank. (But can still get calling states even if denied)
        public void CheckCallReadPermission()
        {
            var AppContext = Application.Context;
            string[] NECESSARY_PERMISSIONS = new string[] { Manifest.Permission.ReadCallLog };

            if (ContextCompat.CheckSelfPermission(AppContext, Manifest.Permission.ReadCallLog) == Permission.Granted)
            {
                LaunchCallingServiceListener(); //Launches the Phone call Listening Service
            }
            else
            {
                ActivityCompat.RequestPermissions(mainActivity, NECESSARY_PERMISSIONS, Constants.PHONE_ACCESS_REQUEST_CODE);
            }
        }

        [Obsolete]
        public bool IsServiceRunning<T>(Context context)
        {
            ActivityManager actMan = (ActivityManager)context.GetSystemService(Context.ActivityService);
            var services = actMan.GetRunningServices(int.MaxValue);
            
            foreach (var service in services)
            {
                if (service.Service.ClassName.EndsWith(typeof(T).Name))
                {
                    return true;
                }
            }
            return false;
        }
    }
}

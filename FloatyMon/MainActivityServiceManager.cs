using System;
using System.Collections.Generic;
using Android;
using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.Content.PM;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Support.V7.App;
using FloatyMon.Source.Services;
using Java.Lang;

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
                this.serviceConnection = new CallingServiceServiceConnection(mainActivity);
            }
            
            Intent serviceToStart = new Intent(mainActivity, typeof(CallingService));
            mainActivity.BindService(serviceToStart, this.serviceConnection, Bind.AutoCreate);
        }

        //public void RelaunchCallingServiceListener()
        //{
        //    if (this.serviceConnection != null) {
        //        this.serviceConnection.Dispose();
        //        serviceConnection = null;
        //    }
        //    LaunchCallingServiceListener();
        //}

        public void CheckFloatAllowedAndLaunchFloatingWindow()
        {
            if (Android.Provider.Settings.CanDrawOverlays(mainActivity))
            {// Launch service right away - the user has already previously granted permission
                LaunchFloatingWindowService();
            }
            else
            {
                // Check that the user has granted permission, and prompt them if not
                CheckDrawOverlayPermission();
            }
        }

        public void LaunchFloatingWindowService()
        {
            var AppContext = Android.App.Application.Context;
            Intent svc = new Intent(AppContext, typeof(FloatingWidgetService));

            AppContext.StopService(svc);
            AppContext.StartService(svc);

            //mainActivity.Finish(); //Without this, it asks again...
        }

        private void StopFloatingWindowService()
        {
            var AppContext = Android.App.Application.Context;
            Intent svc = new Intent(AppContext, typeof(FloatingWidgetService));

            AppContext.StopService(svc);
        }

        public void CheckDrawOverlayPermission()
        {
            var AppContext = Android.App.Application.Context;
            if (!Android.Provider.Settings.CanDrawOverlays(AppContext)) //this
            {
                // If not, Intent to launch the permission request
                Intent intent = new Intent(Android.Provider.Settings.ActionManageOverlayPermission, Android.Net.Uri.Parse("package:" + mainActivity.PackageName));

                // Launch Intent, with the supplied request code
                mainActivity.StartActivityForResult(intent, Constants.OVERLAY_REQUEST_CODE);
            }
            else
            {
                LaunchFloatingWindowService();
            }
        }

        //Needs this permission check to access the Phone Number calling. Otherwise it's always blank. (But can still get calling states even if denied)
        public void CheckCallReadPermission()
        {
            var AppContext = Android.App.Application.Context;
            string[] NECESSARY_PERMISSIONS = new string[] { Android.Manifest.Permission.ReadCallLog };

            if (ContextCompat.CheckSelfPermission(AppContext, Manifest.Permission.ReadCallLog) == Permission.Granted)
            {
                //Permission is granted
            }
            else
            {
                ActivityCompat.RequestPermissions(mainActivity, NECESSARY_PERMISSIONS, Constants.PHONE_ACCESS_REQUEST_CODE);
            }
        }

        [Obsolete]
        public bool isServiceRunning<T>(Context context)
        {
            ActivityManager actMan = (ActivityManager)context.GetSystemService(Context.ActivityService);
            var services = actMan.GetRunningServices(int.MaxValue);
            
            foreach (var service in services)
            {
                //System.Diagnostics.Debug.WriteLine($"Serv = {service.Service.ClassName} -> My Compare = {typeof(T).Name}");   
                if (service.Service.ClassName.EndsWith(typeof(T).Name))
                {
                    return true;
                }
            }
            return false;
        }

        public static Context AppContext()
        {
            return Android.App.Application.Context;
        }
    }
}

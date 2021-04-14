using Android.Content;
using Android.OS;
using Android.Util;

namespace FloatyMon.Source.Services
{
    public class CallingServiceServiceConnection: Java.Lang.Object, IServiceConnection
    {
        static readonly string TAG = typeof(CallingServiceServiceConnection).FullName;
        public bool IsConnected { get; private set; }
        public CallingServiceBinder Binder { get; private set; }

        public CallingServiceServiceConnection()
        {
            IsConnected = false;
            Binder = null;
        }

        public void OnServiceConnected(ComponentName name, IBinder service)
        {
            Binder = service as CallingServiceBinder;
            IsConnected = Binder != null;

            string message = "onServiceConnected - ";
            Log.Debug(TAG, $"OnServiceConnected {name.ClassName}");

            if (IsConnected)
            {
                message = message + " bound to service " + name.ClassName;
            }
            else
            {
                message = message + " not bound to service " + name.ClassName;
            }

            Log.Info(TAG, message);
        }

        public void OnServiceDisconnected(ComponentName name)
        {
            Log.Debug(TAG, $"OnServiceDisconnected {name.ClassName}");
            IsConnected = false;
            Binder = null;
        }
    }
}

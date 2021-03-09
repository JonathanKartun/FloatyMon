using System;
using Android.Content;
using Android.OS;
using Android.Util;

namespace FloatyMon.Source.Services
{
    public class CallingServiceServiceConnection: Java.Lang.Object, IServiceConnection
    {
        static readonly string TAG = typeof(CallingServiceServiceConnection).FullName;
        MainActivity mainActivity;
        public bool IsConnected { get; private set; }
        public CallingServiceBinder Binder { get; private set; }

        public CallingServiceServiceConnection(MainActivity activity)
        {
            IsConnected = false;
            Binder = null;
            mainActivity = activity;
        }

        public void OnServiceConnected(ComponentName name, IBinder service)
        {
            Binder = service as CallingServiceBinder;
            Binder.TheMainActivity = mainActivity; //??
            IsConnected = this.Binder != null;

            string message = "onServiceConnected - ";
            Log.Debug(TAG, $"OnServiceConnected {name.ClassName}");

            if (IsConnected)
            {
                message = message + " bound to service " + name.ClassName;
                //mainActivity.UpdateUiForBoundService();
                //mainActivity.SetFloatingWindowText("Bombabomba");
            }
            else
            {
                message = message + " not bound to service " + name.ClassName;
                //mainActivity.UpdateUiForUnboundService();
            }

            Log.Info(TAG, message);
            //mainActivity.timestampMessageTextView.Text = message;
        }

        public void OnServiceDisconnected(ComponentName name)
        {
            Log.Debug(TAG, $"OnServiceDisconnected {name.ClassName}");
            IsConnected = false;
            Binder = null;
            //mainActivity.UpdateUiForUnboundService();
        }

        public string DoSomethingSomething()
        {
            if (!IsConnected)
            {
                return null;
            }

            return "I am the best from the rest...";
        }
    }
}

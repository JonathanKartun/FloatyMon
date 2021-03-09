using System;
using Android.Content;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Telephony;
using Android.Util;
using FloatyMon;
using FloatyMon.Source.Extras;
using FloatyMon.Source.Monitors;
using static FloatingWidgetService;

namespace FloatyMon.Source.Monitors
{
    public interface IJonCallActivity
    {
        public void JonCallStateChanged(CallState CallState, String incomingNumber);
    }

    public class JonCallMonitor: IJonCallActivity
    {
        private readonly MainActivity _activity;
        private PhoneActivityListener phoneActivityListener;

        IJonCallActivity jCallMonDelegate;

        bool didFirstLaunch = false; //First state is always Idle, so use this to igmore first state

        public JonCallMonitor(MainActivity activity)
        {
            _activity = activity;
            setDelegate(this);
        }

        private void setDelegate(JonCallMonitor jonCallMonitor)
        {
            jCallMonDelegate = jonCallMonitor;
        }

        public void StartListener()
        {
            var AppContext = Android.App.Application.Context;
            phoneActivityListener = new PhoneActivityListener(_activity, jCallMonDelegate);

            TelephonyManager telephonyManager = (TelephonyManager)AppContext.GetSystemService(Context.TelephonyService);
            telephonyManager.Listen(phoneActivityListener, PhoneStateListenerFlags.CallState);

            //telephonyManager.Listen(phoneActivityListener, PhoneStateListenerFlags.CallState |
            //    PhoneStateListenerFlags.SignalStrength | PhoneStateListenerFlags.DataActivity |
            //    PhoneStateListenerFlags.CellLocation | PhoneStateListenerFlags.CallForwardingIndicator |
            //    PhoneStateListenerFlags.DataConnectionState | PhoneStateListenerFlags.ServiceState);
        }

        public void StopListening()
        {
            TelephonyManager telephonyManager = (TelephonyManager)_activity.GetSystemService(Context.TelephonyService);
            telephonyManager.Listen(phoneActivityListener, PhoneStateListenerFlags.None);
        }

        public void JonCallStateChanged(CallState CallState, string incomingNumber)
        {
            System.Diagnostics.Debug.WriteLine("Getting ---> " + incomingNumber);
            var AppContext = Android.App.Application.Context;

            //Intent intent = new Intent("localocal");
            //intent.PutExtra("igotthis", incomingNumber);
            //LocalBroadcastManager.GetInstance(AppContext).SendBroadcast(intent);

            //Intent BroadcastIntent = new Intent(AppContext, typeof(LocaLocalBroadcastReceiver));
            //        BroadcastIntent.SetAction("igotthis");
            //        //BroadcastIntent.PutExtra("igotthis", incomingNumber);
            //        BroadcastIntent.AddCategory(Intent.CategoryDefault);
            //        //SendBroadcast(BroadcastIntent);
            //LocalBroadcastManager.GetInstance(AppContext).SendBroadcast(BroadcastIntent);

            //Intent message = new Intent("com.jon.floatymon.cally");
            //// If desired, pass some values to the broadcast receiver.
            //message.PutExtra("igotthis", incomingNumber);
            //Android.Support.V4.Content.LocalBroadcastManager.GetInstance(AppContext).SendBroadcast(message);

            if (!didFirstLaunch)
            {
                didFirstLaunch = true;
                return;
            }

            //Set Text
            LocalIntentHelper.SendLocalIntent(AppContext,
                Constants.FLOATY_LOCAL_INTENT,
                Constants.FLOATY_SETTEXT_KEY, incomingNumber);

            //Make it Visible
            bool makeVisible = true;
            LocalIntentHelper.SendLocalIntent(AppContext,
                Constants.FLOATY_LOCAL_INTENT,
                Constants.FLOATY_MAKEVISIBLE_KEY, makeVisible);


        }
    }
}

    class PhoneActivityListener: PhoneStateListener
    {
        public static String callCurrentStateResponse = "";
        public static String lastCallNumber = "";

        private readonly MainActivity _activity;
        private IJonCallActivity _jCallMonDelegate;

    public PhoneActivityListener(MainActivity activity, IJonCallActivity jCallMonDelegate)
        {
            _activity = activity;
            _jCallMonDelegate = jCallMonDelegate;
        }

        public override void OnCallStateChanged([GeneratedEnum] CallState state, string phoneNumber)
        {
            base.OnCallStateChanged(state, phoneNumber);

            string callNum = (phoneNumber.Length > 0 ? phoneNumber : "PRIVATE");

            if (!lastCallNumber.Equals(callNum))
            {
                lastCallNumber = callNum;
            }

        //_jCallMonDelegate.JonCallStateChanged(state, lastCallNumber);
        switch (state)
            {
                case CallState.Idle:
                    callCurrentStateResponse = "IDLE";
                    //StopMainService();
                    break;
                case CallState.Ringing:
                    //LaunchMainService();
                    callCurrentStateResponse = "Ringing (" + lastCallNumber + ") ";
                    break;
                case CallState.Offhook:
                    callCurrentStateResponse = "Offhook";
                    break;
            }

        Log.Debug("CALLMON", "STATE === " + callCurrentStateResponse);

        _jCallMonDelegate.JonCallStateChanged(state, callCurrentStateResponse);
    }

    //public override void OnCellInfoChanged(IList<CellInfo> cellInfo)
    //{
    //    base.OnCellInfoChanged(cellInfo);
    //}

    //public override void OnSignalStrengthsChanged(SignalStrength signalStrength)
    //{
    //    base.OnSignalStrengthsChanged(signalStrength);
    //}

        private void LaunchMainService()
        {
            var AppContext = Android.App.Application.Context;
            Intent svc = new Intent(AppContext, typeof(FloatingWidgetService));

            AppContext.StopService(svc);
            AppContext.StartService(svc);

            //Finish(); //Without this, it asks again...
        }

        private void StopMainService()
        {
            var AppContext = Android.App.Application.Context;
            Intent svc = new Intent(AppContext, typeof(FloatingWidgetService));

            AppContext.StopService(svc);
        }
    }

using System;
using Android.Content;
using Android.Telephony;
using FloatyMon.Source.Extras;

namespace FloatyMon.Source.Monitors
{
    public interface ICallActivity
    {
        public void CallStateChanged(CallState CallState, string incomingNumber);
    }

    public class ActiveCallMonitor : ICallActivity
    {
        private PhoneActivityListener PhoneActivityListener;

        readonly ICallActivity CallMonDelegate;

        bool didFirstLaunch = false; //First state is always Idle, so use this to igmore first state

        public ActiveCallMonitor()
        {
            CallMonDelegate = this;
        }

        public void StartListener()
        {
            var AppContext = Android.App.Application.Context;
            PhoneActivityListener = new PhoneActivityListener(CallMonDelegate);

            TelephonyManager telephonyManager = (TelephonyManager)AppContext.GetSystemService(Context.TelephonyService);
            telephonyManager.Listen(PhoneActivityListener, PhoneStateListenerFlags.CallState);
        }

        public void StopListening()
        {
            var AppContext = Android.App.Application.Context;
            TelephonyManager telephonyManager = (TelephonyManager)AppContext.GetSystemService(Context.TelephonyService);
            telephonyManager.Listen(PhoneActivityListener, PhoneStateListenerFlags.None);
        }

        public void CallStateChanged(CallState CallState, string incomingNumber)
        {
            System.Diagnostics.Debug.WriteLine("Getting ---> " + incomingNumber);
            var AppContext = Android.App.Application.Context;

            if (!didFirstLaunch)
            {
                didFirstLaunch = true;
                return;
            }

            LocalIntentHelper.SendLocalIntent(AppContext,
                Constants.FLOATY_LOCAL_INTENT,
                Constants.FLOATY_SETTEXT_KEY, incomingNumber);

            bool makeVisible = true;
            LocalIntentHelper.SendLocalIntent(AppContext,
                Constants.FLOATY_LOCAL_INTENT,
                Constants.FLOATY_MAKEVISIBLE_KEY, makeVisible);
        }
    }
}
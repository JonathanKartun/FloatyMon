using Android.Runtime;
using Android.Telephony;
using Android.Util;

namespace FloatyMon.Source.Monitors
{
    class PhoneActivityListener : PhoneStateListener
    {
        public static string CallCurrentStateResponse = "";
        public static string LastCalledNumber = "";

        readonly ICallActivity CallMonitorDelegate;

        public PhoneActivityListener(ICallActivity CallMonitorDelegate)
        {
            this.CallMonitorDelegate = CallMonitorDelegate;
        }

        public override void OnCallStateChanged([GeneratedEnum] CallState state, string phoneNumber)
        {
            base.OnCallStateChanged(state, phoneNumber);

            string CallingNumber = (phoneNumber.Length > 0 ? phoneNumber : "PRIVATE");

            if (!LastCalledNumber.Equals(CallingNumber))
            {
                LastCalledNumber = CallingNumber;
            }

            switch (state)
            {
                case CallState.Idle:
                    CallCurrentStateResponse = $"IDLE\nLastCall: {LastCalledNumber}";
                    break;
                case CallState.Ringing:
                    CallCurrentStateResponse = "Ringing (" + LastCalledNumber + ") ";
                    break;
                case CallState.Offhook:
                    CallCurrentStateResponse = "Offhook";
                    break;
            }

            Log.Debug("CALLMON", "STATE === " + CallCurrentStateResponse);
            CallMonitorDelegate.CallStateChanged(state, CallCurrentStateResponse);
        }
    }
}

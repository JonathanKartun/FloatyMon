using System;
using Android.Content;

namespace FloatyMon.Source
{
    public class Constants
    {
        public const string FLOATY_LOCAL_INTENT = "com.jon.floatymon.floaty";

        public const string FLOATY_MAKEVISIBLE_KEY = "makevisible";
        public const string FLOATY_SETTEXT_KEY = "settext";

        public const string FLOATY_NOTIFICATION_CHANNEL_ID_MAIN_SERVICE_KEY = "CallingServiceChannel";
        public const string FLOATY_NOTIFICATION_CHANNEL_ID_ACTIVITIES_KEY = "CallingServiceActivities";

        public const string FLOATY_NOTIFICATION_INTENT_PASSED_ACTIIVITY_KEY = "HandledMe";
        
        public const int OVERLAY_REQUEST_CODE = 11248;
        public const int PHONE_ACCESS_REQUEST_CODE = 1985;
    }
}

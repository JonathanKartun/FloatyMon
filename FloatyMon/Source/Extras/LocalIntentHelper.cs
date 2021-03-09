using System;
using Android.Content;

namespace FloatyMon.Source.Extras
{
    public class LocalIntentHelper
    {
        public static void SendLocalIntent(Context context, string IntentIdent, string Key, string Value)
        {
            Intent message = new Intent(IntentIdent);
            message.PutExtra(Key, Value);
            Android.Support.V4.Content.LocalBroadcastManager.GetInstance(context).SendBroadcast(message);
        }

        public static void SendLocalIntent(Context context, string IntentIdent, string Key, bool Value)
        {
            Intent message = new Intent(IntentIdent);
            message.PutExtra(Key, Value);
            Android.Support.V4.Content.LocalBroadcastManager.GetInstance(context).SendBroadcast(message);
        }
    }
}

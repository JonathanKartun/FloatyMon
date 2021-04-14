﻿using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Views;
using Android.Widget;
using FloatyMon;
using FloatyMon.Source;

[Service]
class FloatingWidgetService : Service, View.IOnTouchListener
{
    WindowManagerLayoutParams layoutParams;
    IWindowManager WindowManager;
    View floatView;

    TextView txtResult;

    public override void OnCreate()
    {
        base.OnCreate();
        RegisterLocalBroadcastReceiver();
    }

    public override void OnDestroy()
    {
        base.OnDestroy();

        if (floatView != null)
        {
            WindowManager.RemoveView(floatView);
            UnregisterLocalBroadcastReceiver();
        }
    }

    [return: GeneratedEnum]
    public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
    {
        ShowFloatingWindow();
        return StartCommandResult.Sticky;
    }

    public override IBinder OnBind(Intent intent)
    {
        return null;
    }

    private void ShowFloatingWindow()
    {
        WindowManager = GetSystemService(WindowService).JavaCast<IWindowManager>();
        LayoutInflater mLayoutInflater = LayoutInflater.From(ApplicationContext);
        floatView = mLayoutInflater.Inflate(Resource.Layout.layout_floating_widget, null);
        floatView.SetBackgroundColor(Color.Argb(15, 0,0,0));

        floatView.SetOnTouchListener(this);
        txtResult = floatView.FindViewById<TextView>(Resource.Id.txtDetails);
        ImageView iv1 = floatView.FindViewById<ImageView>(Resource.Id.iv1);
        ImageView iv2 = floatView.FindViewById<ImageView>(Resource.Id.iv2);
        ImageView iv3 = floatView.FindViewById<ImageView>(Resource.Id.iv3);
        iv1.Click += delegate { Toast.MakeText(ApplicationContext, "The first Image Click", ToastLength.Short).Show(); };
        iv2.Click += delegate { Toast.MakeText(ApplicationContext, "Hides Me!", ToastLength.Short).Show(); floatView.Visibility = ViewStates.Invisible; };
        iv3.Click += delegate { Toast.MakeText(ApplicationContext, "STOPPING SERVICE", ToastLength.Short).Show(); StopFloatingWindowService(); };

        layoutParams = new WindowManagerLayoutParams(
            ViewGroup.LayoutParams.WrapContent,
            ViewGroup.LayoutParams.WrapContent,
            WindowManagerTypes.Phone,
            WindowManagerFlags.NotTouchModal | WindowManagerFlags.NotFocusable | WindowManagerFlags.LayoutInScreen,
            Format.Translucent);

        if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
        {
            layoutParams.Type = WindowManagerTypes.ApplicationOverlay;
        }
        else
        {
            layoutParams.Type = WindowManagerTypes.Phone;
        }

        layoutParams.Width = 550;
        layoutParams.X = 100;
        layoutParams.Y = 200;
        layoutParams.Gravity = GravityFlags.Center | GravityFlags.Start;

        floatView.Visibility = ViewStates.Invisible;

        WindowManager.AddView(floatView, layoutParams);
    }

    private int x;
    private int y;
    public bool OnTouch(View v, MotionEvent e)
    {
        switch (e.Action)
        {
            case MotionEventActions.Down:
                x = (int)e.RawX;
                y = (int)e.RawY;
                break;

            case MotionEventActions.Move:
                int nowX = (int)e.RawX;
                int nowY = (int)e.RawY;
                int movedX = nowX - x;
                int movedY = nowY - y;
                x = nowX;
                y = nowY;
                layoutParams.X = layoutParams.X + movedX;
                layoutParams.Y = layoutParams.Y + movedY;

                WindowManager.UpdateViewLayout(floatView, layoutParams);
                break;

            default:
                break;
        }
        return false;
    }

    public void SetDetailsText(string text)
    {
        txtResult.SetText(text, TextView.BufferType.Normal);
    }

    private void StopFloatingWindowService()
    {
        var AppContext = Application.Context;
        Intent svc = new Intent(AppContext, typeof(FloatingWidgetService));
        AppContext.StopService(svc);
    }

    #region Broadcast Receiver - Listen From Call Mon

    LocaLocalBroadcastReceiver myLocalReceiver;

    [BroadcastReceiver(Enabled = true, Exported = false)]
    public class LocaLocalBroadcastReceiver : BroadcastReceiver
    {
        Context context;
        public LocaLocalBroadcastReceiver(Context context)
        {
            this.context = context;
        }
        public LocaLocalBroadcastReceiver(){}

        public override void OnReceive(Context context, Intent intent)
        {
            var makeVisible = intent.GetBooleanExtra(Constants.FLOATY_MAKEVISIBLE_KEY, false);
            var strText = intent.GetStringExtra(Constants.FLOATY_SETTEXT_KEY);
            
            if (intent.HasExtra(Constants.FLOATY_MAKEVISIBLE_KEY))
            {
                ((FloatingWidgetService)this.context).floatView.Visibility = ( makeVisible ? ViewStates.Visible : ViewStates.Invisible );
            }

            if (intent.HasExtra(Constants.FLOATY_SETTEXT_KEY))
            {
                ((FloatingWidgetService)this.context).SetDetailsText(strText);
            }
        }
    }

    void RegisterLocalBroadcastReceiver()
    {
        var AppContext = Application.Context;
        myLocalReceiver = new LocaLocalBroadcastReceiver(this);

        LocalBroadcastManager.GetInstance(AppContext).RegisterReceiver(myLocalReceiver, new IntentFilter("com.jon.floatymon.floaty"));
    }

    void UnregisterLocalBroadcastReceiver()
    {
        var AppContext = Application.Context;
        LocalBroadcastManager.GetInstance(AppContext).UnregisterReceiver(myLocalReceiver);
    }

    #endregion
}

using System;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using FloatyMon.Source;
using Xamarin.Essentials;

namespace FloatyMon
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        public static MainActivity ThisMainActivity;
        public MainActivityServiceManager serviceManager { get; private set; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            FloatingActionButton fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            fab.Click += FabOnClick;

            FloatingActionButton fab2 = FindViewById<FloatingActionButton>(Resource.Id.fab2);
            fab2.Click += Fab2OnClick;

            serviceManager = new MainActivityServiceManager(this);
            ThisMainActivity = this;

            SetupServices();

            CheckTheIntentWasLaunchedWithData();
        }

        [Obsolete]
        void SetupServices()
        {
            if (!serviceManager.IsServiceRunning<FloatingWidgetService>(Application.Context))
            {
                serviceManager.CheckCallReadPermission();
                serviceManager.CheckDrawOverlayPermission();
            }

            CheckIsRunning();
        }

        void CheckTheIntentWasLaunchedWithData()
        {
            Intent intent = this.Intent;

            if (intent != null && intent.Extras != null)
            {
                String intentActivity = intent.Extras.GetString(Constants.FLOATY_NOTIFICATION_INTENT_PASSED_ACTIIVITY_KEY);
                if (intentActivity != null)
                {
                    System.Diagnostics.Debug.WriteLine($">>> Acitivity Launched with Intent Data: {intentActivity}");
                } else
                {
                    System.Diagnostics.Debug.WriteLine($">>> Acitivity Launched with UNHANDLED Intent Data: {intent}");
                }
            }
        }

        [Obsolete]
        void CheckIsRunning()
        {
            var isRunning = serviceManager.IsServiceRunning<FloatingWidgetService>(Application.Context);
            System.Diagnostics.Debug.WriteLine($"IS RUNNING === {(isRunning ? "YES" : "NOP")}");
        }

        private void FabOnClick(object sender, EventArgs eventArgs)
        {
            View view = (View)sender;
            var snackBar = Snackbar.Make(view, "Relaunches Floating Window Service", Snackbar.LengthLong)
                .SetAction("Action", (View.IOnClickListener)null);

            if (!serviceManager.IsServiceRunning<FloatingWidgetService>(Application.Context))
            {
                snackBar.SetText("Relaunches Floating Window Service");
                serviceManager.LaunchCallingServiceListener();
                serviceManager.LaunchFloatingWindowService();
            } else
            {
                snackBar.SetText("Services are already launched. No need to launch again.");
            }
            snackBar.Show();
        }

        private void Fab2OnClick(object sender, EventArgs eventArgs)
        {
            View view = (View)sender;
            var snackBar = Snackbar.Make(view, "Relaunches Floating Window Service", Snackbar.LengthLong)
                .SetAction("Action", (View.IOnClickListener)null);

            if (serviceManager.IsServiceRunning<FloatingWidgetService>(Application.Context))
            {
                snackBar.SetText("Stopping Services.");
                ThisMainActivity.serviceManager.StopCallingMonitorService();
                ThisMainActivity.serviceManager.StopFloatingWindowService();
            }
            else
            {
                snackBar.SetText("Service wasn't running. No need to stop.");
            }
            snackBar.Show();
        }

        #region Permission Handling

        //Callback for the result from requesting permissions. This method is invoked for every call on ActivityCompat.requestPermissions
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            if (requestCode == Constants.PHONE_ACCESS_REQUEST_CODE)
            {
                serviceManager.LaunchCallingServiceListener(); //Launches the Phone call Listening Service [On first launch]
            }
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            if (requestCode == Constants.OVERLAY_REQUEST_CODE)
            {
                if (Android.Provider.Settings.CanDrawOverlays(this))
                {
                    serviceManager.LaunchFloatingWindowService();
                }
            }
            else
            {
                Toast.MakeText(this, "Sorry. Can't draw overlays without permission...", ToastLength.Short).Show();
            }
        }

        #endregion
    }
}

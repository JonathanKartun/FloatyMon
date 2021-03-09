using System;
using System.Threading.Tasks;
using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.App;
using Android.Support.V4.Content;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using FloatyMon.Source;
using FloatyMon.Source.Services;
using Xamarin.Essentials;
using static Xamarin.Essentials.Permissions;

namespace FloatyMon
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        public static MainActivity ThisMainActivity;
        private MainActivityServiceManager serviceManager { get; set; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            FloatingActionButton fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            fab.Click += FabOnClick;

            serviceManager = new MainActivityServiceManager(this);
            ThisMainActivity = this;

            SetupServices();
        }

        [Obsolete]
        void SetupServices()
        {
            //Main Stuff...
            serviceManager.CheckCallReadPermission();    //Number getter
            serviceManager.CheckFloatAllowedAndLaunchFloatingWindow(); //Float Allowed ? -> Show loater Window
            serviceManager.LaunchCallingServiceListener(); //Launches the Phone call Listening Service
            //CheckFloatAllowedAndLaunchFloatingWindow(); //Float Allowed ? -> Show loater Window

            var isRunning = serviceManager.isServiceRunning<FloatingWidgetService>(MainActivityServiceManager.AppContext());
            System.Diagnostics.Debug.WriteLine($"IISSSS RUNN === {(isRunning ? "YEST":"NOP")}");
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.action_settings)
            {
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        private void FabOnClick(object sender, EventArgs eventArgs)
        {
            View view = (View)sender;
            Snackbar.Make(view, "Relaunches Floating Window Service", Snackbar.LengthLong)
                .SetAction("Action", (Android.Views.View.IOnClickListener)null).Show();

            serviceManager.LaunchFloatingWindowService();
        }

        //test
        public void SetFloatingWindowText(string text)
        {
            //MainActivity.FLOATINGsvc.
            //serviceConnection.DoSomethingSomething();
        }

        #region Permission Handling

        //Callback for the result from requesting permissions. This method is invoked for every call on ActivityCompat.requestPermissions
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        protected override void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            if (requestCode == Constants.OVERLAY_REQUEST_CODE)
            {
                if (Android.Provider.Settings.CanDrawOverlays(this))
                {
                    //Launch the service
                    //LaunchFloatingWindowService();  //this would normally launch
                    serviceManager.LaunchFloatingWindowService();
                }
            }
            else
            {
                //Launch the service
                Toast.MakeText(this, "Sorry. Can't draw overlays without permission...", ToastLength.Short).Show();
            }
        }

        #endregion
    }
}

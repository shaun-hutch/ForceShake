using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content;
using Android.Bluetooth;
using Android.Support.V4.Content;
using Android.Content.PM;
using Android.Support.V4.App;
using Android.Runtime;

namespace ForceShake
{
    [Activity(Label = "ForceShake", MainLauncher = true)]
    public class MainActivity : Activity
    {
        private BluetoothReceiver receiver;
        const int LOCATION_REQUEST_CODE = 9001;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);



        }

        protected override void OnResume()
        {
            base.OnResume();
            CheckLocationPermissions();

        }

        protected override void OnPause()
        {
            base.OnPause();

            if (receiver != null)
            {
                UnregisterReceiver(receiver);
                Toast.MakeText(this, "Unregistered receiver.", ToastLength.Short).Show();
            }
        }

        private void StartBluetoothReciever()
        {
            receiver = new BluetoothReceiver();
            RegisterReceiver(receiver, new IntentFilter(BluetoothDevice.ActionFound));
            ScanBluetoothDevices();

            Toast.MakeText(this, "Registered receiver.", ToastLength.Short).Show();


        }

        private void ScanBluetoothDevices()
        {
            if (BluetoothAdapter.DefaultAdapter != null && BluetoothAdapter.DefaultAdapter.IsEnabled)
            {
                if (BluetoothAdapter.DefaultAdapter.StartDiscovery())
                {

                }
            }
        }



        private void CheckLocationPermissions()
        {
            var locationPermission = new string[]
            {
                Android.Manifest.Permission.AccessFineLocation,
                Android.Manifest.Permission.AccessCoarseLocation
            };

            var fineLocationGranted = ContextCompat.CheckSelfPermission(this, locationPermission[0]);
            var coarseLocationGranted = ContextCompat.CheckSelfPermission(this, locationPermission[1]);

            if (fineLocationGranted == Permission.Denied || coarseLocationGranted == Permission.Denied)
            {
                ActivityCompat.RequestPermissions(this, locationPermission, LOCATION_REQUEST_CODE);
            }
            else
            {
                StartBluetoothReciever();
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Permission[] grantResults)
        {
            if (requestCode == LOCATION_REQUEST_CODE)
            {
                if (grantResults[0] == Permission.Granted)
                {
                    StartBluetoothReciever();
                }
                else
                {
                    Toast.MakeText(this, "No location permission granted.", ToastLength.Long).Show();
                }
            }

        }

    }

}


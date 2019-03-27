using Android.App;
using Android.Widget;
using Android.OS;
using Android.Content;
using Android.Bluetooth;
using Android.Support.V4.Content;
using Android.Content.PM;
using Android.Support.V4.App;
using Android.Runtime;
using System.Collections.Generic;
using System;

namespace ForceShake
{
    [Activity(Label = "ForceShake", MainLauncher = true)]
    public class MainActivity : Activity
    {
        private BluetoothReceiver receiver;
        private BluetoothListViewAdapter adapter;
        private ListView deviceListView;
        private ProgressBar scanSpinner;
        private TextView savedDevice;
        const int LOCATION_REQUEST_CODE = 9001;

        ISharedPreferences prefs;
        ISharedPreferencesEditor editor;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Main);
            deviceListView = FindViewById<ListView>(Resource.Id.deviceListView);
            scanSpinner = FindViewById<ProgressBar>(Resource.Id.scanSpinner);
            savedDevice = FindViewById<TextView>(Resource.Id.savedDevice);
            adapter = new BluetoothListViewAdapter(this);
            deviceListView.Adapter = adapter;
            deviceListView.ItemClick += DeviceListView_ItemClick;
            prefs = GetSharedPreferences("settings", FileCreationMode.Private);
            editor = prefs.Edit();
            string macAddress = prefs.GetString("device", "");
            string name = prefs.GetString("name", "");
            savedDevice.Text = string.IsNullOrWhiteSpace(macAddress) ? "No device set" : $"Device: {name} - {macAddress}";
        }

        private void DeviceListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            //save the bluetooth device, then if found on scan, switch bluetooth off and on again
            BluetoothDevice device = Device.Devices[e.Position];
            editor.PutString("device", device.Address);
            editor.PutString("name", device.Name);
            editor.Apply();
            savedDevice.Text = $"Device: {device.Name} - {device.Address}";
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
                StopScan();
                Toast.MakeText(this, "Unregistered receiver.", ToastLength.Short).Show();
            }
        }

        private void StartBluetoothReciever()
        {
            receiver = new BluetoothReceiver(adapter);
            IntentFilter filter = new IntentFilter();
            filter.AddAction(BluetoothDevice.ActionFound);
            filter.AddAction(BluetoothAdapter.ActionDiscoveryStarted);
            filter.AddAction(BluetoothAdapter.ActionDiscoveryFinished);
            RegisterReceiver(receiver, filter);
            StartScan();

            Toast.MakeText(this, "Registered receiver.", ToastLength.Short).Show();
        }

        private void StartScan()
        {
            BluetoothAdapter.DefaultAdapter.Enable();
            if (BluetoothAdapter.DefaultAdapter != null && BluetoothAdapter.DefaultAdapter.IsEnabled)
            {
                BluetoothAdapter.DefaultAdapter.StartDiscovery();

                Device.Devices.Clear();
                IEnumerable<BluetoothDevice> devices = BluetoothAdapter.DefaultAdapter.BondedDevices;

                Device.Devices.AddRange(devices);
                adapter.NotifyDataSetChanged();
            }
        }

        private void StopScan()
        {
            if (BluetoothAdapter.DefaultAdapter != null && BluetoothAdapter.DefaultAdapter.IsEnabled)
            {
                BluetoothAdapter.DefaultAdapter.CancelDiscovery();
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

        public void ShowSpinner()
        {
            scanSpinner.Visibility = Android.Views.ViewStates.Visible;
        }

        public void HideSpinner()
        {
            scanSpinner.Visibility = Android.Views.ViewStates.Gone;
        }

    }

}


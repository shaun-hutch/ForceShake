
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace ForceShake
{
    [BroadcastReceiver]
    public class BluetoothReceiver : BroadcastReceiver
    {
        public BluetoothListViewAdapter Adapter { get; set; }
        public MainActivity activity;

        public BluetoothReceiver() { }
        public BluetoothReceiver(BluetoothListViewAdapter adapter)
        {
            Adapter = adapter;
        }

        public override void OnReceive(Context context, Intent intent)
        {
            activity = (MainActivity)context;
            


            Toast.MakeText(activity, "Received intent!", ToastLength.Short).Show();

            string action = intent.Action;

            switch (action)
            {
                case BluetoothDevice.ActionFound:
                    AddDevice(intent);
                    break;
                case BluetoothAdapter.ActionDiscoveryStarted:
                    activity.RunOnUiThread(() => { activity.ShowSpinner(); });
                    
                    break;
                case BluetoothAdapter.ActionDiscoveryFinished:
                    activity.RunOnUiThread(() => { activity.HideSpinner(); });
                    break;
                case BluetoothDevice.ActionPairingRequest:
                    int pin = intent.GetIntExtra(BluetoothDevice.ExtraPairingKey, 0000);
                    BluetoothDevice device = (BluetoothDevice)intent.GetParcelableExtra(BluetoothDevice.ExtraDevice);
                    device.SetPin(Encoding.UTF8.GetBytes(pin.ToString()));
                    device.SetPairingConfirmation(true);
                    break;
                default:
                    return;
            }

            if (action != BluetoothDevice.ActionFound)
            {
                return;
            }
        }

        private void AddDevice(Intent intent)
        {
            BluetoothDevice device = (BluetoothDevice)intent.GetParcelableExtra(BluetoothDevice.ExtraDevice);
            string deviceMac = activity.GetSharedPreferences("settings", FileCreationMode.Private).GetString("device", "");

            //switch off bluetooth then on again
            if (device.Address == deviceMac)
            {
                BluetoothAdapter.DefaultAdapter.Disable();
                System.Threading.Thread.Sleep(2000);
                BluetoothAdapter.DefaultAdapter.Enable();
                BluetoothAdapter.DefaultAdapter.StartDiscovery();

                return;
            }


            Console.WriteLine($"Found device with {device.Name} and MAC address {device.Address}");
            if (!Device.Devices.Any(x => x.Address == device.Address))
            {
                Device.Devices.Add(device);
                Adapter.NotifyDataSetChanged();
            }
        }
    }
}

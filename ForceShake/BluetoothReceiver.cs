
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
        public override void OnReceive(Context context, Intent intent)
        {
            Toast.MakeText(context, "Received intent!", ToastLength.Short).Show();

            string action = intent.Action;

            if (action != BluetoothDevice.ActionFound)
            {
                return;
            }

            var device = (BluetoothDevice)intent.GetParcelableExtra(BluetoothDevice.ExtraDevice);

            if (device.BondState != Bond.Bonded)
            {
                Console.WriteLine($"Found device with {device.Name} and MAC address {device.Address}");
            }

        }
    }
}

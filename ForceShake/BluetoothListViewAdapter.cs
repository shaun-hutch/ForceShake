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
    public class BluetoothListViewAdapter : BaseAdapter<BluetoothDevice>
    {
        Context context;
        List<BluetoothDevice> devices;
        public BluetoothListViewAdapter(Context context)
        {
            this.context = context;
            this.devices = Device.Devices;
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            var view = convertView;
            TextView title;
            TextView macAddressText;
            if (view == null)
            {
                view = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.BluetoothListViewItem, null);
            }

            title = view.FindViewById<TextView>(Resource.Id.txtDeviceTitle);
            macAddressText = view.FindViewById<TextView>(Resource.Id.txtMacAddress);
            title.Text = this[position].Name;
            macAddressText.Text = this[position].Address;

            return view;
        }

        public override long GetItemId(int position) => position;

        public override int Count => devices == null ? 0 : devices.Count;

        public override BluetoothDevice this[int position] => devices[position];
    }
}
using Android.Bluetooth;
using System.Collections.Generic;

namespace ForceShake
{
    public class Device
    {
        public Device() { }

        public static List<BluetoothDevice> Devices = new List<BluetoothDevice>();
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forensics.Model.Device
{
    public struct DeviceProperty
    {
        public string SerialNumber;
        public string ModelNumber;
        public string IMEI;

        // 苹果设备
        public string Name;
        public string ProductType;
        public string ICCID;
        public string IMSI;
        public string ActivationState;
        public string BasebandMasterKeyHash;
        public string BuildVersion;
        public string Class;
        public string PhoneNumber;
        public string ProductVersion;
        public string SIMStatus;
        public string UniqueChipID;
        public string UniqueDeviceID;
        public string BluetoothAddress;
        public string WiFiAddress;

        // 安卓
        public string OSVersion;
        public string Brand;
        public bool IsRooted;
    };

}

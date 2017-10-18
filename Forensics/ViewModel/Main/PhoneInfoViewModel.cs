using Forensics.Model.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Forensics.ViewModel.Main
{
    public class PhoneInfoViewModel : ViewModelBase
    {
        public override Pages PageIndex => throw new NotImplementedException();

        /// <summary>
        /// 设备名称
        /// </summary>
        public string DeviceName { get; set; }

        /// <summary>
        /// 手机类型图片
        /// </summary>
        public string PhoneImage { get; set; }
        public Thickness MarginInfo { get; set; }

        /// <summary>
        /// 设备信息
        /// </summary>
        public string DeviceInfo { get; set; }

        public PhoneInfoViewModel()
        {
        }
    }

    /// <summary>
    /// 苹果手机信息
    /// </summary>
    public class PhoneInfoAppleViewModel : PhoneInfoViewModel
    {
        public PhoneInfoAppleViewModel(DeviceProperty dp)
        {
            this.MarginInfo = new Thickness(0, 130, 0, 0);
            this.PhoneImage = "/Resources/Images/home_phone_ios.png";

            // 设备名称
            if (dp.Name.Contains("\0"))
            {
                this.DeviceName = dp.Name.Substring(0, dp.Name.IndexOf("\0")).Trim("\0".ToCharArray());
            }
            else
            {
                this.DeviceName = dp.Name;
            }

            var strDevInfo = "";

            // ICCID
            strDevInfo += "ICCID: " + dp.ICCID + "\n";
            // IMEI
            strDevInfo += "IMEI: " + dp.IMEI + "\n";
            // 本机号码
            strDevInfo += "本机号码: " + dp.PhoneNumber + "\n";
            // 本机序号
            strDevInfo += "本机序号: " + dp.SerialNumber + "\n";
            // 设备类型
            strDevInfo += "设备类型: " + dp.ProductType + "\n";
            // 系统版本
            strDevInfo += "系统版本: " + dp.ProductVersion + "\n";
            // WiFi地址
            strDevInfo += "WiFi地址: " + dp.WiFiAddress + "\n";
            // 芯片ID
            strDevInfo += "芯片ID: " + dp.UniqueDeviceID + "\n";
            // 蓝牙地址
            strDevInfo += "蓝牙地址: " + dp.BluetoothAddress + "\n";

            this.DeviceInfo = strDevInfo;
        }
    }
}

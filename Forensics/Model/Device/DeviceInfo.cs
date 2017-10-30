using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forensics.Model.Device
{
    public class DeviceInfo
    {
        public DeviceProperty DeviceProperty { get; set; }

        public DeviceInfo(DeviceProperty dp)
        {
            DeviceProperty = dp;
        }
    }
}

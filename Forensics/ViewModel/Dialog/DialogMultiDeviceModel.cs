using Forensics.Model.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forensics.ViewModel.Dialog
{
    public class DialogMultiDeviceModel : ViewModelBase
    {
        public override Pages PageIndex => throw new NotImplementedException();

        public List<BrandInfo> listDevice { get; set; } = new List<BrandInfo>();
        public int SelectedIndexDevice { get; set; } = 0;

        public DialogMultiDeviceModel()
        {
            // 添加设备
            BrandInfo nn = new BrandInfo();
            nn.Name = "Android device";
            listDevice.Add(nn);

            nn = new BrandInfo();
            nn.Name = "Good Device";
            listDevice.Add(nn);

            nn = new BrandInfo();
            nn.Name = "Do you love me?";
            listDevice.Add(nn);

            nn = new BrandInfo();
            nn.Name = "Will you marry me?";
            listDevice.Add(nn);
        }
    }
}

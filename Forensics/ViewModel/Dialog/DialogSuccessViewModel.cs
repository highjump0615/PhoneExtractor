using Forensics.Model.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forensics.ViewModel.Dialog
{
    public class DialogSuccessViewModel : ViewModelBase
    {
        public override Pages PageIndex => throw new NotImplementedException();

        /// <summary>
        /// 是否安卓设备
        /// </summary>
        public bool IsAndroid { get; set; }

        /// <summary>
        /// 品牌
        /// </summary>
        public string Brand { get; set; }

        /// <summary>
        /// 版本
        /// </summary>
        public string OSVersion { get; set; }

        /// <summary>
        /// 型号
        /// </summary>
        public string ModelNumber { get; set; }

        /// <summary>
        /// root状态
        /// </summary>
        public string RootStatus { get; set; }

        public DialogSuccessViewModel(MainHomeViewModel.DeviceType devType)
        {
            this.IsAndroid = false;

            if (devType == MainHomeViewModel.DeviceType.Android)
            {
                this.IsAndroid = true;

                // 初始化
                MainViewModel mainVM = Globals.Instance.MainVM;
                DeviceProperty devProp = mainVM.CurrentDevice.DeviceProperty;

                this.Brand = devProp.Brand;
                this.OSVersion = devProp.OSVersion;
                this.ModelNumber = devProp.ModelNumber;
                this.RootStatus = devProp.IsRooted ? "Root" : "Unroot";
            }
        }
    }
}

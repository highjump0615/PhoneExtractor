using Forensics.Model.Device;
using Forensics.ViewModel.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forensics.ViewModel
{
    public class MainHomeViewModel : HostViewModel
    {
        public enum ExtractType {
            Apple,
        }

        /// <summary>
        /// 左边手机区域
        /// </summary>
        public ViewModelBase PhoneArea { get; set; }

        public override Pages PageIndex
        {
            get { return Pages.MainHome; }
        }

        public MainHomeViewModel()
        {
            this.RegisterChild<HomeHomeViewModel>(() => new HomeHomeViewModel());
            this.RegisterChild<MainExtractViewModel>(() => new MainExtractViewModel());

            this.SelectedChild = GetChild(typeof(HomeHomeViewModel));

            // 未连接
            this.PhoneArea = new PhoneReadyViewModel();
        }

        /// <summary>
        /// 显示设备信息
        /// </summary>
        public void showDeviceInfo()
        {
            MainViewModel mainVM = Globals.Instance.MainVM;
            if (mainVM.CurrentDevice == null)
            {
                this.PhoneArea = new PhoneReadyViewModel();
            }
            else
            {
                this.PhoneArea = new PhoneInfoAppleViewModel(mainVM.CurrentDevice.DeviceProperty);
            }

            PropertyChanging("PhoneArea");
        }

        /// <summary>
        /// 打开提取页面
        /// </summary>
        public void showExtractPage(ExtractType type, string saveExtractPath = null)
        {
            // 打开提取页面
            this.SelectedChild = GetChild(typeof(MainExtractViewModel));

            MainExtractViewModel vm = (MainExtractViewModel)SelectedChild;
            vm.startExtract(type, saveExtractPath);
        }
    }
}

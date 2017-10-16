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

            this.PhoneArea = new PhoneReadyViewModel();
        }

        /// <summary>
        /// 打开提取页面
        /// </summary>
        public void showExtractPage(ExtractType type)
        {
            // 打开提取页面
            this.SelectedChild = GetChild(typeof(MainExtractViewModel));

            MainExtractViewModel vm = (MainExtractViewModel)SelectedChild;
            vm.startExtract(type);
        }
    }
}

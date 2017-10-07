using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forensics.ViewModel
{
    public class MainHomeViewModel : HostViewModel
    {
        public override Pages PageIndex
        {
            get { return Pages.MainHome; }
        }

        public MainHomeViewModel()
        {
            this.RegisterChild<HomeHomeViewModel>(() => new HomeHomeViewModel());
            this.RegisterChild<MainExtractViewModel>(() => new MainExtractViewModel());

            this.SelectedChild = GetChild(typeof(HomeHomeViewModel));
        }

        /// <summary>
        /// 打开提取页面
        /// </summary>
        public void showExtractPage()
        {
            this.SelectedChild = GetChild(typeof(MainExtractViewModel));
        }
    }
}

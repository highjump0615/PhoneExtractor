using Forensics.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Forensics.ViewModel
{
    class MainDataViewModel : HostViewModel
    {
        /// <summary>
        /// 首页命令
        /// </summary>
        private ICommand _goToCaseCommand;
        public ICommand GoToCaseCommand
        {
            get { return _goToCaseCommand ?? (_goToCaseCommand = new DelegateCommand(GoToCasePage)); }
        }

        /// <summary>
        /// 详情页命令
        /// </summary>
        private ICommand _goToCaseDetailCommand;
        public ICommand GoToCaseDetailCommand
        {
            get { return _goToCaseDetailCommand ?? (_goToCaseDetailCommand = new DelegateCommand(GoToCaseDetailPage)); }
        }

        public override Pages PageIndex
        {
            get { return Pages.MainData; }
        }

        public MainDataViewModel()
        {
            this.RegisterChild<DataCaseViewModel>(() => new DataCaseViewModel());
            this.RegisterChild<DataCaseDetailViewModel>(() => new DataCaseDetailViewModel());

            this.SelectedChild = GetChild(typeof(DataCaseViewModel));
        }

        /// <summary>
        /// 跳转到案件管理页
        /// </summary>
        private void GoToCasePage()
        {
            this.SelectedChild = GetChild(typeof(DataCaseViewModel));
        }

        /// <summary>
        /// 跳转到案件详情页
        /// </summary>
        private void GoToCaseDetailPage()
        {
            this.SelectedChild = GetChild(typeof(DataCaseDetailViewModel));
        }
    }
}

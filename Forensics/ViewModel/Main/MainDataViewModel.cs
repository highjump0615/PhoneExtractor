using Forensics.Command;
using Forensics.Model.DataManagement;
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

        public override Pages PageIndex
        {
            get { return Pages.MainData; }
        }

        public MainDataViewModel()
        {
            this.RegisterChild<DataCaseViewModel>(() => new DataCaseViewModel(this));

            this.SelectedChild = GetChild(typeof(DataCaseViewModel));
        }

        /// <summary>
        /// 跳转到案件管理页
        /// </summary>
        public void GoToCasePage()
        {
            this.SelectedChild = GetChild(typeof(DataCaseViewModel));
        }

        /// <summary>
        /// 跳转到案件详情页
        /// </summary>
        /// <param name="caseInfo"></param>
        public void GoToCaseDetailPage(Case2 caseInfo)
        {
            this.SelectedChild = new DataCaseDetailViewModel(this, caseInfo);
        }
    }
}

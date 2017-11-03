using Forensics.Command;
using Forensics.Model.DataManagement;
using Forensics.ViewModel.Data;
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
        /// 案件列表命令
        /// </summary>
        private ICommand _goToCaseCommand;
        public ICommand GoToCaseCommand
        {
            get { return _goToCaseCommand ?? (_goToCaseCommand = new DelegateCommand(GoToCasePage)); }
        }

        /// <summary>
        /// 物证列表命令
        /// </summary>
        private ICommand _goToEvidenceCommand;
        public ICommand GoToEvidenceCommand
        {
            get { return _goToEvidenceCommand ?? (_goToEvidenceCommand = new DelegateCommand(GoToEvidencePage)); }
        }

        public override Pages PageIndex
        {
            get { return Pages.MainData; }
        }

        public MainDataViewModel()
        {
            this.RegisterChild<DataCaseViewModel>(() => new DataCaseViewModel(this));
            this.RegisterChild<DataEvidenceViewModel>(() => new DataEvidenceViewModel());

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
        /// 跳转到物证列表页
        /// </summary>
        private void GoToEvidencePage()
        {
            this.SelectedChild = GetChild(typeof(DataEvidenceViewModel));
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

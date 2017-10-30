using Forensics.BLL;
using Forensics.Command;
using Forensics.Model;
using Forensics.Model.DataManagement;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Forensics.ViewModel
{
    class DataCaseDetailViewModel : ViewModelBase
    {
        /// <summary>
        /// 当前案件
        /// </summary>
        public Case2 CaseInfo { get; set; }

        public ObservableCollection<Evidence> ListEvidence { get; set; }

        public override Pages PageIndex
        {
            get { return Pages.DataCaseDetail; }
        }

        /// <summary>
        /// 返回命令
        /// </summary>
        private ICommand _goBackCommand;
        public ICommand GoBackCommand
        {
            get { return _goBackCommand ?? (_goBackCommand = new DelegateCommand(GoBack)); }
        }

        /// <summary>
        /// 返回
        /// </summary>
        private void GoBack()
        {
            MainDataViewModel parentVM = (MainDataViewModel)this.ViewModelParent;
            parentVM.GoToCasePage();
        }

        public DataCaseDetailViewModel(ViewModelBase vmParent, Case2 caseInfo)
        {
            this.ViewModelParent = vmParent;
            this.CaseInfo = caseInfo;

            // 获取该案件的物证列表
            DataManager dm = new DataManager();
            this.ListEvidence = new ObservableCollection<Evidence>(dm.GetAllEvidences(this.CaseInfo.CASE_PATH));
        }
    }
}

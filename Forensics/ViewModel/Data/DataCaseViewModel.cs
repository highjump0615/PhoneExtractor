using Forensics.BLL;
using Forensics.Command;
using Forensics.Model;
using Forensics.Model.DataManagement;
using Forensics.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Forensics.ViewModel
{
    class DataCaseViewModel : ViewModelBase
    {
        private CaseManager caseManager = new CaseManager();

        public ObservableCollection<Case2> ListCase { get; set; } = new ObservableCollection<Case2>();

        public override Pages PageIndex
        {
            get { return Pages.DataCase; }
        }

        /// <summary>
        /// 详情命令
        /// </summary>
        private ICommand _goToDetailCommand;
        public ICommand GoToDetailCommand
        {
            get { return _goToDetailCommand ?? (_goToDetailCommand = new DelegateCommand(GoToDetailPage)); }
        }

        public DataCaseViewModel(ViewModelBase vmParent)
        {
            this.ViewModelParent = vmParent;

            // 获取案件列表
            List<Case> caseList = caseManager.GetCaseByWhere("all");
            InitialCaseInfo(caseList);
        }

        /// <summary>
        /// 加载案件
        /// </summary>
        /// <param name="caseList"></param>
        private void InitialCaseInfo(List<Case> caseList)
        {
            foreach (Case c in caseList)
            {
                Case2 c2 = CommonUtil.ToDerived<Case, Case2>(c);
                ListCase.Add(c2);
            }
        }

        /// <summary>
        /// 跳转到详情页
        /// </summary>
        private void GoToDetailPage(object param)
        {
            Case2 caseInfo = (Case2)param;
            MainDataViewModel parentVM = (MainDataViewModel)this.ViewModelParent;
            parentVM.GoToCaseDetailPage(caseInfo);
        }
    }
}

using Forensics.BLL;
using Forensics.Model;
using Forensics.Model.DataManagement;
using Forensics.Util;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public DataCaseViewModel()
        {
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
    }
}

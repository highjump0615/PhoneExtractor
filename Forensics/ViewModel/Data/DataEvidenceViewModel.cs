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
using System.Windows;
using System.Windows.Input;

namespace Forensics.ViewModel.Data
{
    public class DataEvidenceViewModel : ViewModelBase
    {
        private EvidenceManager eviManager = new EvidenceManager();

        public override Pages PageIndex
        {
            get { return Pages.DataEvidence; }
        }

        public ObservableCollection<Evidence2> ListEvidence { get; set; } = new ObservableCollection<Evidence2>();

        /// <summary>
        /// 删除命令
        /// </summary>
        private ICommand _deleteCommand;
        public ICommand DeleteCommand
        {
            get { return _deleteCommand ?? (_deleteCommand = new DelegateCommand(DeleteEvidence)); }
        }

        public DataEvidenceViewModel()
        {
            // 获取案件列表
            CaseManager caseManager = new CaseManager();
            List<Case> caseList = caseManager.GetCaseByWhere("all", "ADDTIME", "desc");

            // 获取该案件的物证列表
            DataManager dm = new DataManager();
            List<Evidence> evidenceList = eviManager.GetAllEvidences();
            foreach (Evidence e in evidenceList)
            {
                Evidence2 e2 = CommonUtil.ToDerived<Evidence, Evidence2>(e);
                Case cc = caseList.Where(x => x.CASE_GUID == e2.CASE_GUID).FirstOrDefault();
                e2.CaseBelonged = cc;

                this.ListEvidence.Add(e2);
            }

        }

        /// <summary>
        /// 删除物证
        /// </summary>
        private void DeleteEvidence()
        {
            // 检查有没有选择的
            if (this.ListEvidence.Where(x => x.IsSelected).Count() == 0)
            {
                return;
            }

            // 删除
            if (!MessageBoxResult.OK.Equals(MessageBox.Show("确定要删除此物证吗？", _clew, MessageBoxButton.OKCancel, MessageBoxImage.Question)))
            {
                return;
            }

            for (var i = this.ListEvidence.Count() - 1; i >= 0; i--)
            {
                var cc = this.ListEvidence[i];
                if (!cc.IsSelected)
                {
                    continue;
                }

                Evidence2 evi = this.ListEvidence[i];
                string strCasePath = "";
                if (evi.CaseBelonged != null)
                {
                    strCasePath = evi.CaseBelonged.CASE_PATH;
                }

                eviManager.DelEvidenceDataById(strCasePath, evi.EVIDENCE_GUID);
                this.ListEvidence.RemoveAt(i);
            }
        }
    }
}

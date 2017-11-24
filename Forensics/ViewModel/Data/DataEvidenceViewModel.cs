using Forensics.BLL;
using Forensics.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forensics.ViewModel.Data
{
    public class DataEvidenceViewModel : ViewModelBase
    {
        private EvidenceManager eviManager = new EvidenceManager();

        public override Pages PageIndex
        {
            get { return Pages.DataEvidence; }
        }

        public List<Evidence> ListEvidence { get; set; }

        public DataEvidenceViewModel()
        {
            // 获取该案件的物证列表
            DataManager dm = new DataManager();
            this.ListEvidence = eviManager.GetAllEvidences();
        }
    }
}

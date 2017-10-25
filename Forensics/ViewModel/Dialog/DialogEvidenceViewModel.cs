using Forensics.BLL;
using Forensics.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forensics.ViewModel.Dialog
{
    public class DialogEvidenceViewModel : ViewModelBase
    {
        public override Pages PageIndex => throw new NotImplementedException();

        public string CaseNumber { get; set; }

        CaseManager caseManager = new CaseManager();

        string gsid = "0";

        public DialogEvidenceViewModel()
        {
            gsid = CommonUtil.Rulename.GetCaseautoid().ToString();

            int limax = 0;
            //if (Directory.Exists(this.txtCasePath.Text))
            //{
            //    DirectoryInfo cDIR = new DirectoryInfo(this.txtCasePath.Text);
            //    foreach (DirectoryInfo dir in cDIR.GetDirectories())
            //    {
            //        if (dir.Name.IndexOf(caseno1) == 0 && dir.Name.IndexOf("_") > 0)
            //        {
            //            //found the old case folder double check the no
            //            string lstmp = dir.Name.Substring(caseno1.Length, dir.Name.IndexOf("_") - caseno1.Length);
            //            try
            //            {
            //                if (Convert.ToInt32(lstmp) > limax)
            //                    limax = Convert.ToInt32(lstmp);
            //            }
            //            catch (Exception ex)
            //            {
            //                Console.Write(ex.ToString());
            //            }
            //        }
            //    }
            //}
            if (limax > Convert.ToInt32(gsid))
            {
                gsid = (limax + 1).ToString();
            }

            string caseno1 = CommonUtil.Rulename.GetCaseNoName();
        }
    }
}

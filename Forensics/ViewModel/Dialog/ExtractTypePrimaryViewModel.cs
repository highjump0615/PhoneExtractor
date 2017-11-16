using Forensics.Command;
using Forensics.Model.Extract;
using Forensics.Util;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Forensics.ViewModel.Dialog
{
    public class ExtractTypePrimaryViewModel : ViewModelBase
    {
        public override Pages PageIndex
        {
            get { return Pages.Other; }
        }

        /// <summary>
        /// 打开二级命令
        /// </summary>
        private ICommand _goToSecondaryCommand;
        public ICommand GoToSecondaryCommand
        {
            get { return _goToSecondaryCommand ?? (_goToSecondaryCommand = new DelegateCommand(GoToSecondary)); }
        }

        public List<Act> listNormalTypes { get; set; }
        public List<Act> listAdvancedTypes { get; set; }

        public ExtractTypePrimaryViewModel(ViewModelBase vmParent)
        {
            this.ViewModelParent = vmParent;

            this.listNormalTypes = Globals.Instance.MainActGroup[0].Acts.ToList();
            this.listAdvancedTypes = Globals.Instance.MainActGroup[1].Acts.ToList();

            // 品种
            if (Globals.Instance.AndroidPhoneSelected != null)
            {
                string strConnection = ConfigurationManager.ConnectionStrings["mdb_phone"].ToString();
                string strQuery = "select * from edec_support_act where BrandModelID = \"" + Globals.Instance.AndroidPhoneSelected.BrandModelID + "\"; ";

                DataTable dt = DatabaseUtil.Query(strQuery, strConnection);
                foreach (DataRow tmpdr in dt.Rows)
                {
                    var strActId = int.Parse(tmpdr["ACT_ID"].ToString());

                    var act = this.listNormalTypes.Where(x => x.Id == strActId).FirstOrDefault();
                    if (act == null)
                    {
                        act = this.listAdvancedTypes.Where(x => x.Id == strActId).FirstOrDefault();
                    }

                    if (act != null)
                    {
                        act.IsAvailable = true;
                    }
                }
            }
            else
            {
                // 自动连接，先能用所有的
                foreach (Act at in this.listNormalTypes)
                {
                    at.IsAvailable = true;
                }
            }

            // 多中选一
        }

        /// <summary>
        /// 打开二级
        /// </summary>
        /// <param name="param"></param>
        private void GoToSecondary(object param)
        {
            Act actSel = (Act)param;

            DialogExtractTypeViewModel vmParent = (DialogExtractTypeViewModel)this.ViewModelParent;
            vmParent.GoToSecondary(actSel);
        }

        /// <summary>
        /// 设置已选好的提取方式
        /// </summary>
        /// <param name="actSecondary"></param>
        public void SetActFromSecondary(Act actSecondary)
        {
            // 交换act
            for (var i = 0; i < this.listNormalTypes.Count(); i++)
            {
                var at = this.listNormalTypes[i];

                if (at.Id == actSecondary.Id)
                {
                    this.listNormalTypes[i] = actSecondary;
                    break;
                }
            }
        }
    }
}

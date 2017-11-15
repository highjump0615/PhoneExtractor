using Forensics.Model.Extract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forensics.ViewModel.Dialog
{
    public class DialogExtractTypeViewModel : HostViewModel
    {
        public override Pages PageIndex => throw new NotImplementedException();

        public DialogExtractTypeViewModel()
        {
            this.AddChild(new ExtractTypePrimaryViewModel(this));

            this.SelectedChild = GetChildAt(0);
        }

        /// <summary>
        /// 打开二级
        /// </summary>
        /// <param name="act"></param>
        public void GoToSecondary(Act act)
        {
            this.AddChild(new ExtractTypeSecondaryViewModel(this, act));
            this.SelectedChild = GetChildAt(1);
        }

        /// <summary>
        /// 返回一级
        /// </summary>
        public void GoBackToPrimary(Act act)
        {
            ExtractTypePrimaryViewModel vmPrimary = (ExtractTypePrimaryViewModel)this.PopChild();
            vmPrimary.SetActFromSecondary(act);
            this.SelectedChild = vmPrimary;
        }
    }
}

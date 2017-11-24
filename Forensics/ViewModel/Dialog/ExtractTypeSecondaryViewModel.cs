using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Forensics.Model.Extract;
using System.Windows.Input;
using Forensics.Command;

namespace Forensics.ViewModel.Dialog
{
    public class ExtractTypeSecondaryViewModel : ViewModelBase
    {
        public Act ActSelected { get; set; }

        public override Pages PageIndex
        {
            get { return Pages.ExtractTypeSecondary; }
        }

        /// <summary>
        /// 返回命令
        /// </summary>
        private ICommand _goBackCommand;
        public ICommand GoBackCommand
        {
            get { return _goBackCommand ?? (_goBackCommand = new DelegateCommand(GoBack)); }
        }

        public ExtractTypeSecondaryViewModel(ViewModelBase vmParent, Act act)
        {
            this.ViewModelParent = vmParent;

            this.ActSelected = act;
        }

        /// <summary>
        /// 返回
        /// </summary>
        private void GoBack()
        {
            DialogExtractTypeViewModel parentVM = (DialogExtractTypeViewModel)this.ViewModelParent;
            parentVM.GoBackToPrimary(this.ActSelected);
        }
    }
}

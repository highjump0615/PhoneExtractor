using Forensics.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Forensics.ViewModel
{
    class MainToolViewModel : HostViewModel
    {
        /// <summary>
        /// 系统工具命令
        /// </summary>
        private ICommand _goToSystemCommand;
        public ICommand GoToSystemCommand
        {
            get { return _goToSystemCommand ?? (_goToSystemCommand = new DelegateCommand(GoToSytemToolPage)); }
        }

        /// <summary>
        /// 安卓工具命令
        /// </summary>
        private ICommand _goToAndroidCommand;
        public ICommand GoToAndroidCommand
        {
            get { return _goToAndroidCommand ?? (_goToAndroidCommand = new DelegateCommand(GoToAndroidToolPage)); }
        }

        /// <summary>
        /// 苹果工具命令
        /// </summary>
        private ICommand _goToAppleCommand;
        public ICommand GoToAppleCommand
        {
            get { return _goToAppleCommand ?? (_goToAppleCommand = new DelegateCommand(GoToAppleToolPage)); }
        }

        /// <summary>
        /// 附件工具命令
        /// </summary>
        private ICommand _goToOtherCommand;
        public ICommand GoToOtherCommand
        {
            get { return _goToOtherCommand ?? (_goToOtherCommand = new DelegateCommand(GoToOtherToolPage)); }
        }

        public override Pages PageIndex
        {
            get { return Pages.MainTool; }
        }

        public MainToolViewModel()
        {
            this.RegisterChild<ToolSystemViewModel>(() => new ToolSystemViewModel());
            this.RegisterChild<ToolAndroidViewModel>(() => new ToolAndroidViewModel());
            this.RegisterChild<ToolAppleViewModel>(() => new ToolAppleViewModel());
            this.RegisterChild<ToolOtherViewModel>(() => new ToolOtherViewModel());

            this.SelectedChild = GetChild(typeof(ToolSystemViewModel));
        }

        /// <summary>
        /// 跳转到系统工具页
        /// </summary>
        private void GoToSytemToolPage()
        {
            this.SelectedChild = GetChild(typeof(ToolSystemViewModel));
        }

        /// <summary>
        /// 跳转到安卓工具页
        /// </summary>
        private void GoToAndroidToolPage()
        {
            this.SelectedChild = GetChild(typeof(ToolAndroidViewModel));
        }

        /// <summary>
        /// 跳转到苹果工具页
        /// </summary>
        private void GoToAppleToolPage()
        {
            this.SelectedChild = GetChild(typeof(ToolAppleViewModel));
        }

        /// <summary>
        /// 跳转到附件工具页
        /// </summary>
        private void GoToOtherToolPage()
        {
            this.SelectedChild = GetChild(typeof(ToolOtherViewModel));
        }
    }
}

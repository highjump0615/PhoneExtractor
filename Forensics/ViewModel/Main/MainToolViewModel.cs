using Forensics.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Forensics.ViewModel
{
    public enum ToolsType
    {
        /// <summary>
        /// 系统工具
        /// </summary>
        System = 1,

        /// <summary>
        /// 安卓工具
        /// </summary>
        Android,

        /// <summary>
        /// 苹果工具
        /// </summary>
        Apple,

        /// <summary>
        /// 附件工具
        /// </summary>
        Other
    }

    public class MainToolViewModel : HostViewModel
    {
        /// <summary>
        /// 系统工具命令
        /// </summary>
        private ICommand _goToToolCommand;
        public ICommand GoToToolCommand
        {
            get { return _goToToolCommand ?? (_goToToolCommand = new DelegateCommand(GoToToolPage)); }
        }

        public override Pages PageIndex
        {
            get { return Pages.MainTool; }
        }

        public MainToolViewModel()
        {
            this.RegisterChild<ToolListViewModel>(() => new ToolListViewModel());

            // 默认是系统工具
            GoToToolPage(ToolsType.System);
        }

        /// <summary>
        /// 跳转到系统工具页
        /// </summary>
        private void GoToToolPage(object param)
        {
            this.SelectedChild = GetChild(typeof(ToolListViewModel));
            ((ToolListViewModel)this.SelectedChild).showTools((ToolsType)param);
        }
    }
}

using Forensics.BLL;
using Forensics.Command;
using Forensics.Model;
using Forensics.Util;
using System.Collections.Generic;
using System.Windows.Input;

namespace Forensics.ViewModel
{
    class ToolListViewModel : ViewModelBase
    {
        public List<Tool> listTool { get; set; }

        private ToolManager mToolManager = new ToolManager();
        private ToolUtil mUtil = new ToolUtil();

        public override Pages PageIndex
        {
            get { return Pages.ToolList; }
        }

        /// <summary>
        /// 打开工具命令
        /// </summary>
        private ICommand _openToolCommand;
        public ICommand OpenToolCommand
        {
            get { return _openToolCommand ?? (_openToolCommand = new DelegateCommand(OpenTool)); }
        }

        public ToolListViewModel()
        {
            listTool = new List<Tool>();
        }

        /// <summary>
        /// 显示工具
        /// </summary>
        /// <param name="type"></param>
        public void showTools(ToolsType type)
        {
            listTool.Clear();

            // 获取工具
            listTool = mToolManager.GetAllTools((int)type);
            PropertyChanging("listTool");
        }

        /// <summary>
        /// 打开相应的工具
        /// </summary>
        private void OpenTool(object param)
        {
            ToolUtil.OpenTool((Tool)param);
        }
    }
}

using Forensics.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Forensics.ViewModel
{
    public class AppleSyncViewModel : StepViewModel
    {
        public enum AppleSyncType
        {
            APPLESYNC,
            APPLEBYPASS,
        };
        private AppleSyncType Type;

        private string _buttonCaption = "开始提取";
        public string ButtonCaption
        {
            get {
                return "    " + _buttonCaption + "    ";
            }
            set { _buttonCaption = value; }
        }

        public AppleSyncViewModel(AppleSyncType type)
        {
            AppleStepViewModel stepVM;

            // 第一步
            stepVM = new AppleStepViewModel();
            stepVM.Desc = "第一步：请确认iTunes备份密码为空";
            this.AddChild(stepVM);

            if (type == AppleSyncType.APPLESYNC)
            {
                // 第二步
                stepVM = new AppleStepViewModel();
                stepVM.Desc = "第二步：第一次连接请注意信任选项";
                this.AddChild(stepVM);

                // 第三步
                stepVM = new AppleStepViewModel();
                stepVM.Desc = "第三步：备份目录空间为案件目录";
                stepVM.ShowSavePath = true;
                this.AddChild(stepVM);
            }
            else
            {
                // 第二步
                stepVM = new AppleStepViewModel();
                stepVM.Desc = "第一步：选择手机对应的lockdown文件";
                stepVM.ShowSavePath = true;
                stepVM.FileOpenType = 1;
                this.AddChild(stepVM);

                this.ButtonCaption = "找到lockdown文件";
            }

            this.Type = type;

            // 初始化
            initPages();
        }

        

    }
}

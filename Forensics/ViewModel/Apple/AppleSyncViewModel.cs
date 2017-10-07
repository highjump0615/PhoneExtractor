using Forensics.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Forensics.ViewModel
{
    public class AppleSyncViewModel : HostViewModel
    {
        private int nCurrentIndex = 0;

        public override Pages PageIndex => throw new NotImplementedException();

        public AppleSyncViewModel()
        {
            // 第一步
            AppleStepViewModel stepVM = new AppleStepViewModel();
            stepVM.Desc = "第一步：请确认iTunes备份密码为空";
            this.AddChild(stepVM);

            // 第二步
            stepVM = new AppleStepViewModel();
            stepVM.Desc = "第二步：第一次连接请注意信任选项";
            this.AddChild(stepVM);

            // 第三步
            stepVM = new AppleStepViewModel();
            stepVM.Desc = "第三步：备份目录空间为案件目录";
            stepVM.ShowSavePath = true;
            this.AddChild(stepVM);

            this.SelectedChild = GetChildAt(nCurrentIndex);
        }

        /// <summary>
        /// 下一步命令
        /// </summary>
        private ICommand _goToNextCommand;
        public ICommand GoToNextCommand
        {
            get { return _goToNextCommand ?? (_goToNextCommand = new DelegateCommand(GoToNextStep)); }
        }

        /// <summary>
        /// 上一步命令
        /// </summary>
        private ICommand _goToPrevCommand;
        public ICommand GoToPrevCommand
        {
            get { return _goToPrevCommand ?? (_goToPrevCommand = new DelegateCommand(GoToPrevStep)); }
        }

        /// <summary>
        /// 是否显示上一步按钮
        /// </summary>
        public bool IsPrevVisible
        {
            get
            {
                return nCurrentIndex > 0;
            }
        }

        /// <summary>
        /// 是否显示下一步按钮
        /// </summary>
        public bool IsNextVisible
        {
            get
            {
                return nCurrentIndex < this.GetCount() - 1;
            }
        }

        /// <summary>
        /// 是否显示上一步按钮
        /// </summary>
        public bool IsStartAvailable
        {
            get
            {
                return !IsNextVisible;
            }
        }

        /// <summary>
        /// 通知属性变化，更新界面
        /// </summary>
        private void updateControls()
        {
            PropertyChanging("IsNextVisible");
            PropertyChanging("IsPrevVisible");
            PropertyChanging("IsStartAvailable");
        }

        /// <summary>
        /// 跳到下一步
        /// </summary>
        private void GoToNextStep()
        {
            this.SelectedChild = GetChildAt(++nCurrentIndex);

            updateControls();
        }

        /// <summary>
        /// 跳到上一步
        /// </summary>
        private void GoToPrevStep()
        {
            this.SelectedChild = GetChildAt(--nCurrentIndex);

            updateControls();
        }

    }
}

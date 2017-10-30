using Forensics.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Forensics.ViewModel
{
    public class StepViewModel : HostViewModel
    {
        public override Pages PageIndex => throw new NotImplementedException();

        private int nCurrentIndex = 0;

        protected void initPages()
        {
            nCurrentIndex = 0;
            this.SelectedChild = GetChildAt(nCurrentIndex);
            updateControls();
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

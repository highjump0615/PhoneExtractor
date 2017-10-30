using Forensics.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Forensics.ViewModel
{
    class MainSettingViewModel : HostViewModel
    {
        public override Pages PageIndex
        {
            get { return Pages.Setting; }
        }

        /// <summary>
        /// 系统设置命令
        /// </summary>
        private ICommand _goToSettingCommand;
        public ICommand GoToSettingCommand
        {
            get { return _goToSettingCommand ?? (_goToSettingCommand = new DelegateCommand(GoToSettingPage)); }
        }

        /// <summary>
        /// 环境监测命令
        /// </summary>
        private ICommand _goToEnvCommand;
        public ICommand GoToEnvCommand
        {
            get { return _goToEnvCommand ?? (_goToEnvCommand = new DelegateCommand(GoToEnvPage)); }
        }

        /// <summary>
        /// 在线更新命令
        /// </summary>
        private ICommand _goToUpgradeCommand;
        public ICommand GoToUpgradeCommand
        {
            get { return _goToUpgradeCommand ?? (_goToUpgradeCommand = new DelegateCommand(GoToUpgradePage)); }
        }

        /// <summary>
        /// 用户反馈命令
        /// </summary>
        private ICommand _goToFeedbackCommand;
        public ICommand GoToFeedbackCommand
        {
            get { return _goToFeedbackCommand ?? (_goToFeedbackCommand = new DelegateCommand(GoToFeedbackPage)); }
        }

        /// <summary>
        /// 关于我们命令
        /// </summary>
        private ICommand _goToAboutCommand;
        public ICommand GoToAboutCommand
        {
            get { return _goToAboutCommand ?? (_goToAboutCommand = new DelegateCommand(GoToAboutPage)); }
        }

        public MainSettingViewModel()
        {
            this.RegisterChild<SettingSettingViewModel>(() => new SettingSettingViewModel());
            this.RegisterChild<SettingEnvViewModel>(() => new SettingEnvViewModel());
            this.RegisterChild<SettingFeedbackViewModel>(() => new SettingFeedbackViewModel());
            this.RegisterChild<SettingUpgradeViewModel>(() => new SettingUpgradeViewModel());
            this.RegisterChild<SettingAboutViewModel>(() => new SettingAboutViewModel());

            this.SelectedChild = GetChild(typeof(SettingSettingViewModel));
        }

        /// <summary>
        /// 跳转到系统设置
        /// </summary>
        private void GoToSettingPage()
        {
            this.SelectedChild = GetChild(typeof(SettingSettingViewModel));
        }

        /// <summary>
        /// 跳转到环境监测
        /// </summary>
        private void GoToEnvPage()
        {
            this.SelectedChild = GetChild(typeof(SettingEnvViewModel));
        }

        /// <summary>
        /// 跳转到在线更新
        /// </summary>
        private void GoToUpgradePage()
        {
            this.SelectedChild = GetChild(typeof(SettingUpgradeViewModel));
        }

        /// <summary>
        /// 跳转到用户反馈
        /// </summary>
        private void GoToFeedbackPage()
        {
            this.SelectedChild = GetChild(typeof(SettingFeedbackViewModel));
        }

        /// <summary>
        /// 跳转到关于我们
        /// </summary>
        private void GoToAboutPage()
        {
            this.SelectedChild = GetChild(typeof(SettingAboutViewModel));
        }

        public void SelectChildViewModel(Type viewModelType)
        {
            this.SelectedChild = GetChild(viewModelType);
        }
    }
}

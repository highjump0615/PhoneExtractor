using Forensics.Command;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Forensics.ViewModel
{
    public enum Pages
    {
        Main,
        MainHome,
        HomeHome,
        HomeExtract,
        MainData,
        DataCase,
        DataCaseDetail,
        MainTool,
        ToolList,
        ToolAndroid,
        ToolApple,
        ToolOther,
        Setting,
        SettingSetting,
        SettingEnv,
        SettingUpgrade,
        SettingFeedback,
        SettingAbout,

        Other,
    };

    public class MainViewModel : HostViewModel
    {
        /// <summary>
        /// 首页命令
        /// </summary>
        private ICommand _goToHomeCommand;
        public ICommand GoToHomeCommand
        {
            get { return _goToHomeCommand ?? (_goToHomeCommand = new DelegateCommand(GoToHomePage)); }
        }

        /// <summary>
        /// 资料管理页面命令
        /// </summary>
        private ICommand _goToDataCommand;
        public ICommand GoToDataCommand
        {
            get { return _goToDataCommand ?? (_goToDataCommand = new DelegateCommand(GoToDataPage)); }
        }

        /// <summary>
        /// 工具管理页面命令
        /// </summary>
        private ICommand _goToToolCommand;
        public ICommand GoToToolCommand
        {
            get { return _goToToolCommand ?? (_goToToolCommand = new DelegateCommand(GoToToolPage)); }
        }

        /// <summary>
        /// 设置页面命令
        /// </summary>
        private ICommand _goToSettingCommand;
        public ICommand GoToSettingCommand
        {
            get { return _goToSettingCommand ?? (_goToSettingCommand = new DelegateCommand(GoToSettingPage)); }
        }

        public override Pages PageIndex
        {
            get { return Pages.Main; }
        }

        public MainViewModel()
        {
            this.RegisterChild<MainHomeViewModel>(() => new MainHomeViewModel());
            this.RegisterChild<MainDataViewModel>(() => new MainDataViewModel());
            this.RegisterChild<MainToolViewModel>(() => new MainToolViewModel());
            this.RegisterChild<MainSettingViewModel>(() => new MainSettingViewModel());

            this.SelectedChild = GetChild(typeof(MainHomeViewModel));
        }

        protected override void OnDispose()
        {
            if (this.SelectedChild != null)
                ((ViewModelBase)SelectedChild).Dispose();

            base.OnDispose();
        }

        /// <summary>
        /// 跳转到首页
        /// </summary>
        private void GoToHomePage()
        {
            this.SelectedChild = GetChild(typeof(MainHomeViewModel));
        }

        /// <summary>
        /// 跳转到资料管理页面
        /// </summary>
        private void GoToDataPage()
        {
            this.SelectedChild = GetChild(typeof(MainDataViewModel));
        }
        
        /// <summary>
        /// 跳转到工具管理页面
        /// </summary>
        private void GoToToolPage()
        {
            this.SelectedChild = GetChild(typeof(MainToolViewModel));
        }

        /// <summary>
        /// 跳转到设置页面
        /// </summary>
        private void GoToSettingPage(object param)
        {
            this.SelectedChild = GetChild(typeof(MainSettingViewModel));

            ((MainSettingViewModel)this.SelectedChild).SelectChildViewModel((Type)param);
        }

        /// <summary>
        /// 跳转到提取页面
        /// </summary>
        public void GoToExtractPage()
        {
            this.SelectedChild = GetChild(typeof(MainHomeViewModel));
            ((MainHomeViewModel)this.SelectedChild).showExtractPage();
        }
    }
}

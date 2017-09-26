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
        MainData,
        MainTool,
        Setting,
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

        public override Pages PageIndex
        {
            get { return Pages.Main; }
        }

        public MainViewModel()
        {
            this.RegisterChild<MainHomeViewModel>(() => new MainHomeViewModel());
            this.RegisterChild<MainDataViewModel>(() => new MainDataViewModel());
            this.RegisterChild<MainToolViewModel>(() => new MainToolViewModel());

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
    }
}

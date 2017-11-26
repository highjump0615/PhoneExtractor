using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Forensics.ViewModel;
using Forensics.View.Apple;
using static Forensics.ViewModel.AppleSyncViewModel;
using Forensics.View;
using Forensics.View.Dialog;
using System.Threading;

namespace Forensics
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : WindowBase
    {
        public string ExtractPath { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            MainViewModel mainVM = new MainViewModel();
            mainVM.View = this;
            Globals.Instance.MainVM = mainVM;

            this.DataContext = mainVM;
            this.Closing += (s, e) => ((ViewModelBase)this.DataContext).Dispose();
        }

        /// <summary>
        /// Maximize/Restore window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onButMaximize(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
            }
            else
            {
                this.WindowState = WindowState.Maximized;
            }
        }

        /// <summary>
        /// Minimize window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onButMinimize(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        ///// <summary>
        ///// 隐藏所有菜单
        ///// </summary>
        private void hideMenus()
        {
            this.butAppleExtract.IsChecked = false;
            this.butAndroidExtract.IsChecked = false;
        }

        private void onMouseDown(object sender, MouseButtonEventArgs e)
        {
            hideMenus();
        }

        private void onButSettingItem(object sender, RoutedEventArgs e)
        {
            this.butSetting.IsChecked = false;
        }

        /// <summary>
        /// 苹果同步获取
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onButAppleSync(object sender, RoutedEventArgs e)
        {
            hideMenus();

            var windowAppleSync = new AppleSync(AppleSyncType.APPLESYNC);
            windowAppleSync.Owner = this;
            windowAppleSync.ShowDialog();

            if (windowAppleSync.DialogResult == true)
            {
                MainViewModel mainVM = (MainViewModel)this.DataContext;
                this.ExtractPath = windowAppleSync.ExtractPath;

                if (mainVM.CurrentDevice != null)
                {
                    // 打开提取页面
                    mainVM.GoToExtractPage(MainHomeViewModel.DeviceType.Apple, this.ExtractPath);
                }
                else
                {
                    // 打开等待连接窗口
                    openConnectWaiting();
                }
            }
        }

        /// <summary>
        /// 打开等待连接对话框
        /// </summary>
        public void openConnectWaiting()
        {
            hideMenus();

            var wWaiting = new DialogConnectWaiting();
            wWaiting.Owner = this;
            wWaiting.ShowDialog();
        }

        /// <summary>
        /// 打开连接成功对话框
        /// </summary>
        public void openConnectSuccess(MainHomeViewModel.DeviceType devType)
        {
            hideMenus();

            var wSuccess = new DialogConnectSuccess(devType);
            wSuccess.Owner = this;
            wSuccess.ShowDialog();

            MainViewModel mainVM = (MainViewModel)this.DataContext;
            if (wSuccess.DialogResult == false)
            {
                return;
            }

            this.ExtractPath = wSuccess.FileControl.TextPath.Text;

            // 苹果设备直接进入提取页面
            if (devType == MainHomeViewModel.DeviceType.Apple)
            {
                ((MainViewModel)this.DataContext).GoToExtractPage(devType, this.ExtractPath);
            }
            // 安卓设备要进入选择提取方式的界面
            else if (devType == MainHomeViewModel.DeviceType.Android)
            {
                var wExtractType = new DialogSelectExtractType();
                wExtractType.Owner = this;
                wExtractType.ShowDialog();

                if (wExtractType.DialogResult == false)
                {
                    return;
                }

                if (mainVM.CurrentDevice != null)
                {
                    // 打开提取页面
                    mainVM.GoToExtractPage(MainHomeViewModel.DeviceType.Android, this.ExtractPath);
                }
            }
        }

        /// <summary>
        /// 打开连接失败
        /// </summary>
        public void openConnectFail()
        {
            hideMenus();

            var wFail = new DialogConnectFail();
            wFail.Owner = this;
            wFail.ShowDialog();

            if (wFail.DialogResult == true)
            {
                hideMenus();

                var windowAndroid = new AndroidConnectAuto();
                windowAndroid.Owner = this;
                windowAndroid.ShowDialog();
            }
        }

        /// <summary>
        /// 打开添加物证
        /// </summary>
        public void openAddEvidence(string savePath)
        {
            hideMenus();

            var wEvidence = new DialogAddEvidence(savePath);
            wEvidence.Owner = this;
            wEvidence.ShowDialog();
        }

        /// <summary>
        /// 苹果密码绕过
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onButAppleBypass(object sender, RoutedEventArgs e)
        {
        }

        /// <summary>
        /// 安卓手动连接
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onAndroidManual(object sender, RoutedEventArgs e)
        {           
            hideMenus();

            var wDialog = new DialogSelectModel();
            wDialog.Owner = this;
            wDialog.ShowDialog();

            if (wDialog.DialogResult == true)
            {
                var wExtract = new DialogSelectExtractType();
                wExtract.Owner = this;
                wExtract.ShowDialog();
            }
        }

        /// <summary>
        /// 点击设置菜单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onButSettingClick(object sender, RoutedEventArgs e)
        {
            hideMenus();
        }
    }
}

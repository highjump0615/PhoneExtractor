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
                    mainVM.GoToExtractPage(MainHomeViewModel.ExtractType.Apple, this.ExtractPath);
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
        public void openConnectSuccess()
        {
            hideMenus();

            var wSuccess = new DialogConnectSuccess();
            wSuccess.Owner = this;
            wSuccess.ShowDialog();

            if (wSuccess.DialogResult == true)
            {
                this.ExtractPath = wSuccess.FileControl.TextPath.Text;
                ((MainViewModel)this.DataContext).GoToExtractPage(MainHomeViewModel.ExtractType.Apple, this.ExtractPath);
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
            //hideMenus();

            //var windowAppleBypass = new AppleSync(AppleSyncType.APPLEBYPASS);
            //windowAppleBypass.Owner = this;
            //windowAppleBypass.ShowDialog();

            //CommonUtil.CurrentPD.IMEI_string = imei;
            //CommonUtil.CurrentPD.Phone_os = "iOS " + labelVersion.Text;
            //CommonUtil.CurrentPD.Phone_model = productType;
            //CommonUtil.CurrentPD.Phone_brand = "Apple";
            //CommonUtil.CurrentPD.Case_ai_file = applexmlPath;
            openAddEvidence("f:\\temp\\Data");
        }

        /// <summary>
        /// 安卓自动连接
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onButAndroidConnectAuto(object sender, RoutedEventArgs e)
        {
            hideMenus();

            var windowAndroid = new AndroidConnectAuto();
            windowAndroid.Owner = this;
            windowAndroid.ShowDialog();
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

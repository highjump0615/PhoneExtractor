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

namespace Forensics
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : WindowBase
    {
        public MainWindow()
        {
            InitializeComponent();

            MainViewModel mainVM = new MainViewModel();
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
                ((MainViewModel)this.DataContext).GoToExtractPage(MainHomeViewModel.ExtractType.Apple);
            }
        }

        /// <summary>
        /// 苹果密码绕过
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onButAppleBypass(object sender, RoutedEventArgs e)
        {
            hideMenus();

            var windowAppleBypass = new AppleSync(AppleSyncType.APPLEBYPASS);
            windowAppleBypass.Owner = this;
            windowAppleBypass.ShowDialog();
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

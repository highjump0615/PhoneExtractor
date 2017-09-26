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

namespace Forensics
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = new MainViewModel();
            this.Closing += (s, e) => ((ViewModelBase)this.DataContext).Dispose();
        }

        /// <summary>
        /// Close window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onButClose(object sender, RoutedEventArgs e)
        {
            this.Close();
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
    }
}

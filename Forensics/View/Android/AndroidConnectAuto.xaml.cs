using Forensics.ViewModel.Android;
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
using System.Windows.Shapes;

namespace Forensics.View
{
    /// <summary>
    /// Interaction logic for AndroidConnectAuto.xaml
    /// </summary>
    public partial class AndroidConnectAuto : WindowBase
    {
        public AndroidConnectAuto()
        {
            InitializeComponent();

            this.DataContext = new AndroidConnectViewModel();
        }

        /// <summary>
        /// 返回
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onButBack(object sender, RoutedEventArgs e)
        {
            onButClose(sender, e);
        }
    }
}

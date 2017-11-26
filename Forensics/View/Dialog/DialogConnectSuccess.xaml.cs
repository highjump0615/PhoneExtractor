using Forensics.ViewModel;
using Forensics.ViewModel.Dialog;
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

namespace Forensics.View.Dialog
{
    /// <summary>
    /// Interaction logic for DialogConnectSucess.xaml
    /// </summary>
    public partial class DialogConnectSuccess : Window
    {
        public DialogConnectSuccess(MainHomeViewModel.DeviceType devType)
        {
            InitializeComponent();

            this.DataContext = new DialogSuccessViewModel(devType);
        }

        /// <summary>
        /// 下一步
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onButNext(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}

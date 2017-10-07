using Forensics.ViewModel;
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
using static Forensics.ViewModel.AppleSyncViewModel;

namespace Forensics.View.Apple
{
    /// <summary>
    /// Interaction logic for AppleSync.xaml
    /// </summary>
    public partial class AppleSync : WindowBase
    {
        public AppleSync(AppleSyncType type)
        {
            InitializeComponent();

            this.DataContext = new AppleSyncViewModel(type);
        }

        /// <summary>
        /// 开始提取
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onButStart(object sender, RoutedEventArgs e)
        {
            Globals.Instance.MainVM.GoToExtractPage();
            onButClose(sender, e);
        }
    }
}

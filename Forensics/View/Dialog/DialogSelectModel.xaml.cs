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
    /// Interaction logic for DialogSelectModel.xaml
    /// </summary>
    public partial class DialogSelectModel : WindowBase
    {
        private double mdScrollOffset = 4;

        public DialogSelectModel()
        {
            InitializeComponent();

            this.DataContext = new DialogManualViewModel();
        }

        private void onButPhoneScrollBack(object sender, RoutedEventArgs e)
        {
            scrollPhoneListbox(-mdScrollOffset);
        }

        private void onButPhoneScrollForward(object sender, RoutedEventArgs e)
        {
            scrollPhoneListbox(mdScrollOffset);
        }

        private void scrollPhoneListbox(double offset)
        {
            Border border = (Border)VisualTreeHelper.GetChild(this.lbPhones, 0);
            ScrollViewer sv = (ScrollViewer)VisualTreeHelper.GetChild(border, 0);

            sv.ScrollToHorizontalOffset(sv.HorizontalOffset + offset);
        }
        
        /// <summary>
        /// 点击开始连接
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onButConnect(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}

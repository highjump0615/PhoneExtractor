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

namespace Forensics
{
    /// <summary>
    /// Interaction logic for SettingSetting.xaml
    /// </summary>
    public partial class SettingSetting : UserControl
    {
        public SettingSetting()
        {
            InitializeComponent();
        }

        private void setFolderPath(TextBox tb)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                dialog.SelectedPath = this.TextCaseDirectory.Text;

                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                tb.Text = dialog.SelectedPath;
            }
        }

        /// <summary>
        /// 案件默认路径
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onBrowseDirectoryCase(object sender, RoutedEventArgs e)
        {
            setFolderPath(this.TextCaseDirectory);
        }

        /// <summary>
        /// 彩虹表默认路径
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onBrowseDirectoryWc(object sender, RoutedEventArgs e)
        {
            setFolderPath(this.TextWcDirectory);
        }

        /// <summary>
        /// 离线地图默认路径
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onBrowseDirectoryMap(object sender, RoutedEventArgs e)
        {
            setFolderPath(this.TextMapDirectory);
        }
    }
}

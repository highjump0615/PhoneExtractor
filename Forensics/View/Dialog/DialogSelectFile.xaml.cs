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

namespace Forensics.View.Dialog
{
    /// <summary>
    /// Interaction logic for DialogSelectFile.xaml
    /// </summary>
    public partial class DialogSelectFile : UserControl
    {
        public int Type
        {
            get { return (int)GetValue(TypeProperty); }
            set { SetValue(TypeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Title.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TypeProperty =
            DependencyProperty.Register(
                "Type",
                typeof(int),
                typeof(DialogSelectFile),
                null
            );

        public DialogSelectFile()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 点击浏览
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void onButBrowse(object sender, RoutedEventArgs e)
        {
            // 文件夹
            if (this.Type == 0)
            {
                var dialog = new System.Windows.Forms.FolderBrowserDialog();
                dialog.SelectedPath = this.TextPath.Text;

                dialog.ShowDialog();
                this.TextPath.Text = dialog.SelectedPath;
            }
            // 文件
            else if (this.Type == 1)
            {
                var dialog = new System.Windows.Forms.OpenFileDialog();
                dialog.Filter = "文件格式|*.plist";
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.Cancel)
                {
                    // 点击了取消，直接退出
                    return;
                }

                this.TextPath.Text = dialog.FileName;
            }
        }
    }
}

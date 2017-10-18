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
        public DialogSelectFile()
        {
            InitializeComponent();
        }

        private void onButBrowse(object sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                dialog.SelectedPath = this.TextPath.Text;

                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                this.TextPath.Text = dialog.SelectedPath;
            }
        }
    }
}

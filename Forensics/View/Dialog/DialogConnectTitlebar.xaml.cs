using Forensics.Util;
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
    /// Interaction logic for DialogConnectTitlebar.xaml
    /// </summary>
    public partial class DialogConnectTitlebar : UserControl
    {
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public bool MaximizeAvailable
        {
            get { return (bool)GetValue(MaximizeAvailableProperty); }
            set { SetValue(MaximizeAvailableProperty, value); }
        }


        // Using a DependencyProperty as the backing store for Title.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(
                "Title", 
                typeof(string), 
                typeof(DialogConnectTitlebar), 
                null
            );

        public static readonly DependencyProperty MaximizeAvailableProperty =
            DependencyProperty.Register(
                "MaximizeAvailable",
                typeof(bool),
                typeof(DialogConnectTitlebar),
                null
            );

        public DialogConnectTitlebar()
        {
            InitializeComponent();

            this.Title = "连接设备";
            this.MaximizeAvailable = false;
        }

        private void onButMaximize(object sender, RoutedEventArgs e)
        {
            WindowBase parent = (WindowBase)CommonUtil.GetParentWindow(this);
            parent.onButMaximize(sender, e);
        }
    }
}

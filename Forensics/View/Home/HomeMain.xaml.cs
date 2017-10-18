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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Forensics.View
{
    /// <summary>
    /// Interaction logic for HomeMain.xaml
    /// </summary>
    public partial class HomeMain : UserControl
    {
        public HomeMain()
        {
            InitializeComponent();
        }

        private void onDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            // VM里设置view
            ViewModelBase vm = (ViewModelBase)e.NewValue;
            if (vm != null)
            {
                vm.View = this;
            }
        }
    }
}

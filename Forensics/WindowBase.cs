using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Forensics
{
    public class WindowBase : Window
    {
        public WindowBase()
        {
        }

        /// <summary>
        /// Close window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void onButClose(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

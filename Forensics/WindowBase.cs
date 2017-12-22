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

        /// <summary>
        /// Maximize/Restore window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void onButMaximize(object sender, RoutedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                this.WindowState = WindowState.Normal;
            }
            else
            {
                this.WindowState = WindowState.Maximized;
            }
        }
    }
}

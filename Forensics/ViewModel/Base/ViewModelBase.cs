using log4net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Forensics.ViewModel
{
    public abstract class ViewModelBase : ObservableObject, IDisposable
    {
        public abstract Pages PageIndex { get; }
        public Control View { get; set; }
        public ViewModelBase ViewModelParent { get; set; }

        private static readonly ILog Log = LogManager.GetLogger(typeof(ViewModelBase));

        protected String _clew = "操作提示";

        protected virtual void OnDispose()
        {
            Console.WriteLine(string.Format("Disposing {0} , {1}", this.GetType().Name, this.GetType().FullName));
        }

        public void Dispose()
        {
            OnDispose();
        }

        /// <summary>
        /// 添加log
        /// </summary>
        /// <param name="message"></param>
        protected void saveErrorLog(string message)
        {
            try
            {
                Log.Error(message, null);
            }
            catch (Exception ex)
            {
                Log.Error(ex.Data.ToString(), ex);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forensics.ViewModel
{
    public abstract class ViewModelBase : ObservableObject, IDisposable
    {
        public abstract Pages PageIndex { get; }

        protected virtual void OnDispose()
        {
            Console.WriteLine(string.Format("Disposing {0} , {1}", this.GetType().Name, this.GetType().FullName));
        }

        public void Dispose()
        {
            OnDispose();
        }
    }
}

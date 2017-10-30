using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Forensics.ViewModel
{
    public abstract class ObservableObject : INotifyPropertyChanged
    {
        protected void SetPropertyValue<T>(ref T target, T value, [CallerMemberName] string caller = null)
        {
            if (object.Equals(target, value))
                return;
            target = value;
            PropertyChanging(caller);
        }

        public void PropertyChanging(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}

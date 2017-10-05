using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Forensics.ViewModel
{
    public abstract class HostViewModel : ViewModelBase
    {
        private readonly Dictionary<Type, Func<ViewModelBase>> _childrenMap = new Dictionary<Type, Func<ViewModelBase>>();

        private ViewModelBase _selectedChild;


        public ViewModelBase SelectedChild
        {
            get { return _selectedChild; }
            set
            {
                if (_selectedChild != null && _selectedChild.PageIndex == value.PageIndex)
                    return;

                SetPropertyValue(ref _selectedChild, value);
            }
        }

        protected void RegisterChild<T>(Func<T> getter) where T : ViewModelBase
        {
            Contract.Requires(getter != null);

            if (_childrenMap.ContainsKey(typeof(T)))
                return;

            _childrenMap.Add(typeof(T), getter);
        }

        protected ViewModelBase GetChild(Type type)
        {
            Contract.Requires(type != null);
            if (_childrenMap.ContainsKey(type) == false)
                throw new InvalidOperationException("Can't resolve type " + type.ToString());

            var viewModel = _childrenMap[type];
            return viewModel();
        }
    }
}

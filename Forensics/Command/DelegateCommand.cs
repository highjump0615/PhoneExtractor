using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Forensics.Command
{
    class DelegateCommand : ICommand
    {
        Action _action;
        Action<object> _parameterizedAction;
        Predicate<object> _canExecute;

        public DelegateCommand(Action action)
        {
            _action = action;
        }

        public DelegateCommand(Action<object> parameterizedAction)
        {
            _parameterizedAction = parameterizedAction;
        }

        public DelegateCommand(Action action, Predicate<object> canExecute)
            : this(action)
        {
            _canExecute = canExecute;
        }

        public DelegateCommand(Action<object> parameterizedAction, Predicate<object> canExecute)
            : this(parameterizedAction)
        {
            _canExecute = canExecute;
        }

        #region ICommand Members

        public bool CanExecute(object parameter)
        {
            return _canExecute == null ? true : _canExecute(parameter);
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            if (_action != null) _action();
            if (_parameterizedAction != null) _parameterizedAction(parameter);
        }

        #endregion
    }
}

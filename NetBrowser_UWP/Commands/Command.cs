using System;
using System.Windows.Input;

namespace NetBrowser_UWP.Commands
{
    public class Command : ICommand
    {
        private readonly Action<object> _execute;
        private Func<object, bool> _canExecute;

        public Command(Action<object> execute)
        {
            _execute = execute;
        }
        public Command(Action<object> execute, Func<object, bool> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            var canExecute = _canExecute == null || _canExecute(parameter);
            return canExecute;
        }
        public void RaiseCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
                CanExecuteChanged(this, EventArgs.Empty);
        }
        public void Execute(object parameter)
        {
            _execute(parameter);
        }
    }
}

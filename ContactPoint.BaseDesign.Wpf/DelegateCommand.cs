using System;
using System.Windows.Input;

namespace ContactPoint.BaseDesign.Wpf
{
    public class DelegateCommand : ICommand
    {
        private readonly Action<object> _handler;
        private readonly Func<object, bool> _canExecuteHandler;

        #pragma warning disable 0067
        public event EventHandler CanExecuteChanged;
        #pragma warning restore 0067

        public DelegateCommand(Action<object> handler)
        {
            _handler = handler;
        }

        public DelegateCommand(Action<object> handler, Func<object, bool> canExecuteHandler)
            : this(handler)
        {
            _canExecuteHandler = canExecuteHandler;
        }

        public void Execute(object parameter)
        {
            _handler(parameter);
        }

        public bool CanExecute(object parameter)
        {
            return _canExecuteHandler == null || _canExecuteHandler(parameter);
        }
    }
}

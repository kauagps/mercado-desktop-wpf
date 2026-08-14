using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace mercado.ViewModel
{
    public class RelayCommand : ICommand
    {
        private Action _execute;

        public RelayCommand(Action execute)
        {
            _execute = execute;
        }

        public bool CanExecute(object ? parameter)
        {
            return true;
        }

        public void Execute(object ? parameter)
        {
            _execute();
        }

        public event EventHandler ? CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
    }
}

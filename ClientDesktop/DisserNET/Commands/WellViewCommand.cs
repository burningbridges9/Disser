using DisserNET.ViewModels;
using System;
using System.Windows.Input;

namespace DisserNET.Commands
{
    abstract public class WellViewCommand : ICommand
    {
        protected WellViewModel _wvm;
        public WellViewCommand(WellViewModel wvm)
        {
            _wvm = wvm;
        }
        public event EventHandler CanExecuteChanged;
        public abstract bool CanExecute(object parameter);
        public abstract void Execute(object parameter);
    }
}

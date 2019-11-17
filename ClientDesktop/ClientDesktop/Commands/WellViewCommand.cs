using ClientDesktop.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ClientDesktop.Commands
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

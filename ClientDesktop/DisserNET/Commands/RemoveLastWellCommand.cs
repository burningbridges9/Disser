using DisserNET.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisserNET.Commands
{
    public class RemoveLastWellCommand : WellViewCommand
    {
        public RemoveLastWellCommand(WellViewModel wvm) : base(wvm)
        {
        }
        public override bool CanExecute(object parameter)
        {
            return true;
        }
        public override void Execute(object parameter)
        {
            if (_wvm.Wells.Count > 0)
                _wvm.Wells.RemoveAt(_wvm.Wells.Count - 1);
            //_wvm.SelectedWell = _wvm.Wells.Last();
        }
    }
}

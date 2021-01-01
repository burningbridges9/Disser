using DisserNET.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisserNET.Commands
{
    public class DeleteAllWellCommand : WellViewCommand
    {
        public DeleteAllWellCommand(WellViewModel wvm) : base(wvm)
        {
        }
        public override bool CanExecute(object parameter)
        {
            return true;
        }
        public override void Execute(object parameter)
        {
            if (_wvm.Wells.Count > 0)
                _wvm.Wells.Clear();
            //_wvm.SelectedWell = _wvm.Wells.Last();
        }
    }
}

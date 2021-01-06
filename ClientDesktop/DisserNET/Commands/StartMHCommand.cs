using DisserNET.Calculs;
using DisserNET.Models;
using DisserNET.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisserNET.Commands
{
    public class StartMHCommand : MetropolisHastingsViewCommand
    {
        public StartMHCommand(MetropolisHastingsViewModel vm) : base(vm)
        {

        }

        public override bool CanExecute(object parameter) => true;

        public override void Execute(object parameter)
        {
            mhvm.AcceptedValues = mhvm.Mode == Mode.Direct ? 
                Functions.MetropolisHastingsAlgorithmForConsumptions(mhvm.WellsList, mhvm.MetropolisHastings, mhvm.Mode) :
                Functions.MetropolisHastingsAlgorithmForPressures(mhvm.WellsList, mhvm.MetropolisHastings, mhvm.Mode);
            mhvm.Save();
        }
    }
}

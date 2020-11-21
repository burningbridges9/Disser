using HydrodynamicStudies.Calculs;
using HydrodynamicStudies.Models;
using HydrodynamicStudies.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydrodynamicStudies.Commands
{
    public class StartMHCommand : MetropolisHastingsViewCommand
    {
        public StartMHCommand(MetropolisHastingsViewModel vm) : base(vm)
        {

        }

        public override bool CanExecute(object parameter) => true;

        public override void Execute(object parameter)
        {
            var wl = new WellsList(MainWindow.MainViewModel.WellViewModel.Wells.ToList());
            var mode = wl.Wells.First().Mode;
            var result = Functions.MetropolisHastingsAlgorithm(wl, metropolisHastingsViewModel.MetropolisHastings, mode);
        }
    }
}

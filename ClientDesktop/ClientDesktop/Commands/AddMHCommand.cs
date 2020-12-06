using HydrodynamicStudies.Models;
using HydrodynamicStudies.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HydrodynamicStudies.Commands
{
    public class AddMHCommand : MetropolisHastingsViewCommand
    {
        public AddMHCommand(MetropolisHastingsViewModel vm) : base(vm)
        {

        }

        public override bool CanExecute(object parameter) => true;

        public override void Execute(object parameter)
        {
            string[] parameters = ((object[])parameter).Select(p=>p.ToString()).ToArray();
            MetropolisHastings metropolisHastings = new MetropolisHastings()
            {
                WalksCount = int.Parse(parameters[0]),
                Ns = int.Parse(parameters[1]),
                C = double.Parse(parameters[2]),
                S_0 = double.Parse(parameters[3]),
                MinK = Math.Pow(10.0, -15) * double.Parse(parameters[4]),
                MaxK = Math.Pow(10.0, -15) * double.Parse(parameters[5]),
                MinKappa = (1.0 / 3600.0) * double.Parse(parameters[6]),
                MaxKappa = (1.0 / 3600.0) * double.Parse(parameters[7]),
                MinKsi = double.Parse(parameters[8]),
                MaxKsi = double.Parse(parameters[9]),
                MinP0 = Math.Pow(10.0, 6) * double.Parse(parameters[10]),
                MaxP0 = Math.Pow(10.0, 6) * double.Parse(parameters[11]),
                IncludedK = bool.Parse(parameters[12]),
                IncludedKappa = bool.Parse(parameters[13]),
                IncludedKsi = bool.Parse(parameters[14]),
                IncludedP0 = bool.Parse(parameters[15]),
                StepK = Math.Pow(10.0, -15) * double.Parse(parameters[16]),
                StepKappa = (1.0 / 3600.0) * double.Parse(parameters[17]),
                StepKsi = double.Parse(parameters[18]),
                StepP0 = Math.Pow(10.0, 6) * double.Parse(parameters[19]),
            };
            metropolisHastingsViewModel.MetropolisHastings = metropolisHastings;
        }
    }
}

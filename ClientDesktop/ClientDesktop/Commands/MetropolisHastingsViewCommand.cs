using HydrodynamicStudies.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace HydrodynamicStudies.Commands
{
    abstract public class MetropolisHastingsViewCommand : ICommand
    {
        protected MetropolisHastingsViewModel metropolisHastingsViewModel;
        public MetropolisHastingsViewCommand(MetropolisHastingsViewModel metropolisHastingsViewModel)
        {
            this.metropolisHastingsViewModel = metropolisHastingsViewModel;
        }

        public event EventHandler CanExecuteChanged;

        public abstract bool CanExecute(object parameter);

        public abstract void Execute(object parameter);
    }
}

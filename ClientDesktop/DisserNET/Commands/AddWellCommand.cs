using DisserNET.Models;
using DisserNET.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DisserNET.Commands
{
    public class AddWellCommand : WellViewCommand
    {
        public AddWellCommand(WellViewModel wvm) : base(wvm)
        {
        }
        public override bool CanExecute(object parameter)
        {
            return true;
        }
        public override void Execute(object parameter)
        {
            var parameters = (object[])parameter;
            Well well = new Well
            {
                Q = 1.0 / (24.0 * 3600.0) * Convert.ToDouble(parameters[0]),
                P = Math.Pow(10.0, 6) * Convert.ToDouble(parameters[1]),
                P0 = Math.Pow(10.0, 6) * Convert.ToDouble(parameters[2]),
                Time1 = 3600.0 * Convert.ToDouble(parameters[3]),
                Time2 = 3600.0 * Convert.ToDouble(parameters[4]),
                H0 = Convert.ToDouble(parameters[5]),
                Mu = Math.Pow(10.0, -3) * Convert.ToDouble(parameters[6]),
                Rw = Convert.ToDouble(parameters[7]),
                K = Math.Pow(10.0, -15) * Convert.ToDouble(parameters[8]),
                Kappa = (1.0 / 3600.0) * Convert.ToDouble(parameters[9]),
                Rs = Convert.ToDouble(parameters[10]),
                Ksi = Convert.ToDouble(parameters[11]),
                N = Convert.ToInt32(parameters[12]),
            };
            _wvm.Wells.Add(well);
            _wvm.SelectedWell = well;
        }
    }

    public class AddAutoWellCommand : WellViewCommand
    {
        public AddAutoWellCommand(WellViewModel wvm) : base(wvm)
        {
        }
        public override bool CanExecute(object parameter)
        {
            return true;
        }
        public override void Execute(object parameter)
        {
            var parameters = (object[])parameter;
            for (int i = 1; i <= 3; i++)
            {
                Well well = new Well
                {
                    Q = 1.0 / (24.0 * 3600.0) * Convert.ToDouble(parameters[0]) * i,
                    P = Math.Pow(10.0, 6) * Convert.ToDouble(parameters[1]) * i,
                    P0 = Math.Pow(10.0, 6) * Convert.ToDouble(parameters[2]),
                    Time1 = 3600.0 * Convert.ToDouble(parameters[4]) * (i-1),
                    Time2 = 3600.0 * Convert.ToDouble(parameters[4]) * i,
                    H0 = Convert.ToDouble(parameters[5]),
                    Mu = Math.Pow(10.0, -3) * Convert.ToDouble(parameters[6]),
                    Rw = Convert.ToDouble(parameters[7]),
                    K = Math.Pow(10.0, -15) * Convert.ToDouble(parameters[8]),
                    Kappa = (1.0 / 3600.0) * Convert.ToDouble(parameters[9]),
                    Rs = Convert.ToDouble(parameters[10]),
                    Ksi = Convert.ToDouble(parameters[11]),
                    N = Convert.ToInt32(parameters[12]),
                };
                _wvm.Wells.Add(well);
                _wvm.SelectedWell = well;
            }
        }
    }
}

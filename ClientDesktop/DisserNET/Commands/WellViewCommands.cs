using DisserNET.Calculs;
using DisserNET.Models;
using DisserNET.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
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
                    Time1 = 3600.0 * Convert.ToDouble(parameters[4]) * (i - 1),
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
                _wvm.Wells.Remove(_wvm.Wells.LastOrDefault());
        }
    }
    public class CalculatePressuresCommand : WellViewCommand
    {
        public Action<PGradientAndPressures> CommandExecuted;
        public CalculatePressuresCommand(WellViewModel wvm) : base(wvm)
        { }

        public override bool CanExecute(object parameter)
        {
            return true;
        }

        public override void Execute(object parameter)
        {
            if (_wvm.ConsumptionsAndTimes == null || _wvm.ConsumptionsAndTimes?.Consumptions.Count == 0)
                _wvm.Wells.ToList().ForEach(w => w.Mode = Mode.Direct);
            
            _wvm.PressuresAndTimes = CalculatePressures();
            _wvm.SetupPressurePerTimeDatas();
        }

        public PressuresAndTimes CalculatePressures()
        {
            var res = Functions.GetPressures(new WellsList(_wvm.Wells.ToList()));
            var grAndPres = new PGradientAndPressures()
            {
                Grad = new PGradient
                {
                    ChangedK = _wvm.Wells[0].K,
                    ChangedKappa = _wvm.Wells[0].Kappa,
                    ChangedKsi = _wvm.Wells[0].Ksi,
                    ChangedP0 = _wvm.Wells[0].P0,
                    FminP = Functions.GetObjectFunctionValue(_wvm.Wells.ToArray())
                },
                ValuesAndTimes = res
            };
            CommandExecuted?.Invoke(grAndPres);
            return res;
        }
    }


    public class CalculateConsumptionsCommand : WellViewCommand
    {
        public Action<QGradientAndConsumptions> CommandExecuted;
        public CalculateConsumptionsCommand(WellViewModel wvm) : base(wvm)
        { }

        public override bool CanExecute(object parameter)
        {
            return true;
        }
        public override void Execute(object parameter)
        {
            if (_wvm.PressuresAndTimes?.Pressures1f.Count == 0 || _wvm.PressuresAndTimes == null)
                _wvm.Wells.ToList().ForEach(w => w.Mode = Mode.Reverse);

            _wvm.ConsumptionsAndTimes = CalculateConsumptions();
            _wvm.SetupConsumptionsPerTimeDatas();
        }

        public ConsumptionsAndTimes CalculateConsumptions() 
        {
            var res = Functions.GetConsumptions(new WellsList(_wvm.Wells.ToList()));            
            var grAndCons = new QGradientAndConsumptions()
            {
                Grad = new QGradient
                {
                    ChangedK = _wvm.Wells[0].K,
                    ChangedKappa = _wvm.Wells[0].Kappa,
                    ChangedKsi = _wvm.Wells[0].Ksi,
                    ChangedP0 = _wvm.Wells[0].P0,
                    FminQ = Functions.GetObjectFunctionValue(_wvm.Wells.ToArray())
                },
                ValuesAndTimes = res
            };
            CommandExecuted?.Invoke(grAndCons);
            return res;
        }

}

    public class ClearCommand : WellViewCommand
    {
        public Action CommandExecuted;
        public ClearCommand(WellViewModel wvm) : base(wvm)
        { }

        public override bool CanExecute(object parameter)
        {
            return true;
        }

        public override void Execute(object parameter)
        {
            _wvm.PressuresAndTimes?.Pressures1f?.Clear();
            _wvm.PressuresAndTimes?.Pressures1s?.Clear();
            _wvm.PressuresAndTimes?.Pressures2f?.Clear();
            _wvm.PressuresAndTimes?.Pressures2s?.Clear();
            _wvm.PressuresAndTimes?.Pressures3?.Clear();
            _wvm.PressuresAndTimes?.Times1f?.Clear();
            _wvm.PressuresAndTimes?.Times1s?.Clear();
            _wvm.PressuresAndTimes?.Times2f?.Clear();
            _wvm.PressuresAndTimes?.Times2s?.Clear();
            _wvm.PressuresAndTimes?.Times3?.Clear();
            _wvm.ConsumptionsAndTimes?.Consumptions?.Clear();
            _wvm.ConsumptionsAndTimes?.StaticConsumptions?.Clear();
            _wvm.ConsumptionsAndTimes?.Times?.Clear();
            CommandExecuted?.Invoke();
        }
    }

}

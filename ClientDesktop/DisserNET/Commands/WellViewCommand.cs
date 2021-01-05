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

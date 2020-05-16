using ClientDesktop.Calculs;
using ClientDesktop.Models;
using ClientDesktop.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ClientDesktop.Commands
{
    abstract public class MainViewCommand : ICommand
    {
        protected MainViewModel _mvm;
        public MainViewCommand(MainViewModel mvm)
        {
            _mvm = mvm;
        }
        public event EventHandler CanExecuteChanged;
        public abstract bool CanExecute(object parameter);
        public abstract void Execute(object parameter);
    }

    public class CalculatePressuresCommand : MainViewCommand
    {
        public CalculatePressuresCommand(MainViewModel mvm) : base(mvm)
        { }

        public override bool CanExecute(object parameter)
        {
            return true;
        }

        public override void Execute(object parameter)
        {
            if (_mvm.ConsumptionsAndTimes == null || _mvm.ConsumptionsAndTimes?.Consumptions.Count == 0)
                for (int i = 0; i < _mvm.WellViewModel.Wells.Count; i++)
                    _mvm.WellViewModel.Wells[i].Mode = Mode.Direct;
            _mvm.PressuresAndTimes = SendWellsForPressures();
            _mvm.PlotViewModel.PlotTimePressures(_mvm.PressuresAndTimes);
            CalculateInitialFminP();
        }

        public PressuresAndTimes SendWellsForPressures()
        {
            WellsList wellsList = new WellsList(_mvm.WellViewModel.Wells.ToList());
            PressuresAndTimes pressuresAndTimes = RealMagic.GetPressures(wellsList);
            // make check
            _mvm.WellViewModel.Wells[0].CalculatedP = pressuresAndTimes.Pressures1f.Last();
            _mvm.WellViewModel.Wells[1].CalculatedP = pressuresAndTimes.Pressures2f.Last();
            _mvm.WellViewModel.Wells[2].CalculatedP = pressuresAndTimes.Pressures3.Last();
            return pressuresAndTimes;
        }

        public double CalculateInitialFminP()
        {
            if (_mvm.PGradientViewModel.PGradientAndPressures.Count != 0)
                _mvm.PGradientViewModel.PGradientAndPressures.Clear();
            PGradient g = new PGradient
            {
                ChangedK = _mvm.WellViewModel.Wells[0].K,
                ChangedKappa = _mvm.WellViewModel.Wells[0].Kappa,
                ChangedKsi = _mvm.WellViewModel.Wells[0].Ksi,
                ChangedP0 = _mvm.WellViewModel.Wells[0].P0
            };
            _mvm.PGradientViewModel.PGradientAndPressures.Add(new PGradientAndPressures());
            _mvm.PGradientViewModel.PGradientAndPressures[0].PGradient = g;
            _mvm.PGradientViewModel.PGradientAndPressures[0].PressuresAndTimes = _mvm.PressuresAndTimes;
            double Fmin = 0;
            switch (_mvm.WellViewModel.Wells.Count)
            {
                //case 1:
                //    Fmin = Math.Pow((wellViewModel.Wells[0].Q - Qk1.Last()), 2);
                //    Fmin = Math.Sqrt(Fmin / (Math.Pow(wellViewModel.Wells[0].Q, 2)));
                //    break;
                //case 2:
                //    Fmin = Math.Pow((wellViewModel.Wells[0].Q - Qk1[indexes[0] - 2]), 2) + Math.Pow((wellViewModel.Wells[1].Q - Qk1.Last()), 2);
                //    Fmin = Math.Sqrt(Fmin / (Math.Pow(wellViewModel.Wells[0].Q, 2) + Math.Pow(wellViewModel.Wells[1].Q, 2)));
                //    break;
                case 3:
                    Fmin = Math.Pow((_mvm.WellViewModel.Wells[0].P - _mvm.PressuresAndTimes.Pressures1f.Last()), 2)
                            + Math.Pow((_mvm.WellViewModel.Wells[1].P - _mvm.PressuresAndTimes.Pressures2f.Last()), 2)
                            + Math.Pow((_mvm.WellViewModel.Wells[2].P - _mvm.PressuresAndTimes.Pressures3.Last()), 2);
                    Fmin = Fmin / (Math.Pow(_mvm.WellViewModel.Wells[0].P, 2) + Math.Pow(_mvm.WellViewModel.Wells[1].P, 2) + Math.Pow(_mvm.WellViewModel.Wells[2].P, 2));
                    break;
            }
            _mvm.PGradientViewModel.PGradientAndPressures[0].PGradient.FminP = Fmin;
            //GradientClc.GradientToShow = GradientsAndConsumptions.Last().Gradient;
            _mvm.PGradientViewModel.SelectedGradient = _mvm.PGradientViewModel.PGradientAndPressures.Last().PGradient;
            _mvm.PGradientViewModel.Gradients.Add(_mvm.PGradientViewModel.PGradientAndPressures[0].PGradient);
            //GradientClc.FQmin.Text = gradientViewModel.SelectedGradient.FminQ.ToString();
            return Fmin;
        }
    }

    public class CalculateConsumptionsCommand : MainViewCommand
    {
        public CalculateConsumptionsCommand(MainViewModel mvm) : base(mvm)
        { }

        public override bool CanExecute(object parameter)
        {
            return true;
        }
        public override void Execute(object parameter)
        {
            if (_mvm.PressuresAndTimes?.Pressures1f.Count == 0 || _mvm.PressuresAndTimes == null)
                for (int i = 0; i < _mvm.WellViewModel.Wells.Count; i++)
                    _mvm.WellViewModel.Wells[i].Mode = Mode.Reverse;
            _mvm.ConsumptionsAndTimes = SendWellsForConsumptions();
            _mvm.PlotViewModel.PlotTimeConsumptions(_mvm.ConsumptionsAndTimes);
            CalculateInitialFminQ();
        }

        public ConsumptionsAndTimes SendWellsForConsumptions()
        {
            WellsList wellsList = new WellsList(_mvm.WellViewModel.Wells.ToList());
            ConsumptionsAndTimes consumptionsAndTimes = RealMagic.GetConsumptions(wellsList);
            _mvm.WellViewModel.Wells[0].CalculatedQ = consumptionsAndTimes.Consumptions[wellsList.Indexes[0] - 2];//5.5099120064701842E-05
            _mvm.WellViewModel.Wells[1].CalculatedQ = consumptionsAndTimes.Consumptions[wellsList.Indexes[1] - 1];//0.00011114639731946801
            _mvm.WellViewModel.Wells[2].CalculatedQ = consumptionsAndTimes.Consumptions[wellsList.Indexes[2] - 2];//0.00016799535363899219
            return consumptionsAndTimes;
        }

        public double CalculateInitialFminQ()
        {
            if (_mvm.QGradientViewModel.GradientsAndConsumptions.Count != 0)
                _mvm.QGradientViewModel.GradientsAndConsumptions.Clear();
            QGradient g = new QGradient
            {
                ChangedK = _mvm.WellViewModel.Wells[0].K,
                ChangedKappa = _mvm.WellViewModel.Wells[0].Kappa,
                ChangedKsi = _mvm.WellViewModel.Wells[0].Ksi,
                ChangedP0 = _mvm.WellViewModel.Wells[0].P0
            };
            _mvm.QGradientViewModel.GradientsAndConsumptions.Add(new QGradientAndConsumptions());
            _mvm.QGradientViewModel.GradientsAndConsumptions[0].QGradient = g;
            _mvm.QGradientViewModel.GradientsAndConsumptions[0].ConsumptionsAndTimes = _mvm.ConsumptionsAndTimes;
            double Fmin = 0;
            switch (_mvm.WellViewModel.Wells.Count)
            {
                //case 1:
                //    Fmin = Math.Pow((wellViewModel.Wells[0].Q - Qk1.Last()), 2);
                //    Fmin = Math.Sqrt(Fmin / (Math.Pow(wellViewModel.Wells[0].Q, 2)));
                //    break;
                //case 2:
                //    Fmin = Math.Pow((wellViewModel.Wells[0].Q - Qk1[indexes[0] - 2]), 2) + Math.Pow((wellViewModel.Wells[1].Q - Qk1.Last()), 2);
                //    Fmin = Math.Sqrt(Fmin / (Math.Pow(wellViewModel.Wells[0].Q, 2) + Math.Pow(wellViewModel.Wells[1].Q, 2)));
                //    break;
                case 3:
                    Fmin = Math.Pow(_mvm.WellViewModel.Wells[0].Q - _mvm.WellViewModel.Wells[0].CalculatedQ, 2)
                            + Math.Pow(_mvm.WellViewModel.Wells[1].Q - _mvm.WellViewModel.Wells[1].CalculatedQ, 2)
                            + Math.Pow(_mvm.WellViewModel.Wells[2].Q - _mvm.WellViewModel.Wells[2].CalculatedQ, 2);
                    Fmin = Fmin / (Math.Pow(_mvm.WellViewModel.Wells[0].Q, 2) + Math.Pow(_mvm.WellViewModel.Wells[1].Q, 2) + Math.Pow(_mvm.WellViewModel.Wells[2].Q, 2));
                    break;
            }
            _mvm.QGradientViewModel.GradientsAndConsumptions[0].QGradient.FminQ = Fmin;
            //GradientClc.GradientToShow = GradientsAndConsumptions.Last().Gradient;
            _mvm.QGradientViewModel.SelectedGradient = _mvm.QGradientViewModel.GradientsAndConsumptions.Last().QGradient;
            _mvm.QGradientViewModel.Gradients.Add(_mvm.QGradientViewModel.GradientsAndConsumptions[0].QGradient);
            return Fmin;
            //GradientClc.FQmin.Text = gradientViewModel.SelectedGradient.FminQ.ToString();
        }
    }

    public class ClearCommand : MainViewCommand
    {
        public ClearCommand(MainViewModel mvm) : base(mvm)
        { }

        public override bool CanExecute(object parameter)
        {
            return true;
        }

        public override void Execute(object parameter)
        {
            _mvm.PlotViewModel.MyModel.Series.Clear();
            _mvm.PlotViewModel.MyModel.InvalidatePlot(true);
            _mvm.QGradientViewModel.GradientsAndConsumptions.Clear();
            _mvm.PressuresAndTimes?.Pressures1f?.Clear();
            _mvm.PressuresAndTimes?.Pressures1s?.Clear();
            _mvm.PressuresAndTimes?.Pressures2f?.Clear();
            _mvm.PressuresAndTimes?.Pressures2s?.Clear();
            _mvm.PressuresAndTimes?.Pressures3?.Clear();
            _mvm.PressuresAndTimes?.Times1f?.Clear();
            _mvm.PressuresAndTimes?.Times1s?.Clear();
            _mvm.PressuresAndTimes?.Times2f?.Clear();
            _mvm.PressuresAndTimes?.Times2s?.Clear();
            _mvm.PressuresAndTimes?.Times3?.Clear();
            _mvm.ConsumptionsAndTimes?.Consumptions?.Clear();
            _mvm.ConsumptionsAndTimes?.StaticConsumptions?.Clear();
            _mvm.ConsumptionsAndTimes?.Times?.Clear();
        }
    }
   

}

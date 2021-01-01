using DisserNET.Views;
using DisserNET.Models;
using DisserNET.ViewModels;
using Newtonsoft.Json;
using System;
using DisserNET.Calculs;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DisserNET.Commands
{
    abstract public class PGradientViewCommand : ICommand
    {
        protected PGradientViewModel _gvm;
        public PGradientViewCommand(PGradientViewModel gvm)
        {
            _gvm = gvm;
        }
        public event EventHandler CanExecuteChanged;
        public abstract bool CanExecute(object parameter);
        public abstract void Execute(object parameter);
    }

    public class NextStepPCommand : PGradientViewCommand
    {
        public NextStepPCommand(PGradientViewModel wvm) : base(wvm)
        {
        }
        public override bool CanExecute(object parameter)
        {
            return true;
        }

        public override void Execute(object parameter)
        {
            var parameters = (object[])parameter;
            PGradient g;
            if (!_gvm.IsFirstTimeGradientClicked)
            {
                _gvm.PGradientAndPressures.Last().PGradient.Lambda = Convert.ToDouble(parameters[0]);
                _gvm.PGradientAndPressures.Last().PGradient.ChangedK = Convert.ToDouble(parameters[1]) * Math.Pow(10.0, -15);
                _gvm.PGradientAndPressures.Last().PGradient.ChangedKappa = Convert.ToDouble(parameters[2]) * (1.0 / 3600.0);
                _gvm.PGradientAndPressures.Last().PGradient.ChangedKsi = Convert.ToDouble(parameters[3]);
                _gvm.PGradientAndPressures.Last().PGradient.ChangedP0 = Convert.ToDouble(parameters[4]) * Math.Pow(10.0, 6);

                _gvm.PGradientAndPressures.Last().PGradient.DeltaK =
                    Convert.ToDouble(parameters[1]) * Math.Pow(10.0, -15) *
                    Math.Pow(10, Convert.ToDouble(parameters[5].ToString().Substring(parameters[5].ToString().IndexOf('-'),
                    parameters[5].ToString().IndexOf(')') - parameters[5].ToString().IndexOf('-'))));

                _gvm.PGradientAndPressures.Last().PGradient.DeltaKappa =
                    Convert.ToDouble(parameters[2]) * (1.0 / 3600.0) *
                    Math.Pow(10, Convert.ToDouble(parameters[6].ToString().Substring(parameters[6].ToString().IndexOf('-'),
                    parameters[6].ToString().IndexOf(')') - parameters[6].ToString().IndexOf('-'))));

                _gvm.PGradientAndPressures.Last().PGradient.DeltaKsi =
                    Convert.ToDouble(parameters[3]) *
                    Math.Pow(10, Convert.ToDouble(parameters[7].ToString().Substring(parameters[7].ToString().IndexOf('-'),
                    parameters[7].ToString().IndexOf(')') - parameters[7].ToString().IndexOf('-'))));

                _gvm.PGradientAndPressures.Last().PGradient.DeltaP0 =
                    Convert.ToDouble(parameters[4]) * Math.Pow(10.0, 6) *
                    Math.Pow(10, Convert.ToDouble(parameters[8].ToString().Substring(parameters[8].ToString().IndexOf('-'),
                    parameters[8].ToString().IndexOf(')') - parameters[8].ToString().IndexOf('-'))));


                _gvm.PGradientAndPressures.Last().PGradient.UsedK = (bool?)parameters[9];
                _gvm.PGradientAndPressures.Last().PGradient.UsedKappa = (bool?)parameters[10];
                _gvm.PGradientAndPressures.Last().PGradient.UsedKsi = (bool?)parameters[11];
                _gvm.PGradientAndPressures.Last().PGradient.UsedP0 = (bool?)parameters[12];

                g = _gvm.PGradientAndPressures.Last().PGradient;
                _gvm.IsFirstTimeGradientClicked = true;
            }
            else
            {
                g = _gvm.PGradientAndPressures.Last().PGradient;
                g.Lambda = Convert.ToDouble(parameters[0]);

                g.DeltaK =
                    MainWindow.MainViewModel.WellViewModel.Wells[0].K *
                    Math.Pow(10, Convert.ToDouble(parameters[5].ToString().Substring(parameters[5].ToString().IndexOf('-'),
                    parameters[5].ToString().IndexOf(')') - parameters[5].ToString().IndexOf('-'))));

                g.DeltaKappa =
                    MainWindow.MainViewModel.WellViewModel.Wells[0].Kappa *
                    Math.Pow(10, Convert.ToDouble(parameters[6].ToString().Substring(parameters[6].ToString().IndexOf('-'),
                    parameters[6].ToString().IndexOf(')') - parameters[6].ToString().IndexOf('-'))));

                g.DeltaKsi =
                    MainWindow.MainViewModel.WellViewModel.Wells[0].Ksi *
                    Math.Pow(10, Convert.ToDouble(parameters[7].ToString().Substring(parameters[7].ToString().IndexOf('-'),
                    parameters[7].ToString().IndexOf(')') - parameters[7].ToString().IndexOf('-'))));

                g.DeltaP0 =
                    MainWindow.MainViewModel.WellViewModel.Wells[0].P0 *
                    Math.Pow(10, Convert.ToDouble(parameters[8].ToString().Substring(parameters[8].ToString().IndexOf('-'),
                    parameters[8].ToString().IndexOf(')') - parameters[8].ToString().IndexOf('-'))));
            }

            PGradientAndPressures pGradientAndPressures = SendWellsForGradient(g);
            if (pGradientAndPressures.PressuresAndTimes != null)
            {
                _gvm.PGradientAndPressures.Add(pGradientAndPressures);
                _gvm.Gradients.Add(pGradientAndPressures.PGradient);
                _gvm.SelectedGradient = _gvm.PGradientAndPressures.Last().PGradient;
                _gvm.SelectedGradient = _gvm.PGradientAndPressures.Last().PGradient;
                MainWindow.MainViewModel.PlotViewModel.PlotTimePressures(pGradientAndPressures.PressuresAndTimes);
            }
        }

        private PGradientAndPressures SendWellsForGradient(PGradient gradient)
        {
            WellsList wellsList = new WellsList(MainWindow.MainViewModel.WellViewModel.Wells.ToList());
            GradientAndWellsList<PGradient> gradientAndWellsList = new GradientAndWellsList<PGradient>
            {
                Gradient = gradient,
                WellsList = wellsList,
            };
            PGradientAndPressures pGradientAndPressures = Functions.PGradientMethod(gradientAndWellsList);
            return pGradientAndPressures;
        }
    }


    public class PreviousStepPCommand : PGradientViewCommand
    {
        public PreviousStepPCommand(PGradientViewModel wvm) : base(wvm)
        {
        }
        public override bool CanExecute(object parameter)
        {
            return true;
        }

        public override void Execute(object parameter)
        {
            if (_gvm.PGradientAndPressures.Count > 1)
            {
                _gvm.PGradientAndPressures.RemoveAt(_gvm.PGradientAndPressures.Count - 1);
                _gvm.Gradients.RemoveAt(_gvm.PGradientAndPressures.Count - 1);
                _gvm.SelectedGradient = _gvm.PGradientAndPressures.Last().PGradient;
                _gvm.SelectedGradient = _gvm.PGradientAndPressures.Last().PGradient;
                if (_gvm.PGradientAndPressures.Count == 1)
                    _gvm.IsFirstTimeGradientClicked = false;
                MainWindow.MainViewModel.PlotViewModel.PlotTimePressures(_gvm.PGradientAndPressures.Last().PressuresAndTimes);
            }
        }
    }

    public class SavePCommand : PGradientViewCommand
    {
        public SavePCommand(PGradientViewModel wvm) : base(wvm)
        {
        }
        public override bool CanExecute(object parameter)
        {
            return true;
        }

        public override void Execute(object parameter)
        {
            string path = Directory.GetCurrentDirectory();
            string writePath = Path.Combine(path, "PressureGradient.json");
            string text = JsonConvert.SerializeObject(parameter as PGradient);
            try
            {
                using (StreamWriter sw = new StreamWriter(writePath, false, System.Text.Encoding.Default))
                {
                    sw.WriteLine(text);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }

    public class ClearGradientsPCommand : PGradientViewCommand
    {
        public ClearGradientsPCommand(PGradientViewModel wvm) : base(wvm)
        {
        }
        public override bool CanExecute(object parameter)
        {
            return true;
        }

        public override void Execute(object parameter)
        {
            if (_gvm.PGradientAndPressures.Count > 1)
            {
                _gvm.PGradientAndPressures.RemoveRange(0, _gvm.PGradientAndPressures.Count - 1);
                while (_gvm.Gradients.Count > 1)
                {
                    _gvm.Gradients.RemoveAt(_gvm.Gradients.Count - 1);
                }
                _gvm.SelectedGradient = _gvm.PGradientAndPressures.Last().PGradient;
                _gvm.SelectedGradient = _gvm.PGradientAndPressures.Last().PGradient;
                _gvm.IsFirstTimeGradientClicked = false;
                MainWindow.MainViewModel.PlotViewModel.PlotTimePressures(_gvm.PGradientAndPressures.Last().PressuresAndTimes);
            }
        }
    }
}

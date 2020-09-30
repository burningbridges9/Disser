using HydrodynamicStudies.Calculs;
using HydrodynamicStudies.Models;
using HydrodynamicStudies.ViewModels;
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

namespace HydrodynamicStudies.Commands
{
    abstract public class QGradientViewCommand : ICommand
    {
        protected QGradientViewModel _gvm;
        public QGradientViewCommand(QGradientViewModel gvm)
        {
            _gvm = gvm;
        }
        public event EventHandler CanExecuteChanged;
        public abstract bool CanExecute(object parameter);
        public abstract void Execute(object parameter);
    }

    public class NextStepCommand : QGradientViewCommand
    {
        public NextStepCommand(QGradientViewModel wvm) : base(wvm)
        {
        }
        public override bool CanExecute(object parameter)
        {
            return true;
        }

        public override void Execute(object parameter)
        {
            var parameters = (object[])parameter;
            QGradient g;
            if (!_gvm.IsFirstTimeGradientClicked)
            {
                _gvm.GradientsAndConsumptions.Last().QGradient.Lambda = Convert.ToDouble(parameters[0]);
                _gvm.GradientsAndConsumptions.Last().QGradient.ChangedK = Convert.ToDouble(parameters[1]) * Math.Pow(10.0, -15);
                _gvm.GradientsAndConsumptions.Last().QGradient.ChangedKappa = Convert.ToDouble(parameters[2]) * (1.0 / 3600.0);
                _gvm.GradientsAndConsumptions.Last().QGradient.ChangedKsi = Convert.ToDouble(parameters[3]);
                _gvm.GradientsAndConsumptions.Last().QGradient.ChangedP0 = Convert.ToDouble(parameters[4]) * Math.Pow(10.0, 6);

                _gvm.GradientsAndConsumptions.Last().QGradient.DeltaK =
                    Convert.ToDouble(parameters[1]) * Math.Pow(10.0, -15) *
                    Math.Pow(10, Convert.ToDouble(parameters[5].ToString().Substring(parameters[5].ToString().IndexOf('-'),
                    parameters[5].ToString().IndexOf(')') - parameters[5].ToString().IndexOf('-'))));

                _gvm.GradientsAndConsumptions.Last().QGradient.DeltaKappa =
                    Convert.ToDouble(parameters[2]) * (1.0 / 3600.0) *
                    Math.Pow(10, Convert.ToDouble(parameters[6].ToString().Substring(parameters[6].ToString().IndexOf('-'),
                    parameters[6].ToString().IndexOf(')') - parameters[6].ToString().IndexOf('-'))));

                _gvm.GradientsAndConsumptions.Last().QGradient.DeltaKsi =
                    Convert.ToDouble(parameters[3]) *
                    Math.Pow(10, Convert.ToDouble(parameters[7].ToString().Substring(parameters[7].ToString().IndexOf('-'),
                    parameters[7].ToString().IndexOf(')') - parameters[7].ToString().IndexOf('-'))));

                _gvm.GradientsAndConsumptions.Last().QGradient.DeltaP0 =
                    Convert.ToDouble(parameters[4]) * Math.Pow(10.0, 6) *
                    Math.Pow(10, Convert.ToDouble(parameters[8].ToString().Substring(parameters[8].ToString().IndexOf('-'),
                    parameters[8].ToString().IndexOf(')') - parameters[8].ToString().IndexOf('-'))));


                _gvm.GradientsAndConsumptions.Last().QGradient.UsedK = (bool?)parameters[9];
                _gvm.GradientsAndConsumptions.Last().QGradient.UsedKappa = (bool?)parameters[10];
                _gvm.GradientsAndConsumptions.Last().QGradient.UsedKsi = (bool?)parameters[11];
                _gvm.GradientsAndConsumptions.Last().QGradient.UsedP0 = (bool?)parameters[12];

                g = _gvm.GradientsAndConsumptions.Last().QGradient;
                _gvm.IsFirstTimeGradientClicked = true;
            }
            else
            {
                g = _gvm.GradientsAndConsumptions.Last().QGradient;
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

            QGradientAndConsumptions gradientAndConsumptions = SendWellsForGradient(g);
            if (gradientAndConsumptions.ConsumptionsAndTimes != null)
            {
                _gvm.GradientsAndConsumptions.Add(gradientAndConsumptions);
                _gvm.Gradients.Add(gradientAndConsumptions.QGradient);
                _gvm.SelectedGradient = _gvm.Gradients.Last() as QGradient;
                MainWindow.MainViewModel.PlotViewModel.PlotTimeConsumptions(gradientAndConsumptions.ConsumptionsAndTimes);
            }
        }

        private QGradientAndConsumptions SendWellsForGradient(QGradient gradient)
        {
            WellsList wellsList = new WellsList(MainWindow.MainViewModel.WellViewModel.Wells.ToList());
            GradientAndWellsList<QGradient> gradientAndWellsList = new GradientAndWellsList<QGradient>
            {
                Gradient = gradient,
                WellsList = wellsList,
            };
            QGradientAndConsumptions gradientAndConsumptions = Functions.QGradientMethod(gradientAndWellsList);
            return gradientAndConsumptions;
        }
    }


    public class PreviousStepCommand : QGradientViewCommand
    {
        public PreviousStepCommand(QGradientViewModel wvm) : base(wvm)
        {
        }
        public override bool CanExecute(object parameter)
        {
            return true;
        }

        public override void Execute(object parameter)
        {
            if (_gvm.GradientsAndConsumptions.Count > 1)
            {
                _gvm.GradientsAndConsumptions.RemoveAt(_gvm.GradientsAndConsumptions.Count - 1);
                _gvm.Gradients.RemoveAt(_gvm.GradientsAndConsumptions.Count - 1);
                _gvm.SelectedGradient = _gvm.Gradients.Last() as QGradient;
                MainWindow.MainViewModel.PlotViewModel.PlotTimeConsumptions(_gvm.GradientsAndConsumptions.Last().ConsumptionsAndTimes);
                if (_gvm.GradientsAndConsumptions.Count == 1)
                    _gvm.IsFirstTimeGradientClicked = false;
            }
        }
    }

    public class SaveQCommand : QGradientViewCommand
    {
        public SaveQCommand(QGradientViewModel wvm) : base(wvm)
        {
        }
        public override bool CanExecute(object parameter)
        {
            return true;
        }

        public override void Execute(object parameter)
        {
            string path = Directory.GetCurrentDirectory();
            string writePath = Path.Combine(path, "ConsumptionGradient.json");
            string text = JsonConvert.SerializeObject(parameter as QGradient);
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
}

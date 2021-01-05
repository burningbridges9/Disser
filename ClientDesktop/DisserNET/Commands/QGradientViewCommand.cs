using DisserNET.Calculs;
using DisserNET.Models;
using DisserNET.ViewModels;
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

namespace DisserNET.Commands
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
                _gvm.GradientsAndConsumptions.Last().Grad.Lambda = Convert.ToDouble(parameters[0]);
                _gvm.GradientsAndConsumptions.Last().Grad.ChangedK = Convert.ToDouble(parameters[1]) * Math.Pow(10.0, -15);
                _gvm.GradientsAndConsumptions.Last().Grad.ChangedKappa = Convert.ToDouble(parameters[2]) * (1.0 / 3600.0);
                _gvm.GradientsAndConsumptions.Last().Grad.ChangedKsi = Convert.ToDouble(parameters[3]);
                _gvm.GradientsAndConsumptions.Last().Grad.ChangedP0 = Convert.ToDouble(parameters[4]) * Math.Pow(10.0, 6);

                _gvm.GradientsAndConsumptions.Last().Grad.DeltaK =
                    Convert.ToDouble(parameters[1]) * Math.Pow(10.0, -15) *
                    Math.Pow(10, Convert.ToDouble(parameters[5].ToString().Substring(parameters[5].ToString().IndexOf('-'),
                    parameters[5].ToString().IndexOf(')') - parameters[5].ToString().IndexOf('-'))));

                _gvm.GradientsAndConsumptions.Last().Grad.DeltaKappa =
                    Convert.ToDouble(parameters[2]) * (1.0 / 3600.0) *
                    Math.Pow(10, Convert.ToDouble(parameters[6].ToString().Substring(parameters[6].ToString().IndexOf('-'),
                    parameters[6].ToString().IndexOf(')') - parameters[6].ToString().IndexOf('-'))));

                _gvm.GradientsAndConsumptions.Last().Grad.DeltaKsi =
                    Convert.ToDouble(parameters[3]) *
                    Math.Pow(10, Convert.ToDouble(parameters[7].ToString().Substring(parameters[7].ToString().IndexOf('-'),
                    parameters[7].ToString().IndexOf(')') - parameters[7].ToString().IndexOf('-'))));

                _gvm.GradientsAndConsumptions.Last().Grad.DeltaP0 =
                    Convert.ToDouble(parameters[4]) * Math.Pow(10.0, 6) *
                    Math.Pow(10, Convert.ToDouble(parameters[8].ToString().Substring(parameters[8].ToString().IndexOf('-'),
                    parameters[8].ToString().IndexOf(')') - parameters[8].ToString().IndexOf('-'))));


                _gvm.GradientsAndConsumptions.Last().Grad.UsedK = (bool?)parameters[9];
                _gvm.GradientsAndConsumptions.Last().Grad.UsedKappa = (bool?)parameters[10];
                _gvm.GradientsAndConsumptions.Last().Grad.UsedKsi = (bool?)parameters[11];
                _gvm.GradientsAndConsumptions.Last().Grad.UsedP0 = (bool?)parameters[12];

                g = _gvm.GradientsAndConsumptions.Last().Grad;
                _gvm.IsFirstTimeGradientClicked = true;
            }
            else
            {
                g = _gvm.GradientsAndConsumptions.Last().Grad;
                g.Lambda = Convert.ToDouble(parameters[0]);

                g.DeltaK =
                    _gvm.GradientsAndConsumptions.First().Grad.ChangedK *
                    Math.Pow(10, Convert.ToDouble(parameters[5].ToString().Substring(parameters[5].ToString().IndexOf('-'),
                    parameters[5].ToString().IndexOf(')') - parameters[5].ToString().IndexOf('-'))));

                g.DeltaKappa =
                    _gvm.GradientsAndConsumptions.First().Grad.ChangedKappa *
                    Math.Pow(10, Convert.ToDouble(parameters[6].ToString().Substring(parameters[6].ToString().IndexOf('-'),
                    parameters[6].ToString().IndexOf(')') - parameters[6].ToString().IndexOf('-'))));

                g.DeltaKsi =
                    _gvm.GradientsAndConsumptions.First().Grad.ChangedKsi *
                    Math.Pow(10, Convert.ToDouble(parameters[7].ToString().Substring(parameters[7].ToString().IndexOf('-'),
                    parameters[7].ToString().IndexOf(')') - parameters[7].ToString().IndexOf('-'))));

                g.DeltaP0 =
                    _gvm.GradientsAndConsumptions.First().Grad.ChangedP0 *
                    Math.Pow(10, Convert.ToDouble(parameters[8].ToString().Substring(parameters[8].ToString().IndexOf('-'),
                    parameters[8].ToString().IndexOf(')') - parameters[8].ToString().IndexOf('-'))));
            }

            QGradientAndConsumptions gradientAndConsumptions = SendWellsForGradient(g);
            if (gradientAndConsumptions.ValuesAndTimes != null)
            {
                _gvm.GradientsAndConsumptions.Add(gradientAndConsumptions);
                _gvm.Gradients.Add(gradientAndConsumptions.Grad);
                _gvm.SelectedGradient = _gvm.Gradients.Last() as QGradient;
               // MainWindow.MainViewModell.PlotViewModel.PlotTimeConsumptions(gradientAndConsumptions.ConsumptionsAndTimes);
            }
        }

        private QGradientAndConsumptions SendWellsForGradient(QGradient gradient)
        {
            WellsList wellsList = _gvm.wellsList;//new WellsList(MainWindow.MainViewModell.WellViewModel.Wells.ToList());
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
               // MainWindow.MainViewModell.PlotViewModel.PlotTimeConsumptions(_gvm.GradientsAndConsumptions.Last().ValuesAndTimes);
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

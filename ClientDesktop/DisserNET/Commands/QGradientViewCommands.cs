using DisserNET.Calculs;
using DisserNET.Models;
using DisserNET.ViewModels;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace DisserNET.Commands
{
    public class GradientCalculateHelper
    {
        string GetSubstring(object param) => param.ToString().Substring(GetStartIndexOfParam(param), GetLength(param));
        int GetStartIndexOfParam(object param) => param.ToString().IndexOf('-');
        int GetLength(object param) => param.ToString().IndexOf(')') - param.ToString().IndexOf('-');
        protected void FillDeltas<TGrad>(object[] parameters, TGrad lastGrad) where TGrad : Gradient
        {
            lastGrad.DeltaK = Convert.ToDouble(parameters[1]) * Math.Pow(10.0, -15) * Math.Pow(10, Convert.ToDouble(GetSubstring(parameters[5])));
            lastGrad.DeltaKappa = Convert.ToDouble(parameters[2]) * (1.0 / 3600.0) * Math.Pow(10, Convert.ToDouble(GetSubstring(parameters[6])));
            lastGrad.DeltaKsi = Convert.ToDouble(parameters[3]) * Math.Pow(10, Convert.ToDouble(GetSubstring(parameters[7])));
            lastGrad.DeltaP0 = Convert.ToDouble(parameters[4]) * Math.Pow(10.0, 6) * Math.Pow(10, Convert.ToDouble(GetSubstring(parameters[8])));
        }
    }
    abstract public class QGradientViewCommand : GradientCalculateHelper, ICommand
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
            QGradient g = new QGradient();
            if (!_gvm.IsFirstTimeGradientClicked)
            {
                var lastGrad = _gvm.GradientsAndConsumptions?.LastOrDefault()?.Grad;
                if (lastGrad is not null)
                {
                    lastGrad.Lambda = Convert.ToDouble(parameters[0]);
                    lastGrad.ChangedK = Convert.ToDouble(parameters[1]) * Math.Pow(10.0, -15);
                    lastGrad.ChangedKappa = Convert.ToDouble(parameters[2]) * (1.0 / 3600.0);
                    lastGrad.ChangedKsi = Convert.ToDouble(parameters[3]);
                    lastGrad.ChangedP0 = Convert.ToDouble(parameters[4]) * Math.Pow(10.0, 6);
                    FillDeltas(parameters, lastGrad);

                    lastGrad.UsedK = (bool?)parameters[9];
                    lastGrad.UsedKappa = (bool?)parameters[10];
                    lastGrad.UsedKsi = (bool?)parameters[11];
                    lastGrad.UsedP0 = (bool?)parameters[12];

                    g = lastGrad;
                    _gvm.IsFirstTimeGradientClicked = true;
                }
            }
            else
            {
                g = _gvm.GradientsAndConsumptions.Last().Grad;
                g.Lambda = Convert.ToDouble(parameters[0]);
                FillDeltas(parameters, g);
            }

            QGradientAndConsumptions gradientAndConsumptions = SendWellsForGradient(g);
            if (gradientAndConsumptions.ValuesAndTimes != null)
            {
                _gvm.GradientsAndConsumptions.Add(gradientAndConsumptions);
                _gvm.Gradients.Add(gradientAndConsumptions.Grad);
                _gvm.SelectedGradient = _gvm.Gradients.Last();
            }
        }

        private QGradientAndConsumptions SendWellsForGradient(QGradient gradient)
        {
            WellsList wellsList = _gvm.wellsList;
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
                _gvm.GradientsAndConsumptions.Remove(_gvm.GradientsAndConsumptions.First());
                _gvm.Gradients.Remove(_gvm.GradientsAndConsumptions.First().Grad);
                _gvm.SelectedGradient = _gvm.Gradients.Last();
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

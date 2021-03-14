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
    abstract public class PGradientViewCommand : GradientCalculateHelper, ICommand
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
            PGradient g = new PGradient();
            if (!_gvm.IsFirstTimeGradientClicked)
            {
                var lastGrad = _gvm.PGradientAndPressures?.LastOrDefault()?.Grad;
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
                g = _gvm.PGradientAndPressures.Last().Grad;
                g.Lambda = Convert.ToDouble(parameters[0]);
                FillDeltas(parameters, g);
            }

            PGradientAndPressures pGradientAndPressures = SendWellsForGradient(g);
            if (pGradientAndPressures.ValuesAndTimes != null)
            {
                _gvm.PGradientAndPressures.Add(pGradientAndPressures);
                _gvm.Gradients.Add(pGradientAndPressures.Grad);
                _gvm.SelectedGradient = _gvm.PGradientAndPressures.Last().Grad;
            }
        }

        private PGradientAndPressures SendWellsForGradient(PGradient gradient)
        {
            WellsList wellsList = _gvm.wellsList;
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
                var grd = _gvm.PGradientAndPressures.Last().Grad;
                _gvm.PGradientAndPressures.Remove(_gvm.PGradientAndPressures.Last());
                _gvm.Gradients.Remove(grd);
                _gvm.SelectedGradient = _gvm.Gradients.Last();
                if (_gvm.PGradientAndPressures.Count == 1)
                    _gvm.IsFirstTimeGradientClicked = false;
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
}

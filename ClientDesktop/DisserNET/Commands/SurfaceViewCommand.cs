using DisserNET.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Surface;

namespace DisserNET.Commands
{
    abstract public class SurfaceViewCommand : ICommand
    {
        protected SurfaceViewModel _svm;
        public SurfaceViewCommand(SurfaceViewModel svm)
        {
            _svm = svm;
        }
        public event EventHandler CanExecuteChanged;
        public abstract bool CanExecute(object parameter);
        public abstract void Execute(object parameter);
    }
    public class FminQ_SurfaceCommand : SurfaceViewCommand
    {
        public FminQ_SurfaceCommand(SurfaceViewModel svm) : base(svm)
        { }

        public override bool CanExecute(object parameter)
        {
            return true;
        }

        public override void Execute(object parameter)
        {
            var reportFile = Path.Combine(Environment.CurrentDirectory, _svm.SurfaceModel.Report_file_name_Q+".xls");
            if (File.Exists(reportFile))
                File.Delete(reportFile);
            var parameters = (object[])parameter; 
            List<double> pZeros = new List<double>();
            List<double> kappas = new List<double>();
            List<double> ks = new List<double>();
            List<double> kappasC = new List<double>();
            List<double> ksC = new List<double>();
            string writePath1 = _svm.SurfaceModel.K_path_Q;
            string writePath2 = _svm.SurfaceModel.Kappa_path_Q;
            string writePath3 = _svm.SurfaceModel.P0_path_Q;
            string writePath5 = _svm.SurfaceModel.Fmin_path_Q;

            double kLeft = double.Parse(parameters[0].ToString());
            double kRight = double.Parse(parameters[1].ToString());
            double kappaLeft = double.Parse(parameters[2].ToString());
            double kappaRight = double.Parse(parameters[3].ToString());
            double p0Left = double.Parse(parameters[4].ToString());
            double p0Rigth = double.Parse(parameters[5].ToString());

            double n = int.Parse(parameters[6].ToString());// 15;
            double kStep = (kRight - kLeft) / n;
            double kappaStep = (kappaRight - kappaLeft) / n;
            double p0Step = (p0Rigth - p0Left) / n;

            #region Fill arrays
            using (StreamWriter sw1 = new StreamWriter(writePath1, false, Encoding.Default))
            using (StreamWriter sw2 = new StreamWriter(writePath2, false, Encoding.Default))
            using (StreamWriter sw3 = new StreamWriter(writePath3, false, Encoding.Default))
            {
                for (int k = 0; k <= n; k++)
                {
                    kappas.Add(kappaLeft + k * kappaStep);
                    kappasC.Add((1.0 / 3600.0) * kappas[k]);
                    ks.Add(kLeft + k * kStep);
                    ksC.Add(Math.Pow(10.0, -15) * ks[k]);
                    pZeros.Add(p0Left + k * p0Step);

                    sw1.Write(ks[k] + " ");
                    sw2.Write(kappas[k] + " ");
                    sw3.Write(pZeros[k] + " ");
                }
            }
            #endregion
            #region Fmin Kappa P0
            using (StreamWriter sw = new StreamWriter(writePath5, false, Encoding.Default))
            {
                for (int i = 0; i <= n; i++)
                {
                    for (int j = 0; j <= n; j++)
                    {
                        for (int v = 0; v <= n; v++)
                        {
                            object P0 = pZeros[i].ToString();
                            object K = ks[v].ToString();
                            object Kappa = kappas[j].ToString();
                            for (int l = 0; l < MainWindow.MainViewModell.WellViewModel.Wells.Count; l++)
                            {
                                MainWindow.MainViewModell.WellViewModel.Wells[l].K = Math.Pow(10.0, -15) * double.Parse(K.ToString());
                                MainWindow.MainViewModell.WellViewModel.Wells[l].Kappa = (1.0 / 3600.0) * double.Parse(Kappa.ToString());
                                MainWindow.MainViewModell.WellViewModel.Wells[l].P0 = Math.Pow(10.0, 6) * double.Parse(P0.ToString());
                            }

                            #region Q calculation
                            MainWindow.MainViewModell.ConsumptionsAndTimes = (MainWindow.MainViewModell.CalculateConsumptions as CalculateConsumptionsCommand).SendWellsForConsumptions();
                            MainWindow.MainViewModell.plotViewModel.PlotTimeConsumptions(MainWindow.MainViewModell.ConsumptionsAndTimes);
                            double min = (MainWindow.MainViewModell.CalculateConsumptions as CalculateConsumptionsCommand).CalculateInitialFminQ();
                            #endregion

                            sw.Write(min);
                            sw.Write(" ");
                        }
                        sw.Write('\n');
                    }
                }
            }
            #endregion
            MainWindow.MainViewModell.WellViewModel.Wells.Clear();
            MainWindow.MainViewModell.QGradientViewModel.Gradients.Clear();
            MainWindow.MainViewModell.QGradientViewModel.GradientsAndConsumptions.Clear();
            MainWindow.MainViewModell.PGradientViewModel.Gradients.Clear();
            MainWindow.MainViewModell.PGradientViewModel.PGradientAndPressures.Clear();
            var surf = new Surface.SurfaceAnimator();
            surf.SurfAnimationFq(_svm.SurfaceModel.Report_file_name_Q);
        }
    }
    public class FminP_SurfaceCommand : SurfaceViewCommand
    {
        public FminP_SurfaceCommand(SurfaceViewModel svm) : base(svm)
        { }

        public override bool CanExecute(object parameter)
        {
            return true;
        }

        public override void Execute(object parameter)
        {
            var parameters = (object[])parameter;
            List<double> pZeros = new List<double>();
            List<double> kappas = new List<double>();
            List<double> ks = new List<double>();
            List<double> kappasC = new List<double>();
            List<double> ksC = new List<double>();
            string writePath1 = _svm.SurfaceModel.K_path_P;// @"C:\Users\Rustam\Documents\Visual Studio 2017\Projects\Disser\DisserNET\DisserNET\Surface\K.txt";
            string writePath2 = _svm.SurfaceModel.Kappa_path_P;// @"C:\Users\Rustam\Documents\Visual Studio 2017\Projects\Disser\DisserNET\DisserNET\Surface\Kappa.txt";
            string writePath3 = _svm.SurfaceModel.P0_path_P;// @"C:\Users\Rustam\Documents\Visual Studio 2017\Projects\Disser\DisserNET\DisserNET\Surface\P0.txt";
            string writePath4 = _svm.SurfaceModel.Fmin_path_P;// @"C:\Users\Rustam\Documents\Visual Studio 2017\Projects\Disser\DisserNET\DisserNET\Surface\FminQKappaP0.txt";

            double kLeft = double.Parse(parameters[0].ToString());// 8.9;
            double kRight = double.Parse(parameters[1].ToString());// 9.4;
            double kappaLeft = double.Parse(parameters[2].ToString());// 1.75;
            double kappaRight = double.Parse(parameters[3].ToString());// 2.75;
            double p0Left = double.Parse(parameters[4].ToString());// 19.88;
            double p0Rigth = double.Parse(parameters[5].ToString());// 19.9;

            double n = int.Parse(parameters[6].ToString());// 15;
            double kStep = (kRight - kLeft) / n;
            double kappaStep = (kappaRight - kappaLeft) / n;
            double p0Step = (p0Rigth - p0Left) / n;
            #region Fill arrays
            using (StreamWriter sw1 = new StreamWriter(writePath1, false, Encoding.Default))
            using (StreamWriter sw2 = new StreamWriter(writePath2, false, Encoding.Default))
            using (StreamWriter sw3 = new StreamWriter(writePath3, false, Encoding.Default))
            {
                for (int k = 0; k <= n; k++)
                {
                    kappas.Add(kappaLeft + k * kappaStep);
                    kappasC.Add((1.0 / 3600.0) * kappas[k]);
                    ks.Add(kLeft + k * kStep);
                    ksC.Add(Math.Pow(10.0, -15) * ks[k]);
                    pZeros.Add(p0Left + k * p0Step);

                    sw1.Write(ks[k] + " ");
                    sw2.Write(kappas[k] + " ");
                    sw3.Write(pZeros[k] + " ");
                }
            }
            #endregion
            bool f = true;
            #region FminP Kappa P0
            using (StreamWriter sw = new StreamWriter(writePath4, false, Encoding.Default))
            {
                for (int i = 0; i <= n; i++)
                {
                    for (int j = 0; j <= n; j++)
                    {
                        for (int v = 0; v <= n; v++)
                        {
                            object P0 = pZeros[i].ToString();
                            object K = ks[v].ToString();
                            object Kappa = kappas[j].ToString();
                            for (int l = 0; l < MainWindow.MainViewModell.WellViewModel.Wells.Count; l++)
                            {
                                MainWindow.MainViewModell.WellViewModel.Wells[l].K = Math.Pow(10.0, -15) * double.Parse(K.ToString());
                                MainWindow.MainViewModell.WellViewModel.Wells[l].Kappa = (1.0 / 3600.0) * double.Parse(Kappa.ToString());
                                MainWindow.MainViewModell.WellViewModel.Wells[l].P0 = Math.Pow(10.0, 6) * double.Parse(P0.ToString());
                            }
                            #region P calculation                                                                                                       
                            MainWindow.MainViewModell.PressuresAndTimes = (MainWindow.MainViewModell.CalculatePressures as CalculatePressuresCommand).SendWellsForPressures();
                            //_mvm.plotViewModel.PlotTimeConsumptions(MainViewModel.ConsumptionsAndTimes);                                                   
                            double min = (MainWindow.MainViewModell.CalculatePressures as CalculatePressuresCommand).CalculateInitialFminP();
                            #endregion

                            sw.Write(min);
                            sw.Write(" ");
                        }
                        sw.Write('\n');
                    }
                }
            }
            #endregion
            MainWindow.MainViewModell.WellViewModel.Wells.Clear();
        }
    }
}

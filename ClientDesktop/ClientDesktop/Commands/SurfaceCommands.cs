using ClientDesktop.Models;
using ClientDesktop.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientDesktop.Commands
{
    #region Not used

    public class SurfaceP0KFminQCommand : MainViewCommand
    {
        public SurfaceP0KFminQCommand(MainViewModel mvm) : base(mvm)
        { }

        public override bool CanExecute(object parameter)
        {
            return true;
        }

        public override void Execute(object parameter)
        {
            List<double> pZeros = new List<double>();
            List<double> kappas = new List<double>();
            List<double> ks = new List<double>();
            List<double> kappasC = new List<double>();
            List<double> ksC = new List<double>();
            string writePath1 = @"C:\Users\Rustam\Documents\Visual Studio 2017\Projects\Disser\ClientDesktop\ClientDesktop\Surface\K.txt";
            string writePath2 = @"C:\Users\Rustam\Documents\Visual Studio 2017\Projects\Disser\ClientDesktop\ClientDesktop\Surface\Kappa.txt";
            string writePath3 = @"C:\Users\Rustam\Documents\Visual Studio 2017\Projects\Disser\ClientDesktop\ClientDesktop\Surface\P0.txt";
            string writePath4 = @"C:\Users\Rustam\Documents\Visual Studio 2017\Projects\Disser\ClientDesktop\ClientDesktop\Surface\FminQKP0.txt";

            double kLeft = 9.4;
            double kRight = 10.80;
            double kappaLeft = 2.25;
            double kappaRight = 7;
            double p0Left = 19.88;
            double p0Rigth = 20.0;

            double n = 15;
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
            #region Fmin K P0
            using (StreamWriter sw = new StreamWriter(writePath4, false, Encoding.Default))
            {
                for (int i = 0; i <= n; i++)
                {
                    for (int j = 0; j <= n; j++)
                    {
                        for (int m = 1; m <= 3; m++)
                        {
                            #region Fill Obj
                            object Q = (5 * m).ToString();
                            object P = (5 * m).ToString();
                            object P0 = (19.88).ToString();
                            object T1 = (5 * (m - 1)).ToString();
                            object T2 = (5 * m).ToString();
                            object H0 = (1).ToString();
                            object Mu = (1).ToString();
                            object Rw = (0.1).ToString();
                            object K = ks[j].ToString();
                            object Kappa = kappas[i].ToString();
                            object Rs = (0.3).ToString();
                            object Ksi = (0).ToString();
                            object N = (100).ToString();
                            List<object> obj = new List<object>()
                            {
                                Q, P,P0, T1,T2, H0, Mu, Rw, K, Kappa, Rs, Ksi, N
                            };
                            #endregion
                            if (_mvm.WellViewModel.Wells.Count < 3)
                            {
                                _mvm.WellViewModel.Add.Execute(obj.ToArray<object>());
                            }
                            else
                            {
                                for (int l = 0; l < _mvm.WellViewModel.Wells.Count; l++)
                                {
                                    _mvm.WellViewModel.Wells[l].K = Math.Pow(10.0, -15) * double.Parse(K.ToString());
                                    _mvm.WellViewModel.Wells[l].Kappa = (1.0 / 3600.0) * double.Parse(Kappa.ToString());
                                    _mvm.WellViewModel.Wells[l].P0 = Math.Pow(10.0, 6) * double.Parse(P0.ToString());
                                }
                            }
                        }

                        #region Q calculation                                                                                                       
                        _mvm.ConsumptionsAndTimes = (_mvm.CalculateConsumptions as CalculateConsumptionsCommand).SendWellsForConsumptions();
                        //plotViewModel.PlotTimeConsumptions(MainViewModel.ConsumptionsAndTimes);                                                   
                        double min = (_mvm.CalculateConsumptions as CalculateConsumptionsCommand).CalculateInitialFminQ();
                        #endregion

                        sw.Write(min);
                        sw.Write(" ");
                        _mvm.ConsumptionsAndTimes = null;
                    }
                    sw.Write('\n');
                }
            }
            #endregion
            _mvm.WellViewModel.Wells.Clear();
        }
    }

    #endregion
    #region Not used

    public class SurfaceP0KFminPCommand : MainViewCommand
    {
        public SurfaceP0KFminPCommand(MainViewModel mvm) : base(mvm)
        { }

        public override bool CanExecute(object parameter)
        {
            return true;
        }

        public override void Execute(object parameter)
        {
            List<double> pZeros = new List<double>();
            List<double> kappas = new List<double>();
            List<double> ks = new List<double>();
            List<double> kappasC = new List<double>();
            List<double> ksC = new List<double>();
            string writePath1 = @"C:\Users\Rustam\Documents\Visual Studio 2017\Projects\Disser\ClientDesktop\ClientDesktop\Surface\K.txt";
            string writePath2 = @"C:\Users\Rustam\Documents\Visual Studio 2017\Projects\Disser\ClientDesktop\ClientDesktop\Surface\Kappa.txt";
            string writePath3 = @"C:\Users\Rustam\Documents\Visual Studio 2017\Projects\Disser\ClientDesktop\ClientDesktop\Surface\P0.txt";
            string writePath6 = @"C:\Users\Rustam\Documents\Visual Studio 2017\Projects\Disser\ClientDesktop\ClientDesktop\Surface\FPminKP0.txt";

            double kLeft = 8.4;
            double kRight = 10.4;
            double kappaLeft = 1.25;
            double kappaRight = 3.25;
            double p0Left = 18.88;
            double p0Rigth = 20.88;

            double n = 15;
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
            #region FminP K P0
            using (StreamWriter sw = new StreamWriter(writePath6, false, Encoding.Default))
            {
                for (int i = 0; i <= n; i++)
                {
                    for (int j = 0; j <= n; j++)
                    {
                        for (int m = 1; m <= 3; m++)
                        {
                            #region Fill Obj
                            object Q = (5 * m).ToString();
                            object P = (5 * m).ToString();
                            object P0 = (19.88).ToString();
                            object T1 = (5 * (m - 1)).ToString();
                            object T2 = (5 * m).ToString();
                            object H0 = (1).ToString();
                            object Mu = (1).ToString();
                            object Rw = (0.1).ToString();
                            object K = ks[j].ToString();
                            object Kappa = kappas[i].ToString();
                            object Rs = (0.3).ToString();
                            object Ksi = (0).ToString();
                            object N = (100).ToString();
                            List<object> obj = new List<object>()
                            {
                                Q, P,P0, T1,T2, H0, Mu, Rw, K, Kappa, Rs, Ksi, N
                            };
                            #endregion
                            if (_mvm.WellViewModel.Wells.Count < 3)
                            {
                                _mvm.WellViewModel.Add.Execute(obj.ToArray<object>());
                            }
                            else
                            {
                                for (int l = 0; l < _mvm.WellViewModel.Wells.Count; l++)
                                {
                                    _mvm.WellViewModel.Wells[l].K = Math.Pow(10.0, -15) * double.Parse(K.ToString());
                                    _mvm.WellViewModel.Wells[l].P0 = Math.Pow(10.0, 6) * double.Parse(P0.ToString());
                                }
                            }
                        }

                        #region Q calculation
                        //if (_mvm.PressuresAndTimes?.Pressures1f.Count == 0 || _mvm.PressuresAndTimes == null)
                        //    for (int k = 0; k < _mvm.WellViewModel.Wells.Count; k++)
                        //        _mvm.WellViewModel.Wells[k].Mode = Mode.Reverse;
                        //if (f)
                        //{
                        //    _mvm.ConsumptionsAndTimes = (_mvm.CalculateConsumptions as CalculateConsumptionsCommand).SendWellsForConsumptions();
                        //    f = false;
                        //}
                        #endregion
                        #region Q calculation                                                                                                       
                        _mvm.PressuresAndTimes = (_mvm.CalculatePressures as CalculatePressuresCommand).SendWellsForPressures();
                        //plotViewModel.PlotTimeConsumptions(MainViewModel.ConsumptionsAndTimes);                                                   
                        double min = (_mvm.CalculatePressures as CalculatePressuresCommand).CalculateInitialFminP();
                        #endregion

                        sw.Write(min);
                        sw.Write(" ");

                        _mvm.ConsumptionsAndTimes = null;
                        //_mvm.Clear.Execute(null);
                        //_mvm.WellViewModel.DeleteAllWellCommand.Execute(null);
                    }
                    sw.Write('\n');
                }
            }
            #endregion
            _mvm.WellViewModel.Wells.Clear();
        }
    }

    #endregion

    public class SurfaceP0KappaFminQCommand : MainViewCommand
    {
        public SurfaceP0KappaFminQCommand(MainViewModel mvm) : base(mvm)
        { }

        public override bool CanExecute(object parameter)
        {
            return true;
        }

        public override void Execute(object parameter)
        {
            List<double> pZeros = new List<double>();
            List<double> kappas = new List<double>();
            List<double> ks = new List<double>();
            List<double> kappasC = new List<double>();
            List<double> ksC = new List<double>();
            string writePath1 = @"C:\Users\Rustam\Documents\Visual Studio 2017\Projects\Disser\ClientDesktop\ClientDesktop\Surface\K.txt";
            string writePath2 = @"C:\Users\Rustam\Documents\Visual Studio 2017\Projects\Disser\ClientDesktop\ClientDesktop\Surface\Kappa.txt";
            string writePath3 = @"C:\Users\Rustam\Documents\Visual Studio 2017\Projects\Disser\ClientDesktop\ClientDesktop\Surface\P0.txt";
            string writePath5 = @"C:\Users\Rustam\Documents\Visual Studio 2017\Projects\Disser\ClientDesktop\ClientDesktop\Surface\FminQKappaP0.txt";

            double kLeft = 8.9;
            double kRight = 9.4;
            double kappaLeft = 1.75;
            double kappaRight = 2.75;
            double p0Left = 19.88;
            double p0Rigth = 19.9;

            double n = 15;
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
                            for (int l = 0; l < _mvm.WellViewModel.Wells.Count; l++)
                            {
                                _mvm.WellViewModel.Wells[l].K = Math.Pow(10.0, -15) * double.Parse(K.ToString());
                                _mvm.WellViewModel.Wells[l].Kappa = (1.0 / 3600.0) * double.Parse(Kappa.ToString());
                                _mvm.WellViewModel.Wells[l].P0 = Math.Pow(10.0, 6) * double.Parse(P0.ToString());
                            }

                            #region Q calculation
                            _mvm.ConsumptionsAndTimes = (_mvm.CalculateConsumptions as CalculateConsumptionsCommand).SendWellsForConsumptions();
                            _mvm.plotViewModel.PlotTimeConsumptions(_mvm.ConsumptionsAndTimes);
                            double min = (_mvm.CalculateConsumptions as CalculateConsumptionsCommand).CalculateInitialFminQ();
                            #endregion

                            sw.Write(min);
                            sw.Write(" ");
                        }
                        sw.Write('\n');
                    }
                }
            }
            #endregion
            _mvm.WellViewModel.Wells.Clear();
        }
    }

    public class SurfaceP0KappaFminPCommand : MainViewCommand
    {
        public SurfaceP0KappaFminPCommand(MainViewModel mvm) : base(mvm)
        { }

        public override bool CanExecute(object parameter)
        {
            return true;
        }

        public override void Execute(object parameter)
        {
            List<double> pZeros = new List<double>();
            List<double> kappas = new List<double>();
            List<double> ks = new List<double>();
            List<double> kappasC = new List<double>();
            List<double> ksC = new List<double>();
            string writePath1 = @"C:\Users\Rustam\Documents\Visual Studio 2017\Projects\Disser\ClientDesktop\ClientDesktop\Surface\Kp.txt";
            string writePath2 = @"C:\Users\Rustam\Documents\Visual Studio 2017\Projects\Disser\ClientDesktop\ClientDesktop\Surface\Kappap.txt";
            string writePath3 = @"C:\Users\Rustam\Documents\Visual Studio 2017\Projects\Disser\ClientDesktop\ClientDesktop\Surface\P0p.txt";
            string writePath7 = @"C:\Users\Rustam\Documents\Visual Studio 2017\Projects\Disser\ClientDesktop\ClientDesktop\Surface\FPminKappaP0.txt";

            int kLeft = 9;
            int kRight = 11;
            int kappaLeft = 3;
            int kappaRight = 7;
            int p0Left = 19;
            int p0Rigth = 21;

            double n = 15;
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
            using (StreamWriter sw = new StreamWriter(writePath7, false, Encoding.Default))
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
                            for (int l = 0; l < _mvm.WellViewModel.Wells.Count; l++)
                            {
                                _mvm.WellViewModel.Wells[l].K = Math.Pow(10.0, -15) * double.Parse(K.ToString());
                                _mvm.WellViewModel.Wells[l].Kappa = (1.0 / 3600.0) * double.Parse(Kappa.ToString());
                                _mvm.WellViewModel.Wells[l].P0 = Math.Pow(10.0, 6) * double.Parse(P0.ToString());
                            }
                            #region P calculation                                                                                                       
                            _mvm.PressuresAndTimes = (_mvm.CalculatePressures as CalculatePressuresCommand).SendWellsForPressures();
                            //_mvm.plotViewModel.PlotTimeConsumptions(MainViewModel.ConsumptionsAndTimes);                                                   
                            double min = (_mvm.CalculatePressures as CalculatePressuresCommand).CalculateInitialFminP();
                            #endregion

                            sw.Write(min);
                            sw.Write(" ");
                        }
                        sw.Write('\n');
                    }
                }
            }
            #endregion
            _mvm.WellViewModel.Wells.Clear();
        }
    }


}

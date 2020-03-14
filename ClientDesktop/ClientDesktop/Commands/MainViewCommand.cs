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

        public override async void Execute(object parameter)
        {
            if (_mvm.ConsumptionsAndTimes == null || _mvm.ConsumptionsAndTimes?.Consumptions.Count == 0)
                for (int i = 0; i < _mvm.WellViewModel.Wells.Count; i++)
                    _mvm.WellViewModel.Wells[i].Mode = Mode.Direct;
            _mvm.PressuresAndTimes = await SendWellsForPressures();
            _mvm.PlotViewModel.PlotTimePressures(_mvm.PressuresAndTimes);
            CalculateInitialFminP();
        }

        public async Task<PressuresAndTimes> SendWellsForPressures()
        {
            WellsList wellsList = new WellsList(_mvm.WellViewModel.Wells.ToList());
            var serializedProduct = JsonConvert.SerializeObject(wellsList);
            HttpClient httpClient = new HttpClient();
            httpClient.Timeout = TimeSpan.FromMinutes(10);
            string apiUrl = "https://localhost:44308/api/values/pressures";
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));//ACCEPT header
            var content = new StringContent(serializedProduct, Encoding.UTF8, "application/json");
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, apiUrl);
            request.Content = new StringContent(serializedProduct, Encoding.UTF8, "application/json");//CONTENT-TYPE header
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var res = await httpClient.PostAsync(apiUrl, content);
            string responseBody = await res.Content.ReadAsStringAsync();
            PressuresAndTimes pressuresAndTimes = JsonConvert.DeserializeObject<PressuresAndTimes>(responseBody);
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
                    Fmin = Math.Sqrt(Fmin / (Math.Pow(_mvm.WellViewModel.Wells[0].P, 2) + Math.Pow(_mvm.WellViewModel.Wells[1].P, 2) + Math.Pow(_mvm.WellViewModel.Wells[2].P, 2)));
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
        public override async void Execute(object parameter)
        {
            if (_mvm.PressuresAndTimes?.Pressures1f.Count == 0 || _mvm.PressuresAndTimes == null)
                for (int i = 0; i < _mvm.WellViewModel.Wells.Count; i++)
                    _mvm.WellViewModel.Wells[i].Mode = Mode.Reverse;
            _mvm.ConsumptionsAndTimes = await SendWellsForConsumptions();
            _mvm.PlotViewModel.PlotTimeConsumptions(_mvm.ConsumptionsAndTimes);
            CalculateInitialFminQ();
        }

        public async Task<ConsumptionsAndTimes> SendWellsForConsumptions()
        {
            WellsList wellsList = new WellsList(_mvm.WellViewModel.Wells.ToList());
            var serializedProduct = JsonConvert.SerializeObject(wellsList);
            HttpClient httpClient = new HttpClient();
            httpClient.Timeout = TimeSpan.FromMinutes(10);
            string apiUrl = "https://localhost:44308/api/values/consumptions";
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));//ACCEPT header
            var content = new StringContent(serializedProduct, Encoding.UTF8, "application/json");
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, apiUrl);
            request.Content = new StringContent(serializedProduct, Encoding.UTF8, "application/json");//CONTENT-TYPE header
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var res = await httpClient.PostAsync(apiUrl, content);
            string responseBody = await res.Content.ReadAsStringAsync();
            ConsumptionsAndTimes consumptionsAndTimes = JsonConvert.DeserializeObject<ConsumptionsAndTimes>(responseBody);
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
                    Fmin = Math.Pow((_mvm.WellViewModel.Wells[0].Q - _mvm.ConsumptionsAndTimes.Consumptions[_mvm.WellViewModel.Wells[0].N - 2]), 2)
                            + Math.Pow((_mvm.WellViewModel.Wells[1].Q - _mvm.ConsumptionsAndTimes.Consumptions[_mvm.WellViewModel.Wells[0].N + _mvm.WellViewModel.Wells[1].N - 2]), 2)
                            + Math.Pow((_mvm.WellViewModel.Wells[2].Q - _mvm.ConsumptionsAndTimes.Consumptions.Last()), 2);
                    Fmin = Math.Sqrt(Fmin / (Math.Pow(_mvm.WellViewModel.Wells[0].Q, 2) + Math.Pow(_mvm.WellViewModel.Wells[1].Q, 2) + Math.Pow(_mvm.WellViewModel.Wells[2].Q, 2)));
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

        public override async void Execute(object parameter)
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
    public class SurfaceCommand : MainViewCommand
    {
        public SurfaceCommand(MainViewModel mvm) : base(mvm)
        { }

        public override bool CanExecute(object parameter)
        {
            return true;
        }

        public override async void Execute(object parameter)
        {
            List<double> pZeros = new List<double>();
            List<double> kappas = new List<double>();
            List<double> ks = new List<double>();
            List<double> kappasC = new List<double>();
            List<double> ksC = new List<double>();
            string writePath1 = @"C:\Users\Rustam\Documents\Visual Studio 2017\Projects\Disser\ClientDesktop\ClientDesktop\Surface\K.txt";
            string writePath2 = @"C:\Users\Rustam\Documents\Visual Studio 2017\Projects\Disser\ClientDesktop\ClientDesktop\Surface\Kappa.txt";
            string writePath3 = @"C:\Users\Rustam\Documents\Visual Studio 2017\Projects\Disser\ClientDesktop\ClientDesktop\Surface\P0.txt";
            string writePath4 = @"C:\Users\Rustam\Documents\Visual Studio 2017\Projects\Disser\ClientDesktop\ClientDesktop\Surface\FminKP0.txt";
            string writePath5 = @"C:\Users\Rustam\Documents\Visual Studio 2017\Projects\Disser\ClientDesktop\ClientDesktop\Surface\FminKappaP0.txt";

            string writePath6 = @"C:\Users\Rustam\Documents\Visual Studio 2017\Projects\Disser\ClientDesktop\ClientDesktop\Surface\FPminKP0.txt";
            string writePath7 = @"C:\Users\Rustam\Documents\Visual Studio 2017\Projects\Disser\ClientDesktop\ClientDesktop\Surface\FPminKappaP0.txt";

            int kLeft = 6;
            int kRight = 12;
            int kappaLeft = 2;
            int kappaRight = 22;
            int p0Left = 0;
            int p0Rigth = 30;

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
            //using (StreamWriter sw = new StreamWriter(writePath4, false, Encoding.Default))
            //{
            //    for (int i = 0; i <= n; i++)
            //    {
            //        for (int j = 0; j <= n; j++)
            //        {
            //            for (int m = 1; m <= 3; m++)
            //            {
            //                #region Fill Obj
            //                object Q = (5 * m).ToString();
            //                object P = (5 * m).ToString();
            //                object P0 = pZeros[i].ToString();
            //                object T1 = (5 * (m - 1)).ToString();
            //                object T2 = (5 * m).ToString();
            //                object H0 = (1).ToString();
            //                object Mu = (5).ToString();
            //                object Rw = (0.1).ToString();
            //                object K = ks[j].ToString();
            //                object Kappa = (4).ToString();
            //                object Rs = (0.3).ToString();
            //                object Ksi = (0).ToString();
            //                object N = (100).ToString();
            //                List<object> obj = new List<object>()
            //                {
            //                    Q, P,P0, T1,T2, H0, Mu, Rw, K, Kappa, Rs, Ksi, N
            //                };
            //                #endregion
            //                if (_mvm.WellViewModel.Wells.Count<3)
            //                {
            //                    _mvm.WellViewModel.Add.Execute(obj.ToArray<object>());
            //                }
            //                else
            //                {
            //                    for (int l = 0; l < _mvm.WellViewModel.Wells.Count; l++)
            //                    {
            //                        _mvm.WellViewModel.Wells[l].K = Math.Pow(10.0, -15) * double.Parse(K.ToString());
            //                        _mvm.WellViewModel.Wells[l].P0 = Math.Pow(10.0, 6) * double.Parse(P0.ToString());
            //                    }
            //                }
            //            }

            //            #region P calculation                                                                                                       
            //            if (_mvm.ConsumptionsAndTimes == null || _mvm.ConsumptionsAndTimes?.Consumptions.Count == 0)
            //                for (int k = 0; k < _mvm.WellViewModel.Wells.Count; k++)
            //                    _mvm.WellViewModel.Wells[k].Mode = Mode.Direct;
            //            if (f)
            //            {
            //                _mvm.PressuresAndTimes = await (_mvm.CalculatePressures as CalculatePressuresCommand).SendWellsForPressures();
            //                f = false;
            //            }
            //            //plotViewModel.PlotTimePressures(PressuresAndTimes);                                                                       
            //            #endregion
            //            #region Q calculation                                                                                                       
            //            _mvm.ConsumptionsAndTimes = await (_mvm.CalculateConsumptions as CalculateConsumptionsCommand).SendWellsForConsumptions();
            //            //plotViewModel.PlotTimeConsumptions(MainViewModel.ConsumptionsAndTimes);                                                   
            //            double min = (_mvm.CalculateConsumptions as CalculateConsumptionsCommand).CalculateInitialFminQ();
            //            #endregion

            //            sw.Write(min);
            //            sw.Write(" ");
            //            _mvm.ConsumptionsAndTimes = null;
            //            //_mvm.Clear.Execute(null);
            //            //_mvm.WellViewModel.DeleteAllWellCommand.Execute(null);
            //        }
            //        sw.Write('\n');
            //    }
            //}
            #endregion
            //_mvm.WellViewModel.Wells.Clear();

            #region Fmin Kappa P0
            //using (StreamWriter sw = new StreamWriter(writePath5, false, Encoding.Default))
            //{
            //    for (int i = 0; i <= n; i++)
            //    {
            //        for (int j = 0; j <= n; j++)
            //        {
            //            for (int m = 1; m <= 3; m++)
            //            {
            //                #region Fill Obj
            //                object Q = (5 * m).ToString();
            //                object P = (5 * m).ToString();
            //                object P0 = pZeros[i].ToString();
            //                object T1 = (5 * (m - 1)).ToString();
            //                object T2 = (5 * m).ToString();
            //                object H0 = (1).ToString();
            //                object Mu = (5).ToString();
            //                object Rw = (0.1).ToString();
            //                object K = (6).ToString();
            //                object Kappa = kappas[j].ToString();
            //                object Rs = (0.3).ToString();
            //                object Ksi = (0).ToString();
            //                object N = (100).ToString();
            //                List<object> obj = new List<object>()
            //                {
            //                    Q, P,P0, T1,T2, H0, Mu, Rw, K, Kappa, Rs, Ksi, N
            //                };
            //                #endregion
            //                if (_mvm.WellViewModel.Wells.Count < 3)
            //                {
            //                    _mvm.WellViewModel.Add.Execute(obj.ToArray<object>());
            //                }
            //                else
            //                {
            //                    for (int l = 0; l < _mvm.WellViewModel.Wells.Count; l++)
            //                    {
            //                        _mvm.WellViewModel.Wells[l].Kappa = (1.0 / 3600.0) * double.Parse(Kappa.ToString());
            //                        _mvm.WellViewModel.Wells[l].P0 = Math.Pow(10.0, 6) * double.Parse(P0.ToString());
            //                    }
            //                }
            //            }

            //            #region P calculation
            //            if (_mvm.ConsumptionsAndTimes == null || _mvm.ConsumptionsAndTimes?.Consumptions.Count == 0)
            //                for (int k = 0; k < _mvm.WellViewModel.Wells.Count; k++)
            //                    _mvm.WellViewModel.Wells[k].Mode = Mode.Direct;
            //            if (f)
            //            {
            //                _mvm.PressuresAndTimes = await (_mvm.CalculatePressures as CalculatePressuresCommand).SendWellsForPressures();
            //                f = false;
            //            }
            //            //plotViewModel.PlotTimePressures(PressuresAndTimes);
            //            #endregion
            //            #region Q calculation
            //            _mvm.ConsumptionsAndTimes = await (_mvm.CalculateConsumptions as CalculateConsumptionsCommand).SendWellsForConsumptions();
            //            //plotViewModel.PlotTimeConsumptions(ConsumptionsAndTimes);
            //            double min = (_mvm.CalculateConsumptions as CalculateConsumptionsCommand).CalculateInitialFminQ();
            //            #endregion

            //            sw.Write(min);
            //            sw.Write(" ");

            //            _mvm.ConsumptionsAndTimes = null;
            //            //_mvm.Clear.Execute(null);
            //            //_mvm.WellViewModel.DeleteAllWellCommand.Execute(null);
            //        }
            //        sw.Write('\n');
            //    }
            //}
            #endregion

            #region FminP K P0
            //using (StreamWriter sw = new StreamWriter(writePath6, false, Encoding.Default))
            //{
            //    for (int i = 0; i <= n; i++)
            //    {
            //        for (int j = 0; j <= n; j++)
            //        {
            //            for (int m = 1; m <= 3; m++)
            //            {
            //                #region Fill Obj
            //                object Q = (5 * m).ToString();
            //                object P = (5 * m).ToString();
            //                object P0 = pZeros[i].ToString();
            //                object T1 = (5 * (m - 1)).ToString();
            //                object T2 = (5 * m).ToString();
            //                object H0 = (1).ToString();
            //                object Mu = (5).ToString();
            //                object Rw = (0.1).ToString();
            //                object K = ks[j].ToString();
            //                object Kappa = (4).ToString();
            //                object Rs = (0.3).ToString();
            //                object Ksi = (0).ToString();
            //                object N = (100).ToString();
            //                List<object> obj = new List<object>()
            //                {
            //                    Q, P,P0, T1,T2, H0, Mu, Rw, K, Kappa, Rs, Ksi, N
            //                };
            //                #endregion
            //                if (_mvm.WellViewModel.Wells.Count < 3)
            //                {
            //                    _mvm.WellViewModel.Add.Execute(obj.ToArray<object>());
            //                }
            //                else
            //                {
            //                    for (int l = 0; l < _mvm.WellViewModel.Wells.Count; l++)
            //                    {
            //                        _mvm.WellViewModel.Wells[l].K = Math.Pow(10.0, -15) * double.Parse(K.ToString());
            //                        _mvm.WellViewModel.Wells[l].P0 = Math.Pow(10.0, 6) * double.Parse(P0.ToString());
            //                    }
            //                }
            //            }

            //            #region Q calculation
            //            if (_mvm.PressuresAndTimes?.Pressures1f.Count == 0 || _mvm.PressuresAndTimes == null)
            //                for (int k = 0; k < _mvm.WellViewModel.Wells.Count; k++)
            //                    _mvm.WellViewModel.Wells[k].Mode = Mode.Reverse;                        
            //            if (f)
            //            {
            //                _mvm.ConsumptionsAndTimes = await (_mvm.CalculateConsumptions as CalculateConsumptionsCommand).SendWellsForConsumptions();
            //                f = false;
            //            }
            //            #endregion
            //            #region Q calculation                                                                                                       
            //            _mvm.PressuresAndTimes = await (_mvm.CalculatePressures as CalculatePressuresCommand).SendWellsForPressures();
            //            //plotViewModel.PlotTimeConsumptions(MainViewModel.ConsumptionsAndTimes);                                                   
            //            double min = (_mvm.CalculatePressures as CalculatePressuresCommand).CalculateInitialFminP();
            //            #endregion

            //            sw.Write(min);
            //            sw.Write(" ");

            //            _mvm.ConsumptionsAndTimes = null;
            //            //_mvm.Clear.Execute(null);
            //            //_mvm.WellViewModel.DeleteAllWellCommand.Execute(null);
            //        }
            //        sw.Write('\n');
            //    }
            //}
            #endregion

            #region FminP Kappa P0
            using (StreamWriter sw = new StreamWriter(writePath7, false, Encoding.Default))
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
                            object P0 = pZeros[i].ToString();
                            object T1 = (5 * (m - 1)).ToString();
                            object T2 = (5 * m).ToString();
                            object H0 = (1).ToString();
                            object Mu = (5).ToString();
                            object Rw = (0.1).ToString();
                            object K = (10).ToString();
                            object Kappa = kappas[j].ToString();
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
                                    _mvm.WellViewModel.Wells[l].Kappa = (1.0 / 3600.0) * double.Parse(Kappa.ToString());
                                    _mvm.WellViewModel.Wells[l].P0 = Math.Pow(10.0, 6) * double.Parse(P0.ToString());
                                }
                            }
                        }

                        #region Q calculation
                        if (_mvm.PressuresAndTimes?.Pressures1f.Count == 0 || _mvm.PressuresAndTimes == null)
                            for (int k = 0; k < _mvm.WellViewModel.Wells.Count; k++)
                                _mvm.WellViewModel.Wells[k].Mode = Mode.Reverse;
                        if (f)
                        {
                            _mvm.ConsumptionsAndTimes = await (_mvm.CalculateConsumptions as CalculateConsumptionsCommand).SendWellsForConsumptions();
                            f = false;
                        }

                        #endregion
                        #region Q calculation                                                                                                       
                        _mvm.PressuresAndTimes = await (_mvm.CalculatePressures as CalculatePressuresCommand).SendWellsForPressures();
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
        }
    }

}

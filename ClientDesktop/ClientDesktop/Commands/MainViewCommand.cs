using ClientDesktop.Models;
using ClientDesktop.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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

        async Task<PressuresAndTimes> SendWellsForPressures()
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

        private void CalculateInitialFminP()
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
        }
    }

    public class CalculateConsumptionCommand : MainViewCommand
    {
        public CalculateConsumptionCommand(MainViewModel mvm) : base(mvm)
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

        async Task<ConsumptionsAndTimes> SendWellsForConsumptions()
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
            _mvm.WellViewModel.Wells[0].CalculatedQ = consumptionsAndTimes.Consumptions[wellsList.Indexes[0] - 2];
            _mvm.WellViewModel.Wells[1].CalculatedQ = consumptionsAndTimes.Consumptions[wellsList.Indexes[1] - 1];
            _mvm.WellViewModel.Wells[2].CalculatedQ = consumptionsAndTimes.Consumptions[wellsList.Indexes[2] - 2];
            return consumptionsAndTimes;
        }

        private double CalculateInitialFminQ()
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
                            + Math.Pow((_mvm.WellViewModel.Wells[1].Q - _mvm.ConsumptionsAndTimes.Consumptions[_mvm.WellViewModel.Wells[0].N + _mvm.WellViewModel.Wells[1].N - 1]), 2)
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
}

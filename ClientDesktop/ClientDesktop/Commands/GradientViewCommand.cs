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
    abstract public class GradientViewCommand : ICommand
    {
        protected GradientViewModel _gvm;
        public GradientViewCommand(GradientViewModel gvm)
        {
            _gvm = gvm;
        }
        public event EventHandler CanExecuteChanged;
        public abstract bool CanExecute(object parameter);
        public abstract void Execute(object parameter);
    }

    public class NextStepCommand : GradientViewCommand
    {
        public NextStepCommand(GradientViewModel wvm) : base(wvm)
        {
        }
        public override bool CanExecute(object parameter)
        {
            return true;
        }

        public override async void Execute(object parameter)
        {
            var parameters = (object[])parameter;
            Gradient g;
            if (!_gvm.IsFirstTimeGradientClicked)
            {
                _gvm.GradientsAndConsumptions.Last().Gradient.Lambda = Convert.ToDouble(parameters[0]);
                _gvm.GradientsAndConsumptions.Last().Gradient.ChangedK = Convert.ToDouble(parameters[1]) * Math.Pow(10.0, -15);
                _gvm.GradientsAndConsumptions.Last().Gradient.ChangedKappa = Convert.ToDouble(parameters[2]) * (1.0 / 3600.0);
                _gvm.GradientsAndConsumptions.Last().Gradient.ChangedKsi = Convert.ToDouble(parameters[3]);
                _gvm.GradientsAndConsumptions.Last().Gradient.ChangedP0 = Convert.ToDouble(parameters[4]) * Math.Pow(10.0, 6);

                _gvm.GradientsAndConsumptions.Last().Gradient.DeltaK =
                    MainWindow.wellViewModel.Wells[0].K *
                    Math.Pow(10, Convert.ToDouble(parameters[5].ToString().Substring(parameters[5].ToString().IndexOf('-'),
                    parameters[5].ToString().IndexOf(')') - parameters[5].ToString().IndexOf('-'))));

                _gvm.GradientsAndConsumptions.Last().Gradient.DeltaKappa =
                    MainWindow.wellViewModel.Wells[0].Kappa *
                    Math.Pow(10, Convert.ToDouble(parameters[6].ToString().Substring(parameters[6].ToString().IndexOf('-'),
                    parameters[6].ToString().IndexOf(')') - parameters[6].ToString().IndexOf('-'))));

                _gvm.GradientsAndConsumptions.Last().Gradient.DeltaKsi =
                    MainWindow.wellViewModel.Wells[0].Ksi *
                    Math.Pow(10, Convert.ToDouble(parameters[7].ToString().Substring(parameters[7].ToString().IndexOf('-'),
                    parameters[7].ToString().IndexOf(')') - parameters[7].ToString().IndexOf('-'))));

                _gvm.GradientsAndConsumptions.Last().Gradient.DeltaP0 =
                    MainWindow.wellViewModel.Wells[0].P0 *
                    Math.Pow(10, Convert.ToDouble(parameters[8].ToString().Substring(parameters[8].ToString().IndexOf('-'),
                    parameters[8].ToString().IndexOf(')') - parameters[8].ToString().IndexOf('-'))));


                _gvm.GradientsAndConsumptions.Last().Gradient.UsedK = (bool?)parameters[9];
                _gvm.GradientsAndConsumptions.Last().Gradient.UsedKappa = (bool?)parameters[10];
                _gvm.GradientsAndConsumptions.Last().Gradient.UsedKsi =   (bool?)parameters[11];
                _gvm.GradientsAndConsumptions.Last().Gradient.UsedP0 = (bool?)parameters[12];

                g = _gvm.GradientsAndConsumptions.Last().Gradient;
                _gvm.IsFirstTimeGradientClicked = true;
            }
            else
            {
                g = _gvm.GradientsAndConsumptions.Last().Gradient;
                g.Lambda = Convert.ToDouble(parameters[0]);

                g.DeltaK =
                    MainWindow.wellViewModel.Wells[0].K *
                    Math.Pow(10, Convert.ToDouble(parameters[5].ToString().Substring(parameters[5].ToString().IndexOf('-'),
                    parameters[5].ToString().IndexOf(')') - parameters[5].ToString().IndexOf('-'))));

                g.DeltaKappa =
                    MainWindow.wellViewModel.Wells[0].Kappa *
                    Math.Pow(10, Convert.ToDouble(parameters[6].ToString().Substring(parameters[6].ToString().IndexOf('-'),
                    parameters[6].ToString().IndexOf(')') - parameters[6].ToString().IndexOf('-'))));

                g.DeltaKsi =
                    MainWindow.wellViewModel.Wells[0].Ksi *
                    Math.Pow(10, Convert.ToDouble(parameters[7].ToString().Substring(parameters[7].ToString().IndexOf('-'),
                    parameters[7].ToString().IndexOf(')') - parameters[7].ToString().IndexOf('-'))));

                g.DeltaP0 =
                    MainWindow.wellViewModel.Wells[0].P0 *
                    Math.Pow(10, Convert.ToDouble(parameters[8].ToString().Substring(parameters[8].ToString().IndexOf('-'),
                    parameters[8].ToString().IndexOf(')') - parameters[8].ToString().IndexOf('-'))));
            }

            GradientAndConsumptions gradientAndConsumptions = await SendWellsForGradient(g);
            if (gradientAndConsumptions.ConsumptionsAndTimes != null)
            {
                _gvm.GradientsAndConsumptions.Add(gradientAndConsumptions);
                _gvm.SelectedGradient = _gvm.GradientsAndConsumptions.Last().Gradient;
                _gvm.SelectedGradient = _gvm.GradientsAndConsumptions.Last().Gradient;
                //GradientClc.Fmin.Text = gradientToShow.FminQ.ToString();
                //GradientClc.CurrentK.Text = (gradientToShow.ChangedK * Math.Pow(10.0, 15)).ToString();
                //GradientClc.CurrentKappa.Text = (gradientToShow.ChangedKappa * 3600).ToString();
                //GradientClc.CurrentKsi.Text = gradientToShow.ChangedKsi.ToString();
                //GradientClc.CurrentP0.Text = (gradientToShow.ChangedP0 * Math.Pow(10.0, -6)).ToString();

                MainWindow.plotViewModel.PlotTimeConsumptions(gradientAndConsumptions.ConsumptionsAndTimes);
            }
        }

        async Task<GradientAndConsumptions> SendWellsForGradient(Gradient gradient)
        {
            WellsList wellsList = new WellsList(MainWindow.wellViewModel.Wells);
            GradientAndWellsList gradientAndWellsList = new GradientAndWellsList
            {
                Gradient = gradient,
                WellsList = wellsList,
            };
            var serializedProduct = JsonConvert.SerializeObject(gradientAndWellsList);
            HttpClient httpClient = new HttpClient();
            httpClient.Timeout = TimeSpan.FromMinutes(60);
            string apiUrl = "https://localhost:44308/api/values/nextgradient";
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));//ACCEPT header
            var content = new StringContent(serializedProduct, Encoding.UTF8, "application/json");
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, apiUrl);
            request.Content = new StringContent(serializedProduct, Encoding.UTF8, "application/json");//CONTENT-TYPE header
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var res = await httpClient.PostAsync(apiUrl, content);
            string responseBody = await res.Content.ReadAsStringAsync();
            GradientAndConsumptions gradientAndConsumptions = JsonConvert.DeserializeObject<GradientAndConsumptions>(responseBody);
            return gradientAndConsumptions;
        }
    }
}

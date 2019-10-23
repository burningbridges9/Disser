using OxyPlot;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ClientDesktop.ViewModels;
using ClientDesktop.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Linq;
using System;
using OxyPlot.Axes;
using OxyPlot.Series;
using System.Collections.Generic;
using System.Windows.Media;

namespace ClientDesktop
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static WellViewModel wellViewModel;
        public PlotViewModel plotViewModel;
        public static List<GradientAndConsumptions> GradientsAndConsumptions;
        public Gradient gradientToShow;
        public PressuresAndTimes PressuresAndTimes;
        public ConsumptionsAndTimes ConsumptionsAndTimes;
        public bool IsFirstTimeGradientClicked = false;
        public MainWindow()
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            InitializeComponent();
            wellViewModel = new WellViewModel();
            plotViewModel = new PlotViewModel();
            GradientsAndConsumptions = new List<GradientAndConsumptions>();
            gradientToShow = new Gradient();
            this.DataContext = plotViewModel;
        }

        private async void CalculatePressuresButton_Click(object sender, RoutedEventArgs e)
        {
            PressuresAndTimes = await SendWellsForPressures();
            PlotTimePressures(PressuresAndTimes);
        }      

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            plotViewModel.MyModel.Series.Clear();
            plotViewModel.MyModel.InvalidatePlot(true);
            Clear();
        }

        private async void CalculateConsumptionsButton_Click(object sender, RoutedEventArgs e)
        {
            ConsumptionsAndTimes = await SendWellsForConsumptions();
            PlotTimeConsumptions(ConsumptionsAndTimes);
            CalculateInitialFmin();
        }

        private void CalculateInitialFmin()
        {
            if (GradientsAndConsumptions.Count != 0)
                GradientsAndConsumptions.Clear();
            Gradient g = new Gradient
            {
                ChangedK = wellViewModel.Wells[0].K,
                ChangedKappa = wellViewModel.Wells[0].Kappa,
                ChangedKsi = wellViewModel.Wells[0].Ksi,
                ChangedP0 = wellViewModel.Wells[0].P0
            };
            GradientsAndConsumptions.Add(new GradientAndConsumptions());
            GradientsAndConsumptions[0].Gradient = g;
            GradientsAndConsumptions[0].ConsumptionsAndTimes = ConsumptionsAndTimes;
            double Fmin = 0;
            switch (wellViewModel.Wells.Count)
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
                    Fmin = Math.Pow((wellViewModel.Wells[0].Q - ConsumptionsAndTimes.Consumptions[PressuresAndTimes.Pressures1f.Count - 2]), 2)
                            + Math.Pow((wellViewModel.Wells[1].Q - ConsumptionsAndTimes.Consumptions[PressuresAndTimes.Pressures1s.Count - 2]), 2)
                            + Math.Pow((wellViewModel.Wells[2].Q - ConsumptionsAndTimes.Consumptions[PressuresAndTimes.Pressures1s.Count + PressuresAndTimes.Pressures1f.Count -3]), 2);
                    Fmin = Math.Sqrt(Fmin / (Math.Pow(wellViewModel.Wells[0].Q, 2) + Math.Pow(wellViewModel.Wells[1].Q, 2) + Math.Pow(wellViewModel.Wells[2].Q, 2)));
                    break;
            }
            GradientsAndConsumptions[0].Gradient.F = Fmin;
            GradientClc.GradientToShow = GradientsAndConsumptions.Last().Gradient;
            gradientToShow = GradientsAndConsumptions.Last().Gradient;
            GradientClc.Fmin.Text = gradientToShow.F.ToString();
        }

        async void Clear()
        {
            HttpClient httpClient = new HttpClient();
            string apiUrl = "https://localhost:44308/api/values";
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, apiUrl);
            var res = await httpClient.DeleteAsync(apiUrl);
            GradientsAndConsumptions.Clear();
            IsFirstTimeGradientClicked = false;
        }

        private async void NextGradButton_Click(object sender, RoutedEventArgs e)
        {
            Gradient g;
            if (!IsFirstTimeGradientClicked)
            {      
                GradientsAndConsumptions.Last().Gradient.Lambda = Convert.ToDouble(GradientClc.Lambda.Text);
                GradientsAndConsumptions.Last().Gradient.ChangedK = Convert.ToDouble(GradientClc.BeginK.Text) * Math.Pow(10.0, -15);
                GradientsAndConsumptions.Last().Gradient.ChangedKappa = Convert.ToDouble(GradientClc.BeginKappa.Text) * (1.0 / 3600.0);
                GradientsAndConsumptions.Last().Gradient.ChangedKsi = Convert.ToDouble(GradientClc.BeginKsi.Text);
                GradientsAndConsumptions.Last().Gradient.ChangedP0 = Convert.ToDouble(GradientClc.BeginP0.Text) * Math.Pow(10.0, -6);

                GradientsAndConsumptions.Last().Gradient.DeltaK = 
                    wellViewModel.Wells[0].K * 
                    Math.Pow(10, Convert.ToDouble(GradientClc.deltaK.Text.Substring(GradientClc.deltaK.Text.IndexOf('-'),
                    GradientClc.deltaK.Text.IndexOf(')') - GradientClc.deltaK.Text.IndexOf('-'))));

                GradientsAndConsumptions.Last().Gradient.DeltaKappa =
                    wellViewModel.Wells[0].Kappa *
                    Math.Pow(10, Convert.ToDouble(GradientClc.deltaKappa.Text.Substring(GradientClc.deltaKappa.Text.IndexOf('-'),
                    GradientClc.deltaKappa.Text.IndexOf(')') - GradientClc.deltaKappa.Text.IndexOf('-'))));

                GradientsAndConsumptions.Last().Gradient.DeltaKsi = 
                    wellViewModel.Wells[0].Ksi * 
                    Math.Pow(10, Convert.ToDouble(GradientClc.deltaKsi.Text.Substring(GradientClc.deltaKsi.Text.IndexOf('-'),
                    GradientClc.deltaKsi.Text.IndexOf(')') - GradientClc.deltaKsi.Text.IndexOf('-'))));

                GradientsAndConsumptions.Last().Gradient.DeltaP0 = 
                    wellViewModel.Wells[0].P0 * 
                    Math.Pow(10, Convert.ToDouble(GradientClc.deltaP0.Text.Substring(GradientClc.deltaP0.Text.IndexOf('-'),
                    GradientClc.deltaP0.Text.IndexOf(')') - GradientClc.deltaP0.Text.IndexOf('-'))));


                GradientsAndConsumptions.Last().Gradient.UsedK = GradientClc.UsedK.IsChecked;
                GradientsAndConsumptions.Last().Gradient.UsedKappa = GradientClc.UsedKappa.IsChecked;
                GradientsAndConsumptions.Last().Gradient.UsedKsi = GradientClc.UsedKsi.IsChecked;
                GradientsAndConsumptions.Last().Gradient.UsedP0 = GradientClc.UsedP0.IsChecked;

                g = GradientsAndConsumptions.Last().Gradient;
                IsFirstTimeGradientClicked = true;
            }
            else
            {
                g = GradientsAndConsumptions.Last().Gradient;
                g.Lambda = Convert.ToDouble(GradientClc.Lambda.Text);

                g.DeltaK =
                    wellViewModel.Wells[0].K *
                    Math.Pow(10, Convert.ToDouble(GradientClc.deltaK.Text.Substring(GradientClc.deltaK.Text.IndexOf('-'),
                    GradientClc.deltaK.Text.IndexOf(')') - GradientClc.deltaK.Text.IndexOf('-'))));

                g.DeltaKappa =
                    wellViewModel.Wells[0].Kappa *
                    Math.Pow(10, Convert.ToDouble(GradientClc.deltaKappa.Text.Substring(GradientClc.deltaKappa.Text.IndexOf('-'),
                    GradientClc.deltaKappa.Text.IndexOf(')') - GradientClc.deltaKappa.Text.IndexOf('-'))));

                g.DeltaKsi =
                    wellViewModel.Wells[0].Ksi *
                    Math.Pow(10, Convert.ToDouble(GradientClc.deltaKsi.Text.Substring(GradientClc.deltaKsi.Text.IndexOf('-'),
                    GradientClc.deltaKsi.Text.IndexOf(')') - GradientClc.deltaKsi.Text.IndexOf('-'))));

                g.DeltaP0 =
                    wellViewModel.Wells[0].P0 *
                    Math.Pow(10, Convert.ToDouble(GradientClc.deltaP0.Text.Substring(GradientClc.deltaP0.Text.IndexOf('-'),
                    GradientClc.deltaP0.Text.IndexOf(')') - GradientClc.deltaP0.Text.IndexOf('-'))));

            }
            //this
            GradientAndConsumptions gradientAndConsumptions = await SendWellsForGradient(g);
            if (gradientAndConsumptions.ConsumptionsAndTimes != null)
            {
                GradientsAndConsumptions.Add(gradientAndConsumptions);
                GradientClc.GradientToShow = GradientsAndConsumptions.Last().Gradient;
                gradientToShow = GradientsAndConsumptions.Last().Gradient;
                GradientClc.Fmin.Text = gradientToShow.F.ToString();
                GradientClc.CurrentK.Text = (gradientToShow.ChangedK * Math.Pow(10.0, 15)).ToString();
                GradientClc.CurrentKappa.Text = (gradientToShow.ChangedKappa * 3600).ToString();
                GradientClc.CurrentKsi.Text = gradientToShow.ChangedKsi.ToString();
                GradientClc.CurrentP0.Text = (gradientToShow.ChangedP0 * Math.Pow(10.0, 6)).ToString();

                PlotTimeConsumptions(gradientAndConsumptions.ConsumptionsAndTimes);
            }
        }

        private void PreviousGradButton_Click(object sender, RoutedEventArgs e)
        {
            if (GradientsAndConsumptions.Count > 1)
            {
                GradientsAndConsumptions.RemoveAt(GradientsAndConsumptions.Count - 1);
                GradientClc.GradientToShow = GradientsAndConsumptions.Last().Gradient;
                gradientToShow = GradientsAndConsumptions.Last().Gradient;
                GradientClc.Fmin.Text = gradientToShow.F.ToString();
                GradientClc.CurrentK.Text = (gradientToShow.ChangedK * Math.Pow(10.0, 15)).ToString();
                GradientClc.CurrentKappa.Text = (gradientToShow.ChangedKappa * 3600).ToString();
                GradientClc.CurrentKsi.Text = gradientToShow.ChangedKsi.ToString();
                GradientClc.CurrentP0.Text = (gradientToShow.ChangedP0 * Math.Pow(10.0, 6)).ToString();
                PlotTimeConsumptions(GradientsAndConsumptions.Last().ConsumptionsAndTimes);
                if (GradientsAndConsumptions.Count == 1)
                    IsFirstTimeGradientClicked = false;
            }
        }

        #region Send to server
        async Task<PressuresAndTimes> SendWellsForPressures()
        {
            WellsList wellsList = new WellsList { Wells = wellViewModel.Wells };
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
            return pressuresAndTimes;
        }
        async Task<ConsumptionsAndTimes> SendWellsForConsumptions()
        {
            WellsList wellsList = new WellsList { Wells = wellViewModel.Wells };
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
            return consumptionsAndTimes;
        }
        async Task<GradientAndConsumptions> SendWellsForGradient(Gradient gradient)
        {
            var serializedProduct = JsonConvert.SerializeObject(gradient);
            HttpClient httpClient = new HttpClient();
            httpClient.Timeout = TimeSpan.FromMinutes(10);
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
        #endregion

        #region Plot
        private void PlotTimePressures(PressuresAndTimes pressuresAndTimes)
        {
            plotViewModel.MyModel.Series.Clear();
            plotViewModel.MyModel.InvalidatePlot(true);
            var model = new PlotModel { LegendSymbolLength = 24 };
            switch (wellViewModel.Wells.Count)
            {
                case 1:
                    model.Series.Add(new LineSeries
                    {
                        Color = OxyColors.SkyBlue,
                        MarkerType = MarkerType.None,
                        MarkerStrokeThickness = 1.5
                    });
                    foreach (var pt in pressuresAndTimes.Pressures1.Zip(pressuresAndTimes.Times1, Tuple.Create))
                    {
                        (model.Series[0] as LineSeries).Points.Add(new DataPoint(pt.Item2 / 3600.0, pt.Item1 * Math.Pow(10, -6)));
                    }
                    plotViewModel.MyModel = model;
                    break;

                case 2:
                    model.Series.Add(new LineSeries
                    {
                        Color = OxyColors.SkyBlue,
                        MarkerType = MarkerType.None,
                        MarkerStrokeThickness = 1.5
                    });
                    model.Series.Add(new LineSeries
                    {
                        Color = OxyColors.Blue,
                        MarkerType = MarkerType.Cross,
                        MarkerSize = 0.5,
                        MarkerStroke = OxyColors.Blue,
                        MarkerFill = OxyColors.Blue,
                        MarkerStrokeThickness = 0.5
                    });
                    model.Series.Add(new LineSeries
                    {
                        Color = OxyColors.Black,
                        MarkerStrokeThickness = 1.5
                    });
                    foreach (var pt in pressuresAndTimes.Pressures1f.Zip(pressuresAndTimes.Times1f, Tuple.Create))
                    {
                        (model.Series[0] as LineSeries).Points.Add(new DataPoint(pt.Item2 / 3600.0, pt.Item1 * Math.Pow(10, -6)));
                    }
                    foreach (var pt in pressuresAndTimes.Pressures1s.Zip(pressuresAndTimes.Times1s, Tuple.Create))
                    {
                        (model.Series[1] as LineSeries).Points.Add(new DataPoint(pt.Item2 / 3600.0, pt.Item1 * Math.Pow(10, -6)));
                    }
                    foreach (var pt in pressuresAndTimes.Pressures2.Zip(pressuresAndTimes.Times2, Tuple.Create))
                    {
                        (model.Series[2] as LineSeries).Points.Add(new DataPoint(pt.Item2 / 3600.0, pt.Item1 * Math.Pow(10, -6)));
                    }
                    plotViewModel.MyModel = model;
                    break;

                case 3:
                    model.Series.Add(new LineSeries
                    {
                        Color = OxyColors.SkyBlue,
                        MarkerType = MarkerType.None,
                        MarkerStrokeThickness = 1.5
                    });
                    model.Series.Add(new LineSeries
                    {
                        Color = OxyColors.Blue,
                        MarkerType = MarkerType.Cross,
                        MarkerSize = 0.5,
                        MarkerStroke = OxyColors.Blue,
                        MarkerFill = OxyColors.Blue,
                        MarkerStrokeThickness = 0.5
                    });
                    model.Series.Add(new LineSeries
                    {
                        Color = OxyColors.Black,
                        MarkerStrokeThickness = 1.5
                    });
                    model.Series.Add(new LineSeries
                    {
                        Color = OxyColors.Green,
                        MarkerType = MarkerType.Cross,
                        MarkerSize = 0.5,
                        MarkerStroke = OxyColors.Green,
                        MarkerFill = OxyColors.Green,
                        MarkerStrokeThickness = 0.5
                    });
                    model.Series.Add(new LineSeries
                    {
                        Color = OxyColors.Red,
                        MarkerStrokeThickness = 1.5
                    });
                    foreach (var pt in pressuresAndTimes.Pressures1f.Zip(pressuresAndTimes.Times1f, Tuple.Create))
                    {
                        (model.Series[0] as LineSeries).Points.Add(new DataPoint(pt.Item2 / 3600.0, pt.Item1 * Math.Pow(10, -6)));
                    }
                    foreach (var pt in pressuresAndTimes.Pressures1s.Zip(pressuresAndTimes.Times1s, Tuple.Create))
                    {
                        (model.Series[1] as LineSeries).Points.Add(new DataPoint(pt.Item2 / 3600.0, pt.Item1 * Math.Pow(10, -6)));
                    }
                    foreach (var pt in pressuresAndTimes.Pressures2f.Zip(pressuresAndTimes.Times2f, Tuple.Create))
                    {
                        (model.Series[2] as LineSeries).Points.Add(new DataPoint(pt.Item2 / 3600.0, pt.Item1 * Math.Pow(10, -6)));
                    }
                    foreach (var pt in pressuresAndTimes.Pressures2s.Zip(pressuresAndTimes.Times2s, Tuple.Create))
                    {
                        (model.Series[3] as LineSeries).Points.Add(new DataPoint(pt.Item2 / 3600.0, pt.Item1 * Math.Pow(10, -6)));
                    }
                    foreach (var pt in pressuresAndTimes.Pressures3.Zip(pressuresAndTimes.Times3, Tuple.Create))
                    {
                        (model.Series[4] as LineSeries).Points.Add(new DataPoint(pt.Item2 / 3600.0, pt.Item1 * Math.Pow(10, -6)));
                    }
                    plotViewModel.MyModel = model;
                    break;
            }
        }

        private void PlotTimeConsumptions(ConsumptionsAndTimes consumptionsAndTimes)
        {
            plotViewModel.MyModel.Series.Clear();
            plotViewModel.MyModel.InvalidatePlot(true);
            var model = new PlotModel { LegendSymbolLength = 24 };
            model.Series.Add(new LineSeries
            {
                Color = OxyColors.SkyBlue,
                MarkerType = MarkerType.None,
                MarkerStrokeThickness = 1.5
            });
            foreach (var pt in consumptionsAndTimes.Consumptions.Zip(consumptionsAndTimes.Times, Tuple.Create))
            {
                (model.Series[0] as LineSeries).Points.Add(new DataPoint(pt.Item2 / 3600.0, pt.Item1 * 24.0 * 3600.0));
            }
            model.Series.Add(new LineSeries
            {
                Color = OxyColors.Red,
                MarkerType = MarkerType.None,
                MarkerStrokeThickness = 1.5
            });
            foreach (var pt in consumptionsAndTimes.StaticConsumptions.Zip(consumptionsAndTimes.Times, Tuple.Create))
            {
                (model.Series[1] as LineSeries).Points.Add(new DataPoint(pt.Item2 / 3600.0, pt.Item1 * 24.0 * 3600.0));
            }
            plotViewModel.MyModel = model;
        }
        #endregion
    }
}

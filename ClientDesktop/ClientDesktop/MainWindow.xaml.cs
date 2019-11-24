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
using ClientDesktop.Layouts;

namespace ClientDesktop
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static WellViewModel wellViewModel;
        public static PlotViewModel plotViewModel;
        public static QGradientViewModel gradientViewModel;
        public static PGradientViewModel pGradientViewModel;
        public PressuresAndTimes PressuresAndTimes;
        public ConsumptionsAndTimes ConsumptionsAndTimes;
        public bool IsFirstTimeGradientClicked = false;
        public MainWindow()
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            InitializeComponent();
            wellViewModel = PressureCalcLayout.wellViewModel;
            gradientViewModel = QGradientCalc.gradientViewModel;
            pGradientViewModel = PGradientCalc.gradientViewModel;
            plotViewModel = new PlotViewModel();
            this.DataContext = plotViewModel;
        }

        private async void CalculatePressuresButton_Click(object sender, RoutedEventArgs e)
        {
            if (ConsumptionsAndTimes==null || ConsumptionsAndTimes?.Consumptions.Count == 0)
                for (int i = 0; i < wellViewModel.Wells.Count; i++)
                    wellViewModel.Wells[i].Mode = Mode.Direct;
            PressuresAndTimes = await SendWellsForPressures();
            plotViewModel.PlotTimePressures(PressuresAndTimes);
            CalculateInitialFminP();
        }      

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            plotViewModel.MyModel.Series.Clear();
            plotViewModel.MyModel.InvalidatePlot(true);
            Clear();
        }

        private async void CalculateConsumptionsButton_Click(object sender, RoutedEventArgs e)
        {
            if (PressuresAndTimes?.Pressures1f.Count == 0 || PressuresAndTimes==null)
                for (int i = 0; i < wellViewModel.Wells.Count; i++)
                    wellViewModel.Wells[i].Mode = Mode.Reverse;
            ConsumptionsAndTimes = await SendWellsForConsumptions();
            plotViewModel.PlotTimeConsumptions(ConsumptionsAndTimes);
            CalculateInitialFminQ();
        }

        private void CalculateInitialFminQ()
        {
            
            if (gradientViewModel.GradientsAndConsumptions.Count != 0)
                gradientViewModel.GradientsAndConsumptions.Clear();
            QGradient g = new QGradient
            {
                ChangedK = wellViewModel.Wells[0].K,
                ChangedKappa = wellViewModel.Wells[0].Kappa,
                ChangedKsi = wellViewModel.Wells[0].Ksi,
                ChangedP0 = wellViewModel.Wells[0].P0
            };
            gradientViewModel.GradientsAndConsumptions.Add(new QGradientAndConsumptions());
            gradientViewModel.GradientsAndConsumptions[0].QGradient = g;
            gradientViewModel.GradientsAndConsumptions[0].ConsumptionsAndTimes = ConsumptionsAndTimes;
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
                    Fmin = Math.Pow((wellViewModel.Wells[0].Q - ConsumptionsAndTimes.Consumptions[wellViewModel.Wells[0].N - 2]), 2)
                            + Math.Pow((wellViewModel.Wells[1].Q - ConsumptionsAndTimes.Consumptions[wellViewModel.Wells[0].N + wellViewModel.Wells[1].N - 1]), 2)
                            + Math.Pow((wellViewModel.Wells[2].Q - ConsumptionsAndTimes.Consumptions.Last()), 2);
                    Fmin = Math.Sqrt(Fmin / (Math.Pow(wellViewModel.Wells[0].Q, 2) + Math.Pow(wellViewModel.Wells[1].Q, 2) + Math.Pow(wellViewModel.Wells[2].Q, 2)));
                    break;
            }
            gradientViewModel.GradientsAndConsumptions[0].QGradient.FminQ = Fmin;
            //GradientClc.GradientToShow = GradientsAndConsumptions.Last().Gradient;
            gradientViewModel.SelectedGradient = gradientViewModel.GradientsAndConsumptions.Last().QGradient;
            gradientViewModel.Gradients.Add(gradientViewModel.GradientsAndConsumptions[0].QGradient);
            //GradientClc.FQmin.Text = gradientViewModel.SelectedGradient.FminQ.ToString();
        }

        private void CalculateInitialFminP()
        {

            if (pGradientViewModel.PGradientAndPressures.Count != 0)
                pGradientViewModel.PGradientAndPressures.Clear();
            PGradient g = new PGradient
            {
                ChangedK = wellViewModel.Wells[0].K,
                ChangedKappa = wellViewModel.Wells[0].Kappa,
                ChangedKsi = wellViewModel.Wells[0].Ksi,
                ChangedP0 = wellViewModel.Wells[0].P0
            };
            pGradientViewModel.PGradientAndPressures.Add(new PGradientAndPressures());
            pGradientViewModel.PGradientAndPressures[0].PGradient = g;
            pGradientViewModel.PGradientAndPressures[0].PressuresAndTimes = PressuresAndTimes;
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
                    Fmin = Math.Pow((wellViewModel.Wells[0].P - PressuresAndTimes.Pressures1f.Last()), 2)
                            + Math.Pow((wellViewModel.Wells[1].Q - PressuresAndTimes.Pressures2f.Last()), 2)
                            + Math.Pow((wellViewModel.Wells[2].Q - PressuresAndTimes.Pressures3.Last()), 2);
                    Fmin = Math.Sqrt(Fmin / (Math.Pow(wellViewModel.Wells[0].P, 2) + Math.Pow(wellViewModel.Wells[1].P, 2) + Math.Pow(wellViewModel.Wells[2].P, 2)));
                    break;
            }
            pGradientViewModel.PGradientAndPressures[0].PGradient.FminP = Fmin;
            //GradientClc.GradientToShow = GradientsAndConsumptions.Last().Gradient;
            pGradientViewModel.SelectedGradient = pGradientViewModel.PGradientAndPressures.Last().PGradient;
            pGradientViewModel.Gradients.Add(pGradientViewModel.PGradientAndPressures[0].PGradient);
            //GradientClc.FQmin.Text = gradientViewModel.SelectedGradient.FminQ.ToString();
        }

        async void Clear()
        {
            HttpClient httpClient = new HttpClient();
            string apiUrl = "https://localhost:44308/api/values";
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, apiUrl);
            var res = await httpClient.DeleteAsync(apiUrl);
            gradientViewModel.GradientsAndConsumptions.Clear();

            PressuresAndTimes?.Pressures1f?.Clear();
            PressuresAndTimes?.Pressures1s?.Clear();
            PressuresAndTimes?.Pressures2f?.Clear();
            PressuresAndTimes?.Pressures2s?.Clear();
            PressuresAndTimes?.Pressures3?.Clear();
            PressuresAndTimes?.Times1f?.Clear();
            PressuresAndTimes?.Times1s?.Clear();
            PressuresAndTimes?.Times2f?.Clear();
            PressuresAndTimes?.Times2s?.Clear();
            PressuresAndTimes?.Times3?.Clear();
            ConsumptionsAndTimes?.Consumptions?.Clear();
            ConsumptionsAndTimes?.StaticConsumptions?.Clear();
            ConsumptionsAndTimes?.Times?.Clear();

            IsFirstTimeGradientClicked = false;
        }
        
        #region Send to server
        async static Task<PressuresAndTimes> SendWellsForPressures()
        {
            WellsList wellsList = new WellsList(wellViewModel.Wells.ToList());
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
            wellViewModel.Wells[0].CalculatedP = pressuresAndTimes.Pressures1f.Last();
            wellViewModel.Wells[1].CalculatedP = pressuresAndTimes.Pressures2f.Last();
            wellViewModel.Wells[2].CalculatedP = pressuresAndTimes.Pressures3.Last();
            return pressuresAndTimes;
        }
        async static Task<ConsumptionsAndTimes> SendWellsForConsumptions()
        {
            WellsList wellsList = new WellsList(wellViewModel.Wells.ToList());
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
            wellViewModel.Wells[0].CalculatedQ = consumptionsAndTimes.Consumptions[wellsList.Indexes[0]-2];
            wellViewModel.Wells[1].CalculatedQ = consumptionsAndTimes.Consumptions[wellsList.Indexes[1]-1];
            wellViewModel.Wells[2].CalculatedQ = consumptionsAndTimes.Consumptions[wellsList.Indexes[2]-2];
            return consumptionsAndTimes;
        }
        async static Task<QGradientAndConsumptions> SendWellsForGradient(QGradient gradient)
        {
            WellsList wellsList = new WellsList(wellViewModel.Wells.ToList());
            QGradientAndWellsList gradientAndWellsList = new QGradientAndWellsList
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
            QGradientAndConsumptions gradientAndConsumptions = JsonConvert.DeserializeObject<QGradientAndConsumptions>(responseBody);
            return gradientAndConsumptions;
        }
        #endregion

        #region Plot
        //private void PlotTimePressures(PressuresAndTimes pressuresAndTimes)
        //{
        //    plotViewModel.MyModel.Series.Clear();
        //    plotViewModel.MyModel.InvalidatePlot(true);
        //    var model = new PlotModel { LegendSymbolLength = 24 };
        //    switch (wellViewModel.Wells.Count)
        //    {
        //        case 1:
        //            model.Series.Add(new LineSeries
        //            {
        //                Color = OxyColors.SkyBlue,
        //                MarkerType = MarkerType.None,
        //                MarkerStrokeThickness = 1.5
        //            });
        //            foreach (var pt in pressuresAndTimes.Pressures1.Zip(pressuresAndTimes.Times1, Tuple.Create))
        //            {
        //                (model.Series[0] as LineSeries).Points.Add(new DataPoint(pt.Item2 / 3600.0, pt.Item1 * Math.Pow(10, -6)));
        //            }
        //            plotViewModel.MyModel = model;
        //            break;

        //        case 2:
        //            model.Series.Add(new LineSeries
        //            {
        //                Color = OxyColors.SkyBlue,
        //                MarkerType = MarkerType.None,
        //                MarkerStrokeThickness = 1.5
        //            });
        //            model.Series.Add(new LineSeries
        //            {
        //                Color = OxyColors.Blue,
        //                MarkerType = MarkerType.Cross,
        //                MarkerSize = 0.5,
        //                MarkerStroke = OxyColors.Blue,
        //                MarkerFill = OxyColors.Blue,
        //                MarkerStrokeThickness = 0.5
        //            });
        //            model.Series.Add(new LineSeries
        //            {
        //                Color = OxyColors.Black,
        //                MarkerStrokeThickness = 1.5
        //            });
        //            foreach (var pt in pressuresAndTimes.Pressures1f.Zip(pressuresAndTimes.Times1f, Tuple.Create))
        //            {
        //                (model.Series[0] as LineSeries).Points.Add(new DataPoint(pt.Item2 / 3600.0, pt.Item1 * Math.Pow(10, -6)));
        //            }
        //            foreach (var pt in pressuresAndTimes.Pressures1s.Zip(pressuresAndTimes.Times1s, Tuple.Create))
        //            {
        //                (model.Series[1] as LineSeries).Points.Add(new DataPoint(pt.Item2 / 3600.0, pt.Item1 * Math.Pow(10, -6)));
        //            }
        //            foreach (var pt in pressuresAndTimes.Pressures2.Zip(pressuresAndTimes.Times2, Tuple.Create))
        //            {
        //                (model.Series[2] as LineSeries).Points.Add(new DataPoint(pt.Item2 / 3600.0, pt.Item1 * Math.Pow(10, -6)));
        //            }
        //            plotViewModel.MyModel = model;
        //            break;

        //        case 3:
        //            model.Series.Add(new LineSeries
        //            {
        //                Color = OxyColors.SkyBlue,
        //                MarkerType = MarkerType.None,
        //                MarkerStrokeThickness = 1.5
        //            });
        //            model.Series.Add(new LineSeries
        //            {
        //                Color = OxyColors.Blue,
        //                MarkerType = MarkerType.Cross,
        //                MarkerSize = 0.5,
        //                MarkerStroke = OxyColors.Blue,
        //                MarkerFill = OxyColors.Blue,
        //                MarkerStrokeThickness = 0.5
        //            });
        //            model.Series.Add(new LineSeries
        //            {
        //                Color = OxyColors.Black,
        //                MarkerStrokeThickness = 1.5
        //            });
        //            model.Series.Add(new LineSeries
        //            {
        //                Color = OxyColors.Green,
        //                MarkerType = MarkerType.Cross,
        //                MarkerSize = 0.5,
        //                MarkerStroke = OxyColors.Green,
        //                MarkerFill = OxyColors.Green,
        //                MarkerStrokeThickness = 0.5
        //            });
        //            model.Series.Add(new LineSeries
        //            {
        //                Color = OxyColors.Red,
        //                MarkerStrokeThickness = 1.5
        //            });
        //            foreach (var pt in pressuresAndTimes.Pressures1f.Zip(pressuresAndTimes.Times1f, Tuple.Create))
        //            {
        //                (model.Series[0] as LineSeries).Points.Add(new DataPoint(pt.Item2 / 3600.0, pt.Item1 * Math.Pow(10, -6)));
        //            }
        //            foreach (var pt in pressuresAndTimes.Pressures1s.Zip(pressuresAndTimes.Times1s, Tuple.Create))
        //            {
        //                (model.Series[1] as LineSeries).Points.Add(new DataPoint(pt.Item2 / 3600.0, pt.Item1 * Math.Pow(10, -6)));
        //            }
        //            foreach (var pt in pressuresAndTimes.Pressures2f.Zip(pressuresAndTimes.Times2f, Tuple.Create))
        //            {
        //                (model.Series[2] as LineSeries).Points.Add(new DataPoint(pt.Item2 / 3600.0, pt.Item1 * Math.Pow(10, -6)));
        //            }
        //            foreach (var pt in pressuresAndTimes.Pressures2s.Zip(pressuresAndTimes.Times2s, Tuple.Create))
        //            {
        //                (model.Series[3] as LineSeries).Points.Add(new DataPoint(pt.Item2 / 3600.0, pt.Item1 * Math.Pow(10, -6)));
        //            }
        //            foreach (var pt in pressuresAndTimes.Pressures3.Zip(pressuresAndTimes.Times3, Tuple.Create))
        //            {
        //                (model.Series[4] as LineSeries).Points.Add(new DataPoint(pt.Item2 / 3600.0, pt.Item1 * Math.Pow(10, -6)));
        //            }
        //            if (pressuresAndTimes.StaticPressures != null)
        //            {
        //                model.Series.Add(new LineSeries
        //                {
        //                    Color = OxyColors.BlueViolet,
        //                    MarkerType = MarkerType.Cross,
        //                    MarkerStrokeThickness = 2.5
        //                });
        //                var tempTimes = new List<Double>();
        //                tempTimes.AddRange(pressuresAndTimes.Times1f);
        //                tempTimes.AddRange(pressuresAndTimes.Times1s);
        //                foreach (var pt in pressuresAndTimes.StaticPressures.Zip(tempTimes, Tuple.Create))
        //                {
        //                    (model.Series[5] as LineSeries).Points.Add(new DataPoint(pt.Item2 / 3600.0, pt.Item1 * Math.Pow(10, -6)));
        //                }
        //            }
        //            plotViewModel.MyModel = model;
        //            break;
        //    }
        //}

        //public void PlotTimeConsumptions(ConsumptionsAndTimes consumptionsAndTimes)
        //{
        //    plotViewModel.MyModel.Series.Clear();
        //    plotViewModel.MyModel.InvalidatePlot(true);
        //    var model = new PlotModel { LegendSymbolLength = 24 };
        //    model.Series.Add(new LineSeries
        //    {
        //        Color = OxyColors.SkyBlue,
        //        MarkerType = MarkerType.None,
        //        MarkerStrokeThickness = 1.5
        //    });
        //    foreach (var pt in consumptionsAndTimes.Consumptions.Zip(consumptionsAndTimes.Times, Tuple.Create))
        //    {
        //        (model.Series[0] as LineSeries).Points.Add(new DataPoint(pt.Item2 / 3600.0, pt.Item1 * 24.0 * 3600.0));
        //    }
        //    if (consumptionsAndTimes.StaticConsumptions != null)
        //    {
        //        model.Series.Add(new LineSeries
        //        {
        //            Color = OxyColors.Red,
        //            MarkerType = MarkerType.None,
        //            MarkerStrokeThickness = 1.5
        //        });
        //        foreach (var pt in consumptionsAndTimes.StaticConsumptions.Zip(consumptionsAndTimes.Times, Tuple.Create))
        //        {
        //            (model.Series[1] as LineSeries).Points.Add(new DataPoint(pt.Item2 / 3600.0, pt.Item1 * 24.0 * 3600.0));
        //        }
        //    }
        //    plotViewModel.MyModel = model;
        //}
        #endregion
    }
}

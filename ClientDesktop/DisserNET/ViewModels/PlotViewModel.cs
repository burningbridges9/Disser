using DisserNET.Calculs;
using DisserNET.Models;
using OxyPlot;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DisserNET.ViewModels
{
    public class PlotViewModel : INotifyPropertyChanged
    {
        public WellViewModel wellViewModel { get; set; }
        public PlotViewModel()
        {
            this.PlotModel = new PlotModel();
            var s1 = new LineSeries
            {
                Color = OxyColors.SkyBlue,
                MarkerType = MarkerType.None,
                //MarkerSize = 3,
                //MarkerStroke = OxyColors.White,
                //MarkerFill = OxyColors.SkyBlue,
                MarkerStrokeThickness = 1.5
            };
            var s3 = new LineSeries
            {
                Color = OxyColors.Black,
                //MarkerType = MarkerType.Cross,
                //MarkerSize = 3,
                //MarkerStroke = OxyColors.White,
                //MarkerFill = OxyColors.SkyBlue,
                MarkerStrokeThickness = 1.5
            };
            var s2 = new LineSeries
            {
                Color = OxyColors.AliceBlue,
                MarkerType = MarkerType.Cross,
                MarkerSize = 1.5,
                MarkerStroke = OxyColors.White,
                MarkerFill = OxyColors.AliceBlue,
                MarkerStrokeThickness = 1.5
            };

            PlotModel.Series.Add(s1);

            PlotModel.Series.Add(s2);
            PlotModel.Series.Add(s3);
            PlotModel.InvalidatePlot(true);
        }

        public void PlotTimeConsumptions(ConsumptionsAndTimes consumptionsAndTimes)
        {
            PlotModel.Series.Clear();
            PlotModel.InvalidatePlot(true);
            var model = new PlotModel { LegendSymbolLength = 24 };
            model.LegendTitle = "Расходы Q = Q(t)";
            model.LegendPosition = LegendPosition.RightBottom;
            model.Series.Add(new LineSeries
            {
                Color = OxyColors.SkyBlue,
                MarkerType = MarkerType.None,
                MarkerStrokeThickness = 1.5,
                Title = "Рассчитанные расходы"
            });
            foreach (var pt in consumptionsAndTimes.Consumptions.Zip(consumptionsAndTimes.Times, Tuple.Create))
            {
                (model.Series[0] as LineSeries).Points.Add(new DataPoint(pt.Item2 / 3600.0, pt.Item1 * 24.0 * 3600.0));
            }
            if (consumptionsAndTimes.StaticConsumptions != null)
            {
                model.Series.Add(new LineSeries
                {
                    Color = OxyColors.Red,
                    MarkerType = MarkerType.None,
                    MarkerStrokeThickness = 1.5,
                    Title = "Замеренные расходы"
                });
                foreach (var pt in consumptionsAndTimes.StaticConsumptions.Zip(consumptionsAndTimes.Times, Tuple.Create))
                {
                    (model.Series[1] as LineSeries).Points.Add(new DataPoint(pt.Item2 / 3600.0, pt.Item1 * 24.0 * 3600.0));
                }
            }
            PlotModel = model;
        }

        public void PlotTimePressures(PressuresAndTimes pressuresAndTimes)
        {
            PlotModel.Series.Clear();
            PlotModel.InvalidatePlot(true);
            var model = new PlotModel { LegendSymbolLength = 24 };
            model.LegendTitle = "Давления P = P(t)";
            model.LegendPosition = LegendPosition.RightBottom;
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
                    PlotModel = model;
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
                    PlotModel = model;
                    break;

                case 3:
                    model.Series.Add(new LineSeries
                    {
                        Color = OxyColors.SkyBlue,
                        MarkerType = MarkerType.None,
                        MarkerStrokeThickness = 1.5,
                        Title = "Рассчитанное давление P1"
                    });
                    model.Series.Add(new LineSeries
                    {
                        Color = OxyColors.Blue,
                        MarkerType = MarkerType.Cross,
                        MarkerSize = 0.5,
                        MarkerStroke = OxyColors.Blue,
                        MarkerFill = OxyColors.Blue,
                        MarkerStrokeThickness = 0.5,
                        Title = "Рассчитанное давление P1 без учета расходов Q2 и Q3"
                    });
                    model.Series.Add(new LineSeries
                    {
                        Color = OxyColors.Black,
                        MarkerStrokeThickness = 1.5,
                        Title = "Рассчитанное давление P2"
                    });
                    model.Series.Add(new LineSeries
                    {
                        Color = OxyColors.Green,
                        MarkerType = MarkerType.Cross,
                        MarkerSize = 0.5,
                        MarkerStroke = OxyColors.Green,
                        MarkerFill = OxyColors.Green,
                        MarkerStrokeThickness = 0.5,
                        Title = "Рассчитанное давление P2 без учета расхода Q3"
                    });
                    model.Series.Add(new LineSeries
                    {
                        Color = OxyColors.Red,
                        MarkerStrokeThickness = 1.5,
                        Title = "Рассчитанное давление P3"
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
                    if (pressuresAndTimes.StaticPressures != null)
                    {
                        model.Series.Add(new LineSeries
                        {
                            Color = OxyColors.BlueViolet,
                            MarkerType = MarkerType.Cross,
                            MarkerStrokeThickness = 2.5,
                            Title = "Замеренное давление P"
                        });
                        var tempTimes = new List<Double>();
                        tempTimes.AddRange(pressuresAndTimes.Times1f);
                        tempTimes.AddRange(pressuresAndTimes.Times1s);
                        foreach (var pt in pressuresAndTimes.StaticPressures.Zip(tempTimes, Tuple.Create))
                        {
                            (model.Series[5] as LineSeries).Points.Add(new DataPoint(pt.Item2 / 3600.0, pt.Item1 * Math.Pow(10, -6)));
                        }
                    }
                    PlotModel = model;
                    break;
            }
        }

        public void CleanUp()
        {
            PlotModel.Series.Clear();
            PlotModel.InvalidatePlot(true);
        }

        private PlotModel plotModel { get; set; }
        public PlotModel PlotModel
        {
            get { return plotModel; }
            set
            {
                plotModel = value;
                OnPropertyChanged("PlotModel");
            }
        }


        public void SetupScatterModel()
        {
            var model = new PlotModel { Title = "ScatterSeries" };
            var scatterSeries = new ScatterSeries { MarkerType = MarkerType.Circle };
            model.Series.Add(scatterSeries);
            PlotModel = model;
        }

        public void OnAccept(AcceptedValueMH obj)
        {
            var x = obj.K;
            var y = obj.Kappa;
            var size = 3;
            var series = PlotModel.Series.FirstOrDefault() as ScatterSeries;
            series.Points.Add(new ScatterPoint(x, y, size, 2));
            PlotModel.Series.Clear();
            PlotModel.Series.Add(series);
            PlotModel.InvalidatePlot(true);
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
